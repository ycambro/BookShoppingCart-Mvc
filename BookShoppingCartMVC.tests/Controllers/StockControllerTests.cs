using BookShoppingCartMvcUI.Controllers;
using BookShoppingCartMvcUI.Repositories;
using BookShoppingCartMvcUI.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookShoppingCartMvcUI.Models.DTOs;
using Microsoft.AspNetCore.Http;

namespace BookShoppingCartMVC.tests.Controllers
{
    public class StockControllerTests
    {
        private readonly StockController _controller;
        private readonly Mock<IStockRepository> _mockRepo;


        public StockControllerTests()
        {
            _mockRepo = new Mock<IStockRepository>();
            _controller = new StockController(_mockRepo.Object);
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }


        [Fact]
        public async Task Index_ReturnsViewWithStocksList()
        {
            // Arrange
            var searchTerm = "test";
            var stocks = new List<StockDisplayModel>
                {
                    new StockDisplayModel { BookId = 1, Quantity = 10 },
                    new StockDisplayModel { BookId = 2, Quantity = 5 }
                };
            _mockRepo.Setup(repo => repo.GetStocks(searchTerm)).ReturnsAsync(stocks);

            // Act
            var result = await _controller.Index(searchTerm);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<StockDisplayModel>>(viewResult.Model);
            Assert.Equal(2, model.Count());
            _mockRepo.Verify(repo => repo.GetStocks(searchTerm), Times.Once);
        }

        [Fact]
        public async Task Index_ReturnsEmptyListWhenNoStocks()
        {
            _mockRepo.Setup(repo => repo.GetStocks("")).ReturnsAsync(new List<StockDisplayModel>());

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<StockDisplayModel>>(viewResult.Model);
            Assert.Empty(model);
            _mockRepo.Verify(repo => repo.GetStocks(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task MangeStock_ReturnsViewWithStockWhenStockExists()
        {
            // Arrange
            var bookId = 1;
            var stock = new Stock { BookId = bookId, Quantity = 10 };
            _mockRepo.Setup(repo => repo.GetStockByBookId(bookId)).ReturnsAsync(stock);

            // Act
            var result = await _controller.ManangeStock(bookId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<StockDTO>(viewResult.Model);
            Assert.Equal(bookId, model.BookId);
            Assert.Equal(10, model.Quantity);
            _mockRepo.Verify(repo => repo.GetStockByBookId(bookId), Times.Once);
        }

        [Fact]
        public async Task ManageStock_ReturnsViewWithStockWhenStockDoesNotExist()
        {
            // Arrange
            var bookId = 1;
            _mockRepo.Setup(repo => repo.GetStockByBookId(bookId)).ReturnsAsync((Stock)null);

            // Act
            var result = await _controller.ManangeStock(bookId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<StockDTO>(viewResult.Model);
            Assert.Equal(bookId, model.BookId);
            _mockRepo.Verify(repo => repo.GetStockByBookId(bookId), Times.Once);
        }


        [Fact]
        public async Task ManageStock_ReturnsStockWithZeroQuantityWhenStockDoesNotExist()
        {
            // Arrange
            var bookId = 13;
            _mockRepo.Setup(repo => repo.GetStockByBookId(bookId)).ReturnsAsync((Stock)null);

            // Act
            var result = await _controller.ManangeStock(bookId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<StockDTO>(viewResult.Model);
            Assert.Equal(bookId, model.BookId);
            Assert.Equal(0, model.Quantity);
            _mockRepo.Verify(repo => repo.GetStockByBookId(bookId), Times.Once);
        }


        [Fact]
        public async Task MangeStock_ValidModel_UpdatesStockAndRedirects()
        {
            // Arrange
            var stockDto = new StockDTO { BookId = 23, Quantity = 11 };
            _mockRepo.Setup(repo => repo.ManageStock(stockDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ManangeStock(stockDto);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);
            _mockRepo.Verify(repo => repo.ManageStock(stockDto), Times.Once);
        }

        [Fact]
        public async Task ManageStock_ValidModel_UpdatesSuccessMessageInTempData()
        {
            // Arrange
            var stockDto = new StockDTO { BookId = 4, Quantity = 37 };
            _mockRepo.Setup(repo => repo.ManageStock(stockDto)).Returns(Task.CompletedTask);

            // Act
            await _controller.ManangeStock(stockDto);

            // Assert
            Assert.Equal("Stock is updated successfully.", _controller.TempData["successMessage"]);  // Verificar el mensaje de éxito en TempData
        }


        [Fact]
        public async Task ManageStock_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var stockDto = new StockDTO { BookId = 32, Quantity = -15 };  // Invalid stock quantity
            _controller.ModelState.AddModelError("Quantity", "Invalid quantity");

            // Act
            var result = await _controller.ManangeStock(stockDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<StockDTO>(viewResult.Model);
            Assert.Equal(stockDto, model);
            _mockRepo.Verify(repo => repo.ManageStock(It.IsAny<StockDTO>()), Times.Never);
        }

        [Fact]
        public async Task ManageStock_ExceptionThrown_HandlesExceptionAndRedirectsToIndex()
        {
            // Arrange
            var stockDto = new StockDTO { BookId = 25, Quantity = 43 };
            _mockRepo.Setup(repo => repo.ManageStock(stockDto)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.ManangeStock(stockDto);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);
            _mockRepo.Verify(repo => repo.ManageStock(stockDto), Times.Once);
        }

        [Fact]
        public async Task ManageStock_ExceptionThrown_UpdatesErrorMessageInTempData()
        {
            // Arrange
            var stockDto = new StockDTO { BookId = 1, Quantity = 10 };
            _mockRepo.Setup(repo => repo.ManageStock(stockDto)).ThrowsAsync(new Exception("Something went wrong"));

            // Act
            await _controller.ManangeStock(stockDto);

            // Assert
            Assert.Equal("Something went wrong!!", _controller.TempData["errorMessage"]);
        }

    }
}