using System;
using System.Collections.Generic;
using BookStore.Controllers;
using BookStore.Data;
using BookStore.Models;
using Xunit;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;


namespace BookStore.Tests
{
    public class BookControllerTests
    {
        private readonly DbContextOptions<BookstoreDbContext> _options;
        private readonly BookstoreDbContext _context;
        private readonly BooksController _controller;

        public BookControllerTests()
        {
            // Set up InMemory Database options
            _options = new DbContextOptionsBuilder<BookstoreDbContext>()
                .UseInMemoryDatabase(databaseName: "TestBookStoreDb")
                .Options;

            _context = new BookstoreDbContext(_options);

            _context.Books.RemoveRange(_context.Books); // Clearing existing data
            _context.SaveChanges();

            // Seed some data into the InMemory database
            _context.Books.Add(new Book { Id = 4, Title = "ABC", Author ="Karl", Price = 500, Category="Tech", Stock = 30, CoverImageUrl="abc" });
            _context.SaveChanges();

            // Initialize the controller to test
            _controller = new BooksController(_context);
        }

        [Fact]
        public async void GetBook_ExistingBook_ReturnsOkResultWithBook()
        {          
            var result = await _controller.GetBook(4);

            result.Result.Should().BeOfType<OkObjectResult>(); 
            var book = (result.Result as OkObjectResult)?.Value as Book;
            book.Should().NotBeNull();
            book?.Title.Should().Be("ABC");
        }

        [Fact]
        public async void GetBook_NonExistingBook_ReturnsNotFound()
        {          
            var result = await _controller.GetBook(99); 
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

       
    }
}
