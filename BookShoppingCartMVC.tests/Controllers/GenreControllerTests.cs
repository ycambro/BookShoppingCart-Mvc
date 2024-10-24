using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BookShoppingCartMvcUI.Controllers;
using BookShoppingCartMvcUI.Repositories;
using BookShoppingCartMvcUI.Models;
using BookShoppingCartMvcUI.Models.DTOs;
using BookShoppingCartMvcUI.Shared;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BookShoppingCartMVC.tests.Controllers
{
    public class GenreControllerTests
    {
        private readonly GenreController _controller;
        private readonly Mock<IGenreRepository> _mockRepo;


        public GenreControllerTests()
        {
            _mockRepo = new Mock<IGenreRepository>();

            _controller = new GenreController(_mockRepo.Object);

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public async Task DeleteGenre_GenreNotFound_throwInvalidOperationException()
        {
            // Arrange
            int genreId = 1;

            // Se configura el mock para devolver null cuando se intenete obtener el género
            _mockRepo.Setup(repo => repo.GetGenreById(genreId))
                 .ReturnsAsync((Genre)null);

            // Act y Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.DeleteGenre(genreId));
        }

        [Fact]
        public async Task DeleteGenre_GenreExists_ReturnRedirectionToIndex()
        {
            int genreId = 1;
            var genre = new Genre { Id = genreId, GenreName = "Action" };

            _mockRepo.Setup(repo => repo.GetGenreById(genreId))
                 .ReturnsAsync(genre);
            _mockRepo.Setup(repo => repo.DeleteGenre(genre))
                     .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteGenre(genreId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockRepo.Verify(repo => repo.DeleteGenre(genre), Times.Once);
        }

        [Fact]
        public async Task AddGenre_ValidModel_AddsGenreAndRedirects()
        {
            // Arrange
            var genre = new GenreDTO { Id = 3, GenreName = "Horror" };
            _mockRepo.Setup(repo => repo.AddGenre(It.IsAny<Genre>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddGenre(genre);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.AddGenre), redirectResult.ActionName);  
            _mockRepo.Verify(repo => repo.AddGenre(It.IsAny<Genre>()), Times.Once); 
        }

        [Fact]
        public async Task AddGenre_ValidModel_UpdatesSuccessMessageInTempData()
        {
            // Arrange
            var genre = new GenreDTO { Id = 3, GenreName = "Horror" };
            _mockRepo.Setup(repo => repo.AddGenre(It.IsAny<Genre>())).Returns(Task.CompletedTask);

            // Act
            await _controller.AddGenre(genre);

            // Assert
            Assert.Equal("Genre added successfully", _controller.TempData["successMessage"]); // Verifica que TempData se actualizó con el mensaje correcto
        }


        [Fact]
        public async Task AddGenre_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var genreDto = new GenreDTO { Id = 1, GenreName = "Romance" };
            _controller.ModelState.AddModelError("GenreName", "Required");

            // Act
            var result = await _controller.AddGenre(genreDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(genreDto, viewResult.Model);  // Verificar que devuelve el modelo recibido
            _mockRepo.Verify(repo => repo.AddGenre(It.IsAny<Genre>()), Times.Never);
        }

        [Fact]
        public async Task AddGenre_ThrowsException_ReturnsViewWithWithErrorMessage()
        {

            // Arrange
            var genreDto = new GenreDTO { Id = 6, GenreName = "Drama" };
            _mockRepo.Setup(repo => repo.AddGenre(It.IsAny<Genre>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.AddGenre(genreDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(genreDto, viewResult.Model);  
            Assert.Equal("Genre could not added!", _controller.TempData["errorMessage"]);  
            _mockRepo.Verify(repo => repo.AddGenre(It.IsAny<Genre>()), Times.Once);  

        }


        [Fact]
        public async Task AddGenre_EmptyGenreName_ReturnsViewWithModelError()
        {
            // Arrange
            var genreDto = new GenreDTO { Id = 9, GenreName = "" };  // Nombre del género vacío
            _controller.ModelState.AddModelError("GenreName", "Required");

            // Act
            var result = await _controller.AddGenre(genreDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(genreDto, viewResult.Model);  // Verificar que devuelve el modelo recibido
            _mockRepo.Verify(repo => repo.AddGenre(It.IsAny<Genre>()), Times.Never);  // Verificar que no se llama al repositorio
        }


        [Fact]
        public async Task AddGenre_InvalidId_ReturnsRedirectAfterAdding()
        {
            // Arrange
            var genreDto = new GenreDTO { Id = -1, GenreName = "Horror" };  // ID inválido, pero se agrega de todas formas
            _mockRepo.Setup(repo => repo.AddGenre(It.IsAny<Genre>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddGenre(genreDto);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.AddGenre), redirectResult.ActionName);  // Verificar redirección
            _mockRepo.Verify(repo => repo.AddGenre(It.IsAny<Genre>()), Times.Once);  // Verificar que se intentó agregar el género
        }


        [Fact]
        public async Task UpdateGenre_GetGenre_ReturnsViewWithGenre()
        {
            // Arrange
            var genre = new Genre { Id = 1, GenreName = "Action" };
            _mockRepo.Setup(repo => repo.GetGenreById(1)).ReturnsAsync(genre);

            // Act
            var result = await _controller.UpdateGenre(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<GenreDTO>(viewResult.Model);

            Assert.Equal(genre.Id, model.Id);
            Assert.Equal(genre.GenreName, model.GenreName);

        }

        [Fact]
        public async Task UpdateGenre_GenreNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetGenreById(1)).ReturnsAsync((Genre)null);

            // Act
            var result = _controller.UpdateGenre(1);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await result);
        }

        [Fact]
        public async Task UpdateGenre_ValidModel_UpdateGenreAndRedirects()
        {
            var genreDTO = new GenreDTO { Id = 1, GenreName = "Mistery" };
            _mockRepo.Setup(repo => repo.UpdateGenre(It.IsAny<Genre>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateGenre(genreDTO);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockRepo.Verify(repo => repo.UpdateGenre(It.IsAny<Genre>()), Times.Once);
            Assert.Equal("Genre is updated successfully", _controller.TempData["successMessage"]);
        }

        [Fact]
        public async Task UpdateGenre_InvalidModelWithNoGenreName_ReturnsViewWithModel()
        {
            // Arrange
            var genreDTO = new GenreDTO { Id = 2, GenreName = "" }; // Nombre del género vacío
            _controller.ModelState.AddModelError("GenreName", "Required");

            // Act
            var result = await _controller.UpdateGenre(genreDTO);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<GenreDTO>(viewResult.Model);
            Assert.Equal(2, model.Id);
            Assert.Equal("", model.GenreName);
        }


        [Fact]
        public async Task UpdateGenre_ExceptionDuringUpdate_ShowsErrorMessageAndReturnModel()
        {
            // Arrange
            var genreDTO = new GenreDTO { Id = 1, GenreName = "Drama"};
            _mockRepo.Setup(repo => repo.UpdateGenre(It.IsAny<Genre>())).ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _controller.UpdateGenre(genreDTO);

            // Assert
            var viewResulta= Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<GenreDTO>(viewResulta.Model);
            Assert.Equal(1, model.Id);
            Assert.Equal("Drama", model.GenreName);
            Assert.Equal("Genre could not updated!", _controller.TempData["errorMessage"]);
        }

        [Fact]
        public async Task UpdateGenre_ValidModel_CallsRepositoryOnce()
        {
            // Arrange
            var genreDTO = new GenreDTO { Id = 1, GenreName = "Drama" };
            _mockRepo.Setup(repo => repo.UpdateGenre(It.IsAny<Genre>())).Returns(Task.CompletedTask);

            // Act
            await _controller.UpdateGenre(genreDTO);

            // Assert
            _mockRepo.Verify(repo => repo.UpdateGenre(It.IsAny<Genre>()), Times.Once);
        }

        [Fact]
        public async Task Index_ReturnsViewWithGenresList()
        {
            // Arrange
            var genresList = new List<Genre>
                {
                    new Genre { Id = 1, GenreName = "Horror" },
                    new Genre { Id = 2, GenreName = "Science Fiction" }
                };

            _mockRepo.Setup(repo => repo.GetGenres()).ReturnsAsync(genresList);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Genre>>(viewResult.Model);
            Assert.Equal(2, model.Count());  // Verifica que hay dos géneros en la lista.
            Assert.Equal("Horror", model.First().GenreName);  // Verifica que el primer género es "Horror".
        }


    }
}
