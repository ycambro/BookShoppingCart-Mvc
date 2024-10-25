using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BookShoppingCartMvcUI.Controllers;
using BookShoppingCartMvcUI.Repositories;
using BookShoppingCartMvcUI.Models;
using BookShoppingCartMvcUI.Models.DTOs;
using BookShoppingCartMvcUI.Shared;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;

namespace BookShoppingCartMVC.tests.Controllers
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

        [Fact]
        //Prueba para el Index del BookController
        public async Task Index_ReturnsViewResult_WithListOfBooks()
        {
            // Arrange
            var mockBookRepo = new Mock<IBookRepository>();
            var mockGenreRepo = new Mock<IGenreRepository>();
            var mockFileService = new Mock<IFileService>();

            //Llamada del método (GetTestBooks) para la creación de una lista de libros
            mockBookRepo.Setup(repo => repo.GetBooks()).ReturnsAsync(GetTestBooks());
            var controller = new BookController(mockBookRepo.Object, mockGenreRepo.Object, mockFileService.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Book>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        private List<Book> GetTestBooks()
        {
            return new List<Book>
        {
            new Book { Id = 1, BookName = "LibroPrueba1", GenreId = 1 },
            new Book { Id = 2, BookName = "LibroPrueba2", GenreId = 2 }
        };
        }

        [Fact]
        public async Task Index_NoBooksFound_ReturnsEmptyList()
        {
            // Arrange
            _mockBookRepo.Setup(repo => repo.GetBooks()).ReturnsAsync(new List<Book>());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Book>>(viewResult.Model);
            Assert.Empty(model);
        }

        // Pruebas para el método GET: AddBook
        [Fact]
        public async Task AddBook_Get_Returns_ViewResult_With_BookDTO()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new Genre { Id = 1, GenreName = "Ficción" },
                new Genre { Id = 2, GenreName = "Romance" }
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
        [Fact]
        public async Task AddBook_InvalidModel_ReturnsViewWithErrors()
        {
            // Arrange
            var bookDto = new BookDTO();
            _controller.ModelState.AddModelError("Error", "Error en el modelo");
            _mockGenreRepo.Setup(repo => repo.GetGenres()).ReturnsAsync(new List<Genre>());

            // Act
            var result = await _controller.AddBook(bookDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            _mockBookRepo.Verify(repo => repo.AddBook(It.IsAny<Book>()), Times.Never);
        }
        [Fact]
        public async Task AddBook_Post_InvalidData_ShowsErrors()
        {
            // Arrange
            var bookDto = new BookDTO();
            _controller.ModelState.AddModelError("Error", "Error en el modelo");

            // Act
            var result = await _controller.AddBook(bookDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }

        //Pruebas para el método GET: UpdateBook
        [Fact]
        public async Task UpdateBook_Get_Returns_ViewResult_With_BookDTO()
        {
            // Arrange
            var book = new Book
            {
                Id = 1,
                BookName = "PruebaLibro",
                AuthorName = "Yowi",
                GenreId = 1,
                Price = 19.99,
                Image = "image.jpg"
            };
            var genres = new List<Genre>
            {
                new Genre { Id = 1, GenreName = "Ficcion" },
                new Genre { Id = 2, GenreName = "Romance" }
            };
            _mockBookRepo.Setup(repo => repo.GetBookById(1)).ReturnsAsync(book);
            _mockGenreRepo.Setup(repo => repo.GetGenres()).ReturnsAsync(genres);

            // Act
            var result = await _controller.UpdateBook(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<BookDTO>(viewResult.Model);
            Assert.Equal("PruebaLibro", model.BookName);
            Assert.Equal(1, model.GenreId);
        }

        

        [Fact]
        public async Task UpdateBook_InvalidModel_ReturnsViewWithErrors()
        {
            // Arrange
            var bookDto = new BookDTO();
            _controller.ModelState.AddModelError("Error", "Error en el modelo");

            // Act
            var result = await _controller.UpdateBook(bookDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            _mockBookRepo.Verify(repo => repo.UpdateBook(It.IsAny<Book>()), Times.Never);
        }
        [Fact]
        public async Task UpdateBook_Post_InvalidData_ReturnsViewWithModel()
        {
            // Arrange
            var bookDto = new BookDTO();
            _controller.ModelState.AddModelError("Error", "Error");

            // Act
            var result = await _controller.UpdateBook(bookDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }


        // Pruebas para el método GET: DeleteBook
        [Fact]
        public async Task DeleteBook_BookExists_DeletesBookAndRedirects()
        {
            // Arrange
            var book = new Book
            {
                Id = 1,
                BookName = "Libro Prueba",
                Image = "imagen.jpg"
            };
            _mockBookRepo.Setup(repo => repo.GetBookById(1)).ReturnsAsync(book);

            // Act
            var result = await _controller.DeleteBook(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockBookRepo.Verify(repo => repo.DeleteBook(book), Times.Once);
            _mockFileService.Verify(service => service.DeleteFile("imagen.jpg"), Times.Once);
        }
    }
}

