using Moq;
using Microsoft.AspNetCore.Mvc;
using BookShoppingCartMvcUI.Controllers;
using BookShoppingCartMvcUI.Repositories;
using BookShoppingCartMvcUI.Models;
using BookShoppingCartMvcUI.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShoppingCartMvcUI.Tests.Controllers
{
    public class AdminOperationsControllerTests
    {
        private Mock<IUserOrderRepository> _mockOrderRepo;
        private AdminOperationsController _controller;

        public AdminOperationsControllerTests()
        {
            _mockOrderRepo = new Mock<IUserOrderRepository>();
            _controller = new AdminOperationsController(_mockOrderRepo.Object);
        }

        [Fact]
        public async Task AllOrders_ReturnsViewResult_WithListOfOrders()
        {
            // Arrange
            _mockOrderRepo.Setup(repo => repo.UserOrders(true)).ReturnsAsync(GetTestOrders);

            // Act
            var result = await _controller.AllOrders();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Order>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        private List<Order> GetTestOrders()
        {
            return new List<Order>
        {
            new Order { Id = 1, UserId = "id1",OrderStatusId = 1, Name = "María", Email = "mariagomez@gmail.com", MobileNumber = "88882232", Address = "San José Centro", PaymentMethod = "Tarjeta", IsPaid = true },
            new Order { Id = 2, UserId = "id2",OrderStatusId = 1, Name = "Pedro", Email = "pedrorodriguez@gmail.com", MobileNumber = "67766766", Address = "Alajuela Centro", PaymentMethod = "Tarjeta", IsPaid = true }
        };
        }

        [Fact]
        public async Task AllOrders_ReturnsEmptyOrders_WithNoOrders()
        {
            // Arrange
            _mockOrderRepo.Setup(repo => repo.UserOrders(true)).ReturnsAsync(new List<Order>());

            // Act
            var result = await _controller.AllOrders();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Order>>(viewResult.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task TogglePaymentStatus_ValidOrderId_RedirectsToAllOrders()
        {
            // Arrange
            _mockOrderRepo.Setup(repo => repo.TogglePaymentStatus(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.TogglePaymentStatus(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AllOrders", redirectResult.ActionName);
        }

        [Fact]
        public async Task TogglePaymentStatus_InvalidOrderId_RedirectToAllOrders()
        {
            // Arrange
            _mockOrderRepo.Setup(repo => repo.TogglePaymentStatus(1)).Throws(new InvalidOperationException($"order withi id:1 does not found"));

            // Act
            var result = await _controller.TogglePaymentStatus(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AllOrders", redirectResult.ActionName);
        }

        [Fact]
        public async Task UpdateOrderStatus_ValidOrderId_ReturnsViewResultWithOrderStatusModel()
        {
            // Arrange
            var order = new Order { Id = 1, UserId = "id1", OrderStatusId = 2, Name = "María", Email = "" };
            var orderStatuses = new List<OrderStatus>
            {
                new OrderStatus { Id = 1, StatusName = "Pending" },
                new OrderStatus { Id = 2, StatusName = "Shipped" }
            };
            _mockOrderRepo.Setup(repo => repo.GetOrderById(1)).ReturnsAsync(order);
            _mockOrderRepo.Setup(repo => repo.GetOrderStatuses()).ReturnsAsync(orderStatuses);

            // Act
            var result = await _controller.UpdateOrderStatus(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UpdateOrderStatusModel>(viewResult.Model);
            Assert.Equal(1, model.OrderId);
            Assert.Equal(2, model.OrderStatusId);
        }

        [Fact]
        public async Task UpdateOrderStatus_InvalidOrderId_ReturnsNotFound()
        {
            // Arrange
            _mockOrderRepo.Setup(repo => repo.GetOrderById(1)).ReturnsAsync((Order)null);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _controller.UpdateOrderStatus(1));

            // Assert
            Assert.Equal($"Order with id:1 does not found.", exception.Message);
        }

        [Fact]
        public async Task UpdateOrderStatus_InvalidModel_ReturnsViewWithErrors()
        {
            // Arrange
            var updateModel = new UpdateOrderStatusModel { OrderId = 1, OrderStatusId = 2, OrderStatusList = null };
            _controller.ModelState.AddModelError("Error", "Invalid model");

            // Act
            var result = await _controller.UpdateOrderStatus(updateModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task UpdateOrderStatus_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var orderStatuses = new List<OrderStatus>
            {
                new OrderStatus { Id = 1, StatusName = "Pending" },
                new OrderStatus { Id = 2, StatusName = "Shipped" }
            };

            var orderStatusList = (orderStatuses.Select(orderStatus =>
            {
                return new SelectListItem { Value = orderStatus.Id.ToString(), Text = orderStatus.StatusName, Selected = 2 == orderStatus.Id };
            }));

            var updateModel = new UpdateOrderStatusModel { OrderId = 1, OrderStatusId = 2, OrderStatusList = orderStatusList };
            _controller.ModelState.AddModelError("Error", "Invalid model");

            // Act
            var result = await _controller.UpdateOrderStatus(updateModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }
    }
}
