using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Clinic_Backend.Data;
using Clinic_Backend.Models;
using Clinic_Backend.DTOs;
using Clinic_Backend.Services;

namespace Clinic_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public BookingsController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Booking>> CreateBooking([FromBody] BookingDto bookingDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = new Booking
            {
                ClinicId = bookingDto.ClinicId,
                UserId = userId,
                Name = bookingDto.Name,
                Email = bookingDto.Email,
                Phone = bookingDto.Phone,
                DateOfBirth = bookingDto.DateOfBirth,
                Sex = bookingDto.Sex,
                City = bookingDto.City,
                Country = bookingDto.Country,
                DesiredTreatment = bookingDto.DesiredTreatment,
                HairLossType = bookingDto.HairLossType,
                HairColor = bookingDto.HairColor,
                HairLossDuration = bookingDto.HairLossDuration,
                HasPreviousTransplant = bookingDto.HasPreviousTransplant,
                PreferredTiming = bookingDto.PreferredTiming
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }

        [HttpPost("{id}/images")]
        [Authorize]
        public async Task<IActionResult> UploadBookingImages(int id, List<IFormFile> images)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
            {
                return NotFound("Booking not found or you do not have permission to upload images for this booking.");
            }

            if (images == null || images.Count == 0)
            {
                return BadRequest("No images provided.");
            }

            var bookingImages = new List<string>();
            foreach (var image in images)
            {
                var imageUrl = await _fileService.SaveFileAsync(image, "booking-images");
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return StatusCode(500, "Error uploading image.");
                }

                var bookingImage = new BookingImage
                {
                    BookingId = booking.Id,
                    ImagePath = imageUrl
                };

                _context.BookingImages.Add(bookingImage);
                bookingImages.Add(imageUrl);
            }
            await _context.SaveChangesAsync();
            return Ok(new { Images = bookingImages });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var query = _context.Bookings
                .Include(b => b.Clinic)
                .Include(b => b.User)
                .Include(b => b.Images)
                .AsQueryable();

            if (!isAdmin)
                query = query.Where(b => b.UserId == userId);

            var booking = await query.FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            var bookingDto = new BookingDto
            {
                Id = booking.Id,
                ClinicId = booking.ClinicId,
                ClinicName = booking.Clinic.Name,
                UserId = booking.UserId,
                UserName = $"{booking.User.FirstName} {booking.User.LastName}",
                UserEmail = booking.User.Email,
                Name = booking.Name,
                Email = booking.Email,
                Phone = booking.Phone,
                DateOfBirth = booking.DateOfBirth,
                Sex = booking.Sex,
                City = booking.City,
                Country = booking.Country,
                DesiredTreatment = booking.DesiredTreatment,
                HairLossType = booking.HairLossType,
                HairColor = booking.HairColor,
                HairLossDuration = booking.HairLossDuration,
                HasPreviousTransplant = booking.HasPreviousTransplant,
                PreferredTiming = booking.PreferredTiming,
                CreatedAt = booking.CreatedAt,
                Status = booking.Status,
                Images = booking.Images.Select(img => img.ImagePath).ToList(),
            };

            return Ok(bookingDto);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            var query = _context.Bookings
                .Include(b => b.Clinic)
                .Include(b => b.User)
                .Include(b => b.Images)
                .AsQueryable();

            if (!isAdmin)
                query = query.Where(b => b.UserId == userId);

            var bookings = await query.Select(b => new BookingDto
            {
                Id = b.Id,
                ClinicId = b.ClinicId,
                ClinicName = b.Clinic.Name,
                UserId = b.UserId,
                UserName = $"{b.User.FirstName} {b.User.LastName}",
                UserEmail = b.User.Email,
                Name = b.Name,
                Email = b.Email,
                Phone = b.Phone,
                DateOfBirth = b.DateOfBirth,
                Sex = b.Sex,
                City = b.City,
                Country = b.Country,
                DesiredTreatment = b.DesiredTreatment,
                HairLossType = b.HairLossType,
                HairColor = b.HairColor,
                HairLossDuration = b.HairLossDuration,
                HasPreviousTransplant = b.HasPreviousTransplant,
                PreferredTiming = b.PreferredTiming,
                CreatedAt = b.CreatedAt,
                Status = b.Status,
                Images = b.Images.Select(i => i.ImagePath).ToList()
            }).ToListAsync();

            return Ok(bookings);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] string status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            booking.Status = status;
            await _context.SaveChangesAsync();

            return Ok(new { Status = status });
        }
    }
}