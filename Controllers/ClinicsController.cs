using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clinic_Backend.Data;
using Clinic_Backend.Models;
using Clinic_Backend.DTOs;
using Clinic_Backend.Services;

namespace Clinic_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public ClinicsController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClinicListDto>>> GetClinics(
            [FromQuery] string? location = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? search = null
        )
        {
            var query = _context.Clinics.AsQueryable();

            if (!string.IsNullOrEmpty(location))
                query = query.Where(c => c.Location.ToLower().Contains(location.ToLower()));

            if (minPrice.HasValue)
                query = query.Where(c => c.PriceRangeMin >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(c => c.PriceRangeMax <= maxPrice.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Name.ToLower().Contains(search.ToLower()) ||
                                         c.ShortDescription.ToLower().Contains(search.ToLower()));

            var clinics = await query.Select(c => new ClinicListDto
            {
                Id = c.Id,
                Name = c.Name,
                Location = c.Location,
                ShortDescription = c.ShortDescription,
                PriceRangeMin = c.PriceRangeMin,
                PriceRangeMax = c.PriceRangeMax,
                EstimatedAppointmentMonths = c.EstimatedAppointmentMonths,
                AverageRating = c.Reviews.Any() ? c.Reviews.Average(r => r.Rating) : 0,
                BannerImagePath = c.BannerImagePath
            }).ToListAsync();

            return Ok(clinics);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicDetailDto>> GetClinic(int id)
        {
            var clinic = await _context.Clinics
                .Include(c => c.Images)
                .Include(c => c.Videos)
                .Include(c => c.Treatments)
                .Include(c => c.Doctors)
                .Include(c => c.Reviews)
                    .ThenInclude(r => r.Images)
                .Include(c => c.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (clinic == null)
                return NotFound();

            var clinicDto = new ClinicDetailDto
            {
                Id = clinic.Id,
                Name = clinic.Name,
                ClinicEmail = clinic.ClinicEmail,
                Location = clinic.Location,
                ShortDescription = clinic.ShortDescription,
                AboutSection = clinic.AboutSection,
                PriceRangeMin = clinic.PriceRangeMin,
                PriceRangeMax = clinic.PriceRangeMax,
                EstimatedAppointmentMonths = clinic.EstimatedAppointmentMonths,
                AverageRating = clinic.Reviews.Any() ? clinic.Reviews.Average(r => r.Rating) : 0,
                BannerImagePath = clinic.BannerImagePath,
                Images = clinic.Images.Select(i => i.ImagePath).ToList(),
                Videos = clinic.Videos.Select(v => v.VideoPath).ToList(),
                Treatments = clinic.Treatments.Select(t => new TreatmentDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Price = t.Price
                }).ToList(),
                Doctors = clinic.Doctors.Select(d => new DoctorDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Bio = d.Bio,
                    ImagePath = d.ImagePath
                }).ToList(),
                Reviews = clinic.Reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    ClinicId = r.ClinicId,
                    UserId = r.UserId,
                    UserName = r.IsAnonymous ? "Anonymous" : r.User.UserName,
                    Rating = r.Rating,
                    ReviewText = r.ReviewText,
                    NumberOfGrafts = r.NumberOfGrafts,
                    Price = r.Price,
                    TransplantType = r.TransplantType,
                    BookingToAppointmentDays = r.BookingToAppointmentDays,
                    CreatedAt = r.CreatedAt,
                    Images = r.Images.Select(ri => ri.ImagePath).ToList()
                }).ToList(),
                ReviewCount = clinic.Reviews.Count
            };

            return Ok(clinicDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Clinic>> CreateClinic([FromBody] CreateClinicDto clinicDto)
        {
            var clinic = new Clinic
            {
                Name = clinicDto.Name,
                ClinicEmail = clinicDto.ClinicEmail,
                Location = clinicDto.Location,
                ShortDescription = clinicDto.ShortDescription,
                AboutSection = clinicDto.AboutSection,
                PriceRangeMin = clinicDto.PriceRangeMin,
                PriceRangeMax = clinicDto.PriceRangeMax,
                EstimatedAppointmentMonths = clinicDto.EstimatedAppointmentMonths
            };

            _context.Clinics.Add(clinic);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClinic), new { id = clinic.Id }, clinic);
        }

        [HttpPost("{id}/banner")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadBanner(int id, IFormFile file)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
                return NotFound();

            if (file == null || file.Length == 0)
                return BadRequest("Invalid file.");

            // Delete existing banner if any
            if (!string.IsNullOrEmpty(clinic.BannerImagePath))
                _fileService.DeleteFile(clinic.BannerImagePath);

            var filePath = await _fileService.SaveFileAsync(file, "banners");
            clinic.BannerImagePath = filePath;

            await _context.SaveChangesAsync();

            return Ok(new { BannerPath = filePath });
        }

        [HttpPost("{id}/images")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadImages(int id, List<IFormFile> files)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
                return NotFound();

            if (files == null || files.Count == 0)
                return BadRequest("No files provided.");


            var uploadedImages = new List<string>();

            foreach (var file in files)
            {
                var filePath = await _fileService.SaveFileAsync(file, "clinic-images");
                if (!string.IsNullOrEmpty(filePath))
                {
                    var clinicImage = new ClinicImage
                    {
                        ClinicId = id,
                        ImagePath = filePath
                    };
                    _context.ClinicImages.Add(clinicImage);
                    uploadedImages.Add(filePath);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { Images = uploadedImages });
        }

        [HttpPost("{id}/videos")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadVideos(int id, List<IFormFile> files)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
                return NotFound();

            if (files == null || files.Count == 0)
                return BadRequest("No files provided");

            var uploadedVideos = new List<string>();

            foreach (var file in files)
            {
                var filePath = await _fileService.SaveFileAsync(file, "clinic-videos");
                if (!string.IsNullOrEmpty(filePath))
                {
                    var clinicVideo = new ClinicVideo
                    {
                        ClinicId = id,
                        VideoPath = filePath
                    };
                    _context.ClinicVideos.Add(clinicVideo);
                    uploadedVideos.Add(filePath);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { Videos = uploadedVideos });
        }

        [HttpPost("{id}/treatments")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTreatment(int id, [FromBody] TreatmentDto treatmentDto)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
                return NotFound();

            var treatment = new Treatment
            {
                ClinicId = id,
                Name = treatmentDto.Name,
                Description = treatmentDto.Description,
                Price = treatmentDto.Price,
            };

            _context.Treatments.Add(treatment);
            await _context.SaveChangesAsync();

            return Ok(treatment);
        }

        [HttpPost("{id}/doctors")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddDoctor(int id, [FromBody] DoctorDto doctorDto)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
                return NotFound();

            var doctor = new Doctor
            {
                ClinicId = id,
                Name = doctorDto.Name,
                Bio = doctorDto.Bio,
                ImagePath = doctorDto.ImagePath,
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return Ok(doctor);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateClinic(int id, [FromBody] CreateClinicDto clinicDto)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
                return NotFound();

            clinic.Name = clinicDto.Name;
            clinic.ClinicEmail = clinicDto.ClinicEmail;
            clinic.Location = clinicDto.Location;
            clinic.ShortDescription = clinicDto.ShortDescription;
            clinic.AboutSection = clinicDto.AboutSection;
            clinic.PriceRangeMin = clinicDto.PriceRangeMin;
            clinic.PriceRangeMax = clinicDto.PriceRangeMax;
            clinic.EstimatedAppointmentMonths = clinicDto.EstimatedAppointmentMonths;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            var clinic = await _context.Clinics
                .Include(c => c.Images)
                .Include(c => c.Videos)
                .Include(c => c.Treatments)
                .Include(c => c.Doctors)
                .Include(c => c.Reviews)
                    .ThenInclude(r => r.Images)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (clinic == null)
                return NotFound();

            // Delete associated files
            if (!string.IsNullOrEmpty(clinic.BannerImagePath))
                _fileService.DeleteFile(clinic.BannerImagePath);

            foreach (var image in clinic.Images)
                _fileService.DeleteFile(image.ImagePath);

            foreach (var video in clinic.Videos)
                _fileService.DeleteFile(video.VideoPath);

            foreach (var review in clinic.Reviews)
            {
                foreach (var reviewImage in review.Images)
                    _fileService.DeleteFile(reviewImage.ImagePath);
            }

            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}