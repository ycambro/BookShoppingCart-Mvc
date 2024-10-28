using Moq;
using Microsoft.AspNetCore.Mvc;
using BookShoppingCartMvcUI.Controllers;
using BookShoppingCartMvcUI.Repositories;
using BookShoppingCartMvcUI.Models;
using Microsoft.Extensions.Logging;
using BookShoppingCartMvcUI.Models.DTOs;

namespace BookShoppingCartMvcUI.Tests.Controllers
{
    public class HomeControllerTests
    {
        private Mock<IHomeRepository> _mockHomeRepo;
        private Mock<ILogger<HomeController>> _ilogger;
        private HomeController _controller;

        public HomeControllerTests()
        {
            var _ilogger = new Mock<ILogger<HomeController>>();
            _mockHomeRepo = new Mock<IHomeRepository>();
            _controller = new HomeController(_ilogger.Object,_mockHomeRepo.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfBooks()
        {
            // Arrange
            _mockHomeRepo.Setup(repo => repo.GetBooks("", 0)).ReturnsAsync(GetTestBooks());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookDisplayModel>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Books.ToList().Count);
        }

        private List<Book> GetTestBooks()
        {
            return new List<Book>
        {
            new Book { Id = 1, BookName = "El Principito", GenreId = 1 },
            new Book { Id = 2, BookName = "El proceso", GenreId = 1 }
        };
        }

        [Fact]
        public async Task Index_NoBooksFound_ReturnsEmptyList()
        {
            // Arrange
            _mockHomeRepo.Setup(repo => repo.GetBooks("", 0)).ReturnsAsync(new List<Book>());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookDisplayModel>(viewResult.ViewData.Model);
            Assert.Empty(model.Books);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfGenres()
        {
            // Arrange
            _mockHomeRepo.Setup(repo => repo.Genres()).ReturnsAsync(GetTestGenres());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookDisplayModel>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Genres.ToList().Count);
        }

        private List<Genre> GetTestGenres()
        {
            return new List<Genre>
        {
            new Genre { Id = 1, GenreName = "Musical" },
            new Genre { Id = 2, GenreName = "Romance" }
        };
        }

        [Fact]
        public async Task Index_NoGenresFound_ReturnsEmptyList()
        {
            // Arrange
            _mockHomeRepo.Setup(repo => repo.Genres()).ReturnsAsync(new List<Genre>());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookDisplayModel>(viewResult.ViewData.Model);
            Assert.Empty(model.Genres);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithASearchForBooksByTerm()
        {
            // Arrange
            _mockHomeRepo.Setup(repo => repo.GetBooks("El Principito", 0)).ReturnsAsync(GetTestBooks1());

            // Act
            var result = await _controller.Index("El Principito");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookDisplayModel>(viewResult.ViewData.Model);
            Assert.Single(model.Books.ToList());
        }

        private List<Book> GetTestBooks1()
        {
            return new List<Book>
        {
            new Book { Id = 1, BookName = "El Principito", GenreId = 1 }
        };
        }

        [Fact]
        public async Task Index_ReturnsOnlyBooksThatStartsWithTheTerm_WhenSearchForBooksByTerm()
        {
            // Arrange
            _mockHomeRepo.Setup(repo => repo.GetBooks("El", 0)).ReturnsAsync(GetTestBooks2());

            // Act
            var result = await _controller.Index("Principito");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookDisplayModel>(viewResult.ViewData.Model);
            Assert.Empty(model.Books.ToList());
        }

        private List<Book> GetTestBooks2()
        {
            return new List<Book>
        {
            new Book { Id = 1, BookName = "El Principito", GenreId = 1 },
            new Book { Id = 2, BookName = "El proceso", GenreId = 1 }
        };
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithASearchForBooksByGenreId()
        {
            // Arrange
            _mockHomeRepo.Setup(repo => repo.GetBooks("", 1)).ReturnsAsync(GetTestBooks());

            // Act
            var result = await _controller.Index("", 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookDisplayModel>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Books.ToList().Count);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithASearchForBooksByTermAndGenreId()
        {
            // Arrange
            _mockHomeRepo.Setup(repo => repo.GetBooks("El Principito", 1)).ReturnsAsync(GetTestBooks1());

            // Act
            var result = await _controller.Index("El Principito", 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookDisplayModel>(viewResult.ViewData.Model);
            Assert.Single(model.Books.ToList());
        }
    }
}
