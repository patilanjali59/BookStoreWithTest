using System;
using System.Threading.Tasks;
using BookStore.Models;
using BookStore.Controllers;
using BookStore.Data;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Tests
{
    public class OrdersControllerTests
    {
        private readonly OrdersController _controller;
        private readonly BookstoreDbContext _context;

        public OrdersControllerTests()
        {
            // Setting up in-memory database for testing
            var options = new DbContextOptionsBuilder<BookstoreDbContext>()
                .UseInMemoryDatabase(databaseName: "BookstoreTestDb")
                .Options;

            _context = new BookstoreDbContext(options);
            _controller = new OrdersController(_context);

            _context.Books.RemoveRange(_context.Books); // Clear existing data
            _context.SaveChanges();

            // Seed the in-memory database with test data
            _context.Books.Add(new Book { Id = 1, Title = "Test Book", Author = "Karl", Price = 100, Category = "Tech", Stock = 10, CoverImageUrl = "abc" });
            _context.SaveChanges();
        }


        [Fact]
        public async Task CreateOrder_InvalidBook_ReturnsBadRequest()
        {
            var newOrder = new Order { BookId = 99, Quantity = 5 }; // Invalid book ID

            var result = await _controller.CreateOrder(newOrder);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
            (result.Result as BadRequestObjectResult).Value.Should().Be("Insufficient stock or invalid book.");
        }

        [Fact]
        public async Task CreateOrder_InsufficientStock_ReturnsBadRequest()
        {
            
            var newOrder = new Order { BookId = 1, Quantity = 50 }; // Exceeds available stock

            var result = await _controller.CreateOrder(newOrder);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
            (result.Result as BadRequestObjectResult).Value.Should().Be("Insufficient stock or invalid book.");
        }
    }
}
