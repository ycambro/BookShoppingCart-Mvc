using Moq;
using Microsoft.AspNetCore.Mvc;
using BookShoppingCartMvcUI.Controllers;
using BookShoppingCartMvcUI.Repositories;
using BookShoppingCartMvcUI.Models;
using BookShoppingCartMvcUI.Models.DTOs;

namespace BookShoppingCartMvcUI.Tests.Controllers
{
    public class ReportsControllerTests
    {
        private Mock<IReportRepository> _mockReportRepo;
        private ReportsController _controller;

        public ReportsControllerTests()
        {
            _mockReportRepo = new Mock<IReportRepository>();
            _controller = new ReportsController(_mockReportRepo.Object);
        }

        [Fact]
        public async Task TopFiveSellingBooks_ValidDates_ReturnsViewWithTopBooks()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 12, 31);
            _mockReportRepo.Setup(repo => repo.GetTopNSellingBooksByDate(startDate, endDate)).ReturnsAsync(GetTestTopBooks());

            // Act
            var result = await _controller.TopFiveSellingBooks(startDate, endDate);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<TopNSoldBooksVm>(viewResult.Model);

            // Verificar que la lista contiene los tres libros
            Assert.Equal(5, model.TopNSoldBooks.Count());
        }

        private List<TopNSoldBookModel> GetTestTopBooks()
        {
            return new List<TopNSoldBookModel>
            {
                new TopNSoldBookModel("Don Quijote de la Mancha", "Miguel de Cervantes", 100),
                new TopNSoldBookModel("El retrato de Dorian Gray", "Oscar Wilde", 75),
                new TopNSoldBookModel("Ana Karenina", "León Tolstói", 50),
                new TopNSoldBookModel("El Principito", "Antoine de Saint-Exupéry", 25),
                new TopNSoldBookModel("El proceso", "Franz Kafka", 10)
            };
        }


        [Fact]
        public async Task TopFiveSellingBooks_NoBooksFound_ReturnsEmptyList()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 12, 31);
            _mockReportRepo.Setup(repo => repo.GetTopNSellingBooksByDate(startDate, endDate)).ReturnsAsync(new List<TopNSoldBookModel>());

            // Act
            var result = await _controller.TopFiveSellingBooks(startDate, endDate);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<TopNSoldBooksVm>(viewResult.Model);
            Assert.Empty(model.TopNSoldBooks);
        }

        [Fact]
        public async Task TopFiveSellingBooks_EndDateBeforeStartDate_ReturnsEmptyList()
        {
            // Arrange
            var startDate = new DateTime(2024, 12, 31);
            var endDate = new DateTime(2024, 1, 1); // End date before start date
            _mockReportRepo.Setup(repo => repo.GetTopNSellingBooksByDate(endDate, startDate)).ReturnsAsync(GetTestTopBooks());

            // Act
            var result = await _controller.TopFiveSellingBooks(startDate, endDate);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<TopNSoldBooksVm>(viewResult.Model);

            // Verificar que la lista contiene los tres libros
            Assert.Empty(model.TopNSoldBooks);
        }

        [Fact]
        public async Task TopFiveSellingBooks_ValidDatesButNotFiveBooks_ReturnsViewWithTopBooks()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 12, 31);
            _mockReportRepo.Setup(repo => repo.GetTopNSellingBooksByDate(startDate, endDate)).ReturnsAsync(GetTestTopBooks1());

            // Act
            var result = await _controller.TopFiveSellingBooks(startDate, endDate);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<TopNSoldBooksVm>(viewResult.Model);

            // Verificar que la lista contiene los tres libros
            Assert.Equal(3, model.TopNSoldBooks.Count());
        }

        private List<TopNSoldBookModel> GetTestTopBooks1()
        {
            return new List<TopNSoldBookModel>
            {
                new TopNSoldBookModel("Don Quijote de la Mancha", "Miguel de Cervantes", 100),
                new TopNSoldBookModel("El retrato de Dorian Gray", "Oscar Wilde", 75),
                new TopNSoldBookModel("Ana Karenina", "León Tolstói", 50)
            };
        }
    }
}
