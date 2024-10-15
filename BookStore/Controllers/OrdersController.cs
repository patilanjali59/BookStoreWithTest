using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly BookstoreDbContext _context;

        public OrdersController(BookstoreDbContext context)
        {
            _context = context;
        }

        // Placing book order
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            var book = await _context.Books.FindAsync(order.BookId);

            if (book == null || book.Stock < order.Quantity)
            {
                return BadRequest("Insufficient stock or invalid book.");
            }
            var userId = GetUserIdFromToken();
            order.UserId = Convert.ToInt32(userId);
            book.Stock -= order.Quantity;
            _context.orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateOrder), new { id = order.Id }, order);
        }

        public string GetUserIdFromToken()
        {
            if (User.Identity is ClaimsIdentity identity)
            {              
                var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                 {
                    return userIdClaim.Value; 

                 }
            }
            return null;
        }
    }
}
