using BookShoppingCartMvcUI.Controllers;
using BookShoppingCartMvcUI.Models;
using BookShoppingCartMvcUI.Models.DTOs;
using BookShoppingCartMvcUI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingCartMVC.tests.Controllers
{
    public class CartControllerTests
    {
        private readonly Mock<ICartRepository> _mockCartRepo;
        private readonly CartController _controller;

        public CartControllerTests()
        {
            _mockCartRepo = new Mock<ICartRepository>();
            _controller = new CartController(_mockCartRepo.Object);
        }


        // Pruebas para AddItem
        [Fact]

        //Retorna Ok si recibe el item correctamente
        public async Task AddItem_NoRedirect_ReturnsOkWithCartCount()
        {
            // Arrange
            _mockCartRepo.Setup(repo => repo.AddItem(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(3);

            // Act
            var result = await _controller.AddItem(bookId: 1, qty: 1, redirect: 0);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(3, okResult.Value);
        }
        [Fact]

        // Redirecciona a la vista correcta
        public async Task AddItem_WithRedirect_ReturnsRedirectToActionResult()
        {
            // Arrange
            _mockCartRepo.Setup(repo => repo.AddItem(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(1);

            // Act
            var result = await _controller.AddItem(bookId: 1, qty: 1, redirect: 1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("GetUserCart", redirectResult.ActionName);
        }

        // Probar el añadir libro si no hay un usuario logeado
        [Fact]
        public async Task AddItem_UserNotLoggedIn_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _mockCartRepo.Setup(repo => repo.AddItem(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new UnauthorizedAccessException("No hay un usuario logeado"));

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _controller.AddItem(1, 2));
        }

        // Añadir libro con ID no valido, devuelve excepcion
        [Fact]
        public async Task AddItem_InvalidBookId_ThrowsException()
        {
            // Arrange
            _mockCartRepo.Setup(repo => repo.AddItem(It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new ArgumentException("Invalid book ID"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _controller.AddItem(-1, 1, 0));
        }

        // Pruebas para método RemoveItem 
        [Fact]

        //Remover elemento, redirecciona a la vista correcta
        public async Task RemoveItem_ReturnsRedirectToActionResult()
        {
            // Arrange
            _mockCartRepo.Setup(repo => repo.RemoveItem(It.IsAny<int>())).ReturnsAsync(1);

            // Act
            var result = await _controller.RemoveItem(bookId: 1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("GetUserCart", redirectResult.ActionName);
        }

        [Fact]
        public async Task RemoveItem_NonExistentBookId_ReturnsRedirectWithMessage()
        {
            // Arrange
            _mockCartRepo.Setup(repo => repo.RemoveItem(It.IsAny<int>())).ThrowsAsync(new InvalidOperationException("Libro no encontrado"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.RemoveItem(-1));
        }

        // Prueba para GetUserCart cuando el carrito está disponible
        [Fact]
        public async Task GetUserCart_ReturnsViewResultWithCart()
        {
            // Arrange
            var cart = new ShoppingCart
            {
                CartDetails = new List<CartDetail>
                {
                    new CartDetail { BookId = 1, Quantity = 1 }
                }
            };
            _mockCartRepo.Setup(repo => repo.GetUserCart()).ReturnsAsync(cart);

            // Act
            var result = await _controller.GetUserCart();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(cart, viewResult.Model);
        }

        // Prueba obtener total de elementos en el carrito, devuelve estado OK 

        [Fact]
        public async Task GetTotalItemInCart_ReturnsOkWithCartItemCount()
        {
            // Arrange
            _mockCartRepo.Setup(repo => repo.GetCartItemCount(It.IsAny<string>())).ReturnsAsync(5);

            // Act
            var result = await _controller.GetTotalItemInCart();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(5, okResult.Value);
        }

        [Fact]
        public async Task GetUserCart_NoUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _mockCartRepo.Setup(repo => repo.GetUserCart()).ThrowsAsync(new UnauthorizedAccessException());

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _controller.GetUserCart());
        }

        //Prueba para metodo Checkout, en caso de 

        [Fact]
        public async Task Checkout_FailedPurchase_ReturnsOrderFailureView()
        {
            // Arrange
            var model = new CheckoutModel {Name = "Yosward Ulate", Address = "Belen" };
            _mockCartRepo.Setup(repo => repo.DoCheckout(model)).ReturnsAsync(false);

            // Act
            var result = await _controller.Checkout(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("OrderFailure", redirectResult.ActionName);
        }

        //Checkout, en caso de ser correcto redirige al OrderSuccess()

        [Fact]
        public async Task Checkout_CorrectCheckout_RedirectsToOrderSuccess()
        {
            // Arrange
            var model = new CheckoutModel { Name = "Yurgen Cambronero", Address = "Sarchí" };
            _mockCartRepo.Setup(repo => repo.DoCheckout(model)).ReturnsAsync(true);

            // Act
            var result = await _controller.Checkout(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("OrderSuccess", redirectResult.ActionName);
        }

        // Pruebas para metodo Checkout (GET)
        [Fact]
        public void Checkout_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Checkout();

            // Assert
            Assert.IsType<ViewResult>(result);
        }


        // Pruebas para metodo Checkout (POST)
        [Fact]
        public async Task Checkout_Post_ValidModel_ReturnsRedirectToOrderSuccess()
        {
            // Arrange
            var model = new CheckoutModel();
            _mockCartRepo.Setup(repo => repo.DoCheckout(model)).ReturnsAsync(true);

            // Act
            var result = await _controller.Checkout(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.OrderSuccess), redirectResult.ActionName);
        }

        //Checkout, en caso de tener errores de validacion en el modelo

        [Fact]
        public async Task Checkout_Post_ModelError_ReturnsModelWithValidationErrors()
        {
            // Arrange
            var model = new CheckoutModel();
            _controller.ModelState.AddModelError("Error", "Error de validacion");

            // Act
            var result = await _controller.Checkout(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.False(_controller.ModelState.IsValid);
        }


        //Checkout, si el carrito está vacio, lanza excepcion 
        [Fact]
        public async Task Checkout_Post_CartEmpty_ThrowsInvalidOperationException()
        {
            // Arrange
            var model = new CheckoutModel();
            _mockCartRepo.Setup(repo => repo.DoCheckout(model)).ThrowsAsync(new InvalidOperationException("El carrito está vacio!"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Checkout(model));
        }


        // Prueba para POST fallida, modelo no válido
        [Fact]
        public async Task Checkout_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var model = new CheckoutModel();
            _controller.ModelState.AddModelError("Error", "Model invalido");

            // Act
            var result = await _controller.Checkout(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        // Prueba para OrderSuccess devuelve la vista correcta
        [Fact]
        public void OrderSuccess_ReturnsViewResult()
        {
            // Act
            var result = _controller.OrderSuccess();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        // Prueba para OrderFailure 
        [Fact]
        public void OrderFailure_ReturnsViewResult()
        {
            // Act
            var result = _controller.OrderFailure();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
