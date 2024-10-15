using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookstoreDbContext _context;
        public BooksController(BookstoreDbContext context)
        {
            _context = context;
        }

        // Listing all the books available 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> Getbooks()
        {
            return await _context.Books.ToListAsync();
        }

        // Displaying book as per bookId
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if(book == null || book.Stock <= 0)
            {
                return NotFound("Book not found or out of stock.");
            }

            return Ok(book);

        }

        // Adding book
        [HttpPost("AddBook")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Book>>> AddBook(Book book)
        {
            
            var bookToAdd = _context.Books.FromSqlInterpolated($"Select * from Books where Title = {book.Title}").ToList();

            if (bookToAdd == null )
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return Ok("Book added");               
            }
            if(bookToAdd != null)
            {               
                bookToAdd[0].Stock = bookToAdd[0].Stock + book.Stock;
                await _context.SaveChangesAsync();
                return Ok("Stock updated");
            }

            return NoContent();

        }

        // Uploading book cover image
        [Authorize(Roles = "Admin")]
        [HttpPost("/admin/book/{id}/upload-cover")]
        public async Task<IActionResult> UploadCover(int id, IFormFile coverImage)
        {
            if (coverImage == null || coverImage.Length == 0)
                return BadRequest("No image uploaded.");

            var path = Path.Combine("wwwroot/images", $"{id}.jpg");
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await coverImage.CopyToAsync(stream);
            }
            return Ok("Cover image uploaded successfully.");
        }
    }
}
