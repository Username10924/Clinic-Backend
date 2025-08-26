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
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public ReviewsController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Review>> CreateReview([FromBody] CreateReviewDto reviewDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if user has a completed booking with this clinic
            var hasCompletedBooking = await _context.Bookings
                .AnyAsync(b => b.UserId == userId &&
                              b.ClinicId == reviewDto.ClinicId &&
                              b.Status == "Completed");

            if (!hasCompletedBooking)
                return BadRequest("You can only review clinics where you have completed a booking");

            // Check if user already reviewed this clinic
            var existingReview = await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.ClinicId == reviewDto.ClinicId);

            if (existingReview)
                return BadRequest("You have already reviewed this clinic");

            var review = new Review
            {
                ClinicId = reviewDto.ClinicId,
                UserId = userId,
                IsAnonymous = reviewDto.IsAnonymous,
                Rating = reviewDto.Rating,
                ReviewText = reviewDto.ReviewText,
                NumberOfGrafts = reviewDto.NumberOfGrafts,
                Price = reviewDto.Price,
                TransplantType = reviewDto.TransplantType,
                BookingToAppointmentDays = reviewDto.BookingToAppointmentDays
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Update clinic average rating
            await UpdateClinicRating(reviewDto.ClinicId);

            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
        }

        [HttpPost("{id}/images")]
        [Authorize]
        public async Task<IActionResult> UploadReviewImages(int id, List<IFormFile> files)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (review == null)
                return NotFound();

            if (files == null || !files.Any())
                return BadRequest("No files provided");

            var uploadedImages = new List<string>();

            foreach (var file in files)
            {
                var filePath = await _fileService.SaveFileAsync(file, "review-images");
                if (!string.IsNullOrEmpty(filePath))
                {
                    var reviewImage = new ReviewImage
                    {
                        ReviewId = id,
                        ImagePath = filePath
                    };
                    _context.ReviewImages.Add(reviewImage);
                    uploadedImages.Add(filePath);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { Images = uploadedImages });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return NotFound();

            var reviewDto = new ReviewDto
            {
                Id = review.Id,
                ClinicId = review.ClinicId,
                UserId = review.UserId,
                UserName = review.IsAnonymous ? "Anonymous" : $"{review.User.FirstName} {review.User.LastName}",
                IsAnonymous = review.IsAnonymous,
                Rating = review.Rating,
                ReviewText = review.ReviewText,
                NumberOfGrafts = review.NumberOfGrafts,
                Price = review.Price,
                TransplantType = review.TransplantType,
                BookingToAppointmentDays = review.BookingToAppointmentDays,
                CreatedAt = review.CreatedAt,
                Images = review.Images.Select(i => i.ImagePath).ToList()
            };

            return Ok(reviewDto);
        }

        [HttpGet("clinic/{clinicId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetClinicReviews(int clinicId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Images)
                .Where(r => r.ClinicId == clinicId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    ClinicId = r.ClinicId,
                    UserId = r.UserId,
                    UserName = r.IsAnonymous ? "Anonymous" : $"{r.User.FirstName} {r.User.LastName}",
                    IsAnonymous = r.IsAnonymous,
                    Rating = r.Rating,
                    ReviewText = r.ReviewText,
                    NumberOfGrafts = r.NumberOfGrafts,
                    Price = r.Price,
                    TransplantType = r.TransplantType,
                    BookingToAppointmentDays = r.BookingToAppointmentDays,
                    CreatedAt = r.CreatedAt,
                    Images = r.Images.Select(i => i.ImagePath).ToList()
                })
                .ToListAsync();

            return Ok(reviews);
        }

        private async Task UpdateClinicRating(int clinicId)
        {
            var clinic = await _context.Clinics.FindAsync(clinicId);
            if (clinic == null) return;

            var averageRating = await _context.Reviews
                .Where(r => r.ClinicId == clinicId)
                .AverageAsync(r => (double)r.Rating);

            clinic.AverageRating = Math.Round(averageRating, 1);
            await _context.SaveChangesAsync();
        }
    }
}