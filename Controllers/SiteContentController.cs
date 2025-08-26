using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clinic_Backend.Data;
using Clinic_Backend.Models;

namespace Clinic_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SiteContentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SiteContentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{contentType}")]
        public async Task<ActionResult<SiteContent>> GetContent(string contentType)
        {
            var content = await _context.SiteContents
                .FirstOrDefaultAsync(c => c.ContentType.ToLower() == contentType.ToLower());

            if (content == null)
                return NotFound();

            return Ok(content);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SiteContent>> CreateOrUpdateContent([FromBody] SiteContent content)
        {
            var existingContent = await _context.SiteContents
                .FirstOrDefaultAsync(c => c.ContentType == content.ContentType);

            if (existingContent != null)
            {
                existingContent.Content = content.Content;
                existingContent.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.SiteContents.Add(content);
            }

            await _context.SaveChangesAsync();

            return Ok(existingContent ?? content);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SiteContent>>> GetAllContent()
        {
            var contents = await _context.SiteContents.ToListAsync();
            return Ok(contents);
        }
    }
}