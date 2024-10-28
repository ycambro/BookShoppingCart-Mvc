using Moq;
using Microsoft.AspNetCore.Mvc;
using BookShoppingCartMvcUI.Controllers;
using BookShoppingCartMvcUI.Repositories;
using BookShoppingCartMvcUI.Models;
using BookShoppingCartMvcUI.Models.DTOs;

namespace BookShoppingCartMvcUI.Tests.Controllers
{
    public class UserOrderControllerTests
    {
        private Mock<IUserOrderRepository> _mockUserOrderRepo;
        private UserOrderController _controller;

        public UserOrderControllerTests()
        {
            _mockUserOrderRepo = new Mock<IUserOrderRepository>();
            _controller = new UserOrderController(_mockUserOrderRepo.Object);
        }

        [Fact]
        public async Task Orders_ReturnsViewResult_WithListOfOrders()
        {
            // Arrange
            _mockUserOrderRepo.Setup(repo => repo.UserOrders(false)).ReturnsAsync(GetTestOrders);

            // Act
            var result = await _controller.UserOrders();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Order>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        private List<Order> GetTestOrders()
        {
            return new List<Order>
        {
            new Order { Id = 1, UserId = "id1",OrderStatusId = 1, Name = "María", Email = "mariagomez@gmail.com", MobileNumber = "88882232", Address = "San José Centro", PaymentMethod = "Tarjeta", IsPaid = true },
            new Order { Id = 2, UserId = "id1",OrderStatusId = 1, Name = "María", Email = "mariagomez@gmail.com", MobileNumber = "88882232", Address = "San José Centro", PaymentMethod = "Tarjeta", IsPaid = true }
        };
        }

        [Fact]
        public async Task Orders_NoOrdersFound_ReturnsEmptyList()
        {
            // Arrange
            _mockUserOrderRepo.Setup(repo => repo.UserOrders(false)).ReturnsAsync(new List<Order>());

            // Act
            var result = await _controller.UserOrders();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Order>>(viewResult.ViewData.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task Orders_ThrowsNotLoggedInException_WithNoUserId()
        {
            // Arrange
            _mockUserOrderRepo.Setup(repo => repo.UserOrders(false))
                      .Throws(new Exception("User is not logged-in"));

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _controller.UserOrders());

            // Assert
            Assert.Equal("User is not logged-in", exception.Message);
        }
    }
}
