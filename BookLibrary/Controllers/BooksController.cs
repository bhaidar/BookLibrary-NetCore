﻿using BookLibrary.Common;
using BookLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [FormatFilter]
    public class BooksController : Controller
    {
        private readonly IBookRepository _repository;
        private readonly ILogger _logger;

        public BooksController(IBookRepository repository, ILogger<BooksController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/books
        [HttpGet]
        public async Task<IEnumerable<Book>> Get()
        {
            var books = await _repository.GetAllBooksAsync();
            _logger.LogInformation(
                ApplicationEvents.LibraryQueried, 
                "Retrieved {Count} books.", 
                books.Count());

            return books;
        }

        // GET api/books/5
        [HttpGet("{id}", Name = "GetById")]
        public async Task<ActionResult<Book>> Get(string id)
        {
            var book = await _repository.FindBookAsync(id);
            if (book != null)
            {
                _logger.LogInformation(
                    ApplicationEvents.BookQueried, 
                    "Retrieved book '{Id}'.", 
                    book.Id);

                return Ok(book);
            }

            _logger.LogInformation(
                ApplicationEvents.BookNotFound, 
                "Failed to retrieve book '{Id}'.", 
                id);

            return NotFound();
        }

        // POST api/books
        [HttpPost]
        public async Task<ActionResult<Book>> Post(Book book)
        {
            var newBook = await _repository.AddBookAsync(book);
            _logger.LogInformation(
                ApplicationEvents.BookCreated,
                "Added book with id '{Id}'.",
                newBook.Id);

            return CreatedAtRoute("GetById", new { id = newBook.Id }, newBook);
        }

        // PUT api/books/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Book>> Put(string id, [FromBody]Book input)
        {
            bool isUpdated = await _repository.UpdateBookAsync(id, input);
            if (isUpdated)
            {
                _logger.LogInformation(
                    ApplicationEvents.BookUpdated,
                    "Updated book with id '{Id}'.",
                    id);
                return Ok(input);
            }

            _logger.LogInformation(
                ApplicationEvents.BookNotFound,
                "Failed to update book '{Id}'.",
                id);

            return NotFound();
        }

        // DELETE api/books/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            bool isDeleted = await _repository.RemoveBookAsync(id);
            if (isDeleted)
            {
                _logger.LogInformation(
                    ApplicationEvents.BookDeleted, 
                    "Removed book with id '{Id}'.", 
                    id);

                return StatusCode(StatusCodes.Status204NoContent);
            }

            _logger.LogInformation(
                ApplicationEvents.BookNotFound, 
                "Failed to delete book '{Id}'.", 
                id);

            return NotFound();
        }
    }
}
