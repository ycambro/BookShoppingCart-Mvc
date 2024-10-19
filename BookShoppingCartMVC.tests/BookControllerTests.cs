using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BookShoppingCartMvcUI.Controllers;
using BookShoppingCartMvcUI.Repositories;
using BookShoppingCartMvcUI.Models;
using BookShoppingCartMvcUI.Models.DTOs;
using BookShoppingCartMvcUI.Shared;

namespace BookShoppingCartMvcUI.Tests.Controllers
{
    public class BookControllerTests
    {
        private Mock<IBookRepository> _mockBookRepo;
        private Mock<IGenreRepository> _mockGenreRepo;
        private Mock<IFileService> _mockFileService;
        private BookController _controller;

        public BookControllerTests()
        {
            _mockBookRepo = new Mock<IBookRepository>();
            _mockGenreRepo = new Mock<IGenreRepository>();
            _mockFileService = new Mock<IFileService>();
            _controller = new BookController(_mockBookRepo.Object, _mockGenreRepo.Object, _mockFileService.Object);
        }

        // Prueba para el método GET: AddBook
        [Fact]
        public async Task AddBook_Get_Returns_ViewResult_With_BookDTO()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new Genre { Id = 1, GenreName = "Fiction" },
                new Genre { Id = 2, GenreName = "Non-Fiction" }
            };
            _mockGenreRepo.Setup(repo => repo.GetGenres()).ReturnsAsync(genres);

            // Act
            var result = await _controller.AddBook();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<BookDTO>(viewResult.Model);
            Assert.NotNull(model.GenreList);
            Assert.Equal(2, model.GenreList.Count());
        }

        // Prueba para el método POST: AddBook con archivo de imagen muy grande
        [Fact]
        public async Task AddBook_Post_ImageTooLarge_ReturnsViewWithError()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new Genre { Id = 1, GenreName = "Fiction" },
                new Genre { Id = 2, GenreName = "Non-Fiction" }
            };
            _mockGenreRepo.Setup(repo => repo.GetGenres()).ReturnsAsync(genres);

            var largeImageFile = new Mock<IFormFile>();
            largeImageFile.Setup(f => f.Length).Returns(2 * 1024 * 1024);

            var bookToAdd = new BookDTO { ImageFile = largeImageFile.Object };

            // Act
            var result = await _controller.AddBook(bookToAdd);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Image file can not exceed 1 MB", _controller.TempData["errorMessage"]);
        }

        // Prueba para el método POST: AddBook con modelo válido
        [Fact]
        public async Task AddBook_Post_ValidBook_AddsBookAndRedirects()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new Genre { Id = 1, GenreName = "Fiction" },
                new Genre { Id = 2, GenreName = "Non-Fiction" }
            };
            _mockGenreRepo.Setup(repo => repo.GetGenres()).ReturnsAsync(genres);
            _mockFileService.Setup(service => service.SaveFile(It.IsAny<IFormFile>(), It.IsAny<string[]>())).ReturnsAsync("image.jpg");

            var bookToAdd = new BookDTO
            {
                BookName = "Test Book",
                AuthorName = "Test Author",
                GenreId = 1,
                Price = 19.99
            };

            // Act
            var result = await _controller.AddBook(bookToAdd);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddBook", redirectResult.ActionName);
            Assert.Equal("Book is added successfully", _controller.TempData["successMessage"]);
        }

        // Prueba para el método GET: UpdateBook
        [Fact]
        public async Task UpdateBook_Get_Returns_ViewResult_With_BookDTO()
        {
            // Arrange
            var book = new Book
            {
                Id = 1,
                BookName = "Test Book",
                AuthorName = "Test Author",
                GenreId = 1,
                Price = 19.99,
                Image = "image.jpg"
            };
            var genres = new List<Genre>
            {
                new Genre { Id = 1, GenreName = "Fiction" },
                new Genre { Id = 2, GenreName = "Non-Fiction" }
            };
            _mockBookRepo.Setup(repo => repo.GetBookById(1)).ReturnsAsync(book);
            _mockGenreRepo.Setup(repo => repo.GetGenres()).ReturnsAsync(genres);

            // Act
            var result = await _controller.UpdateBook(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<BookDTO>(viewResult.Model);
            Assert.Equal("Test Book", model.BookName);
            Assert.Equal(1, model.GenreId);
        }

        // Prueba para el método POST: UpdateBook con imagen actualizada
        [Fact]
        public async Task UpdateBook_Post_ValidBook_UpdatesBookAndRedirects()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new Genre { Id = 1, GenreName = "Fiction" },
                new Genre { Id = 2, GenreName = "Non-Fiction" }
            };
            _mockGenreRepo.Setup(repo => repo.GetGenres()).ReturnsAsync(genres);
            _mockFileService.Setup(service => service.SaveFile(It.IsAny<IFormFile>(), It.IsAny<string[]>())).ReturnsAsync("newImage.jpg");

            var bookToUpdate = new BookDTO
            {
                Id = 1,
                BookName = "Updated Book",
                AuthorName = "Updated Author",
                GenreId = 1,
                Price = 25.99,
                Image = "oldImage.jpg",
                ImageFile = new Mock<IFormFile>().Object
            };

            // Act
            var result = await _controller.UpdateBook(bookToUpdate);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Book is updated successfully", _controller.TempData["successMessage"]);
        }

        // Prueba para el método GET: DeleteBook
        [Fact]
        public async Task DeleteBook_BookExists_DeletesBookAndRedirects()
        {
            // Arrange
            var book = new Book
            {
                Id = 1,
                BookName = "Test Book",
                Image = "image.jpg"
            };
            _mockBookRepo.Setup(repo => repo.GetBookById(1)).ReturnsAsync(book);

            // Act
            var result = await _controller.DeleteBook(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockBookRepo.Verify(repo => repo.DeleteBook(book), Times.Once);
            _mockFileService.Verify(service => service.DeleteFile("image.jpg"), Times.Once);
        }
    }
}

