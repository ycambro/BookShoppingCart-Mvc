using BookShoppingAutomatedTest.Pages;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingAutomatedTest.Tests
{

    
    public class CartTests
    {

        private IWebDriver driver;
        CartPage cartPage;

        [SetUp]
        public void SetUp()
        {
            cartPage = new CartPage(driver);
            driver = cartPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            cartPage.Visit("http://localhost:5232/");

            cartPage.EnsureLoggedIn("admin@gmail.com", "Admin@123");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }


        // Codigo 073
        [Test]
        public void AddBookToCart_DisplayedInCart()
        {
            cartPage.AddBookToCart();
            cartPage.ManageCart();
            int units = cartPage.GetUnits();
            cartPage.AddBook();

            Assert.IsTrue(cartPage.IsBookAddedAgain(units));
        }

        // Codigo 074
        [Test]
        public void ReduceBookFromCart_DisplayedInCart()
        {
            cartPage.AddBookToCart();
            cartPage.ManageCart();
            float price = cartPage.GetTotalPrice();
            cartPage.AddBook();
            cartPage.ReduceBook();

            Assert.IsTrue(cartPage.IsBookReduced(price));
        }

        // Codigo 075
        [Test]
        public void RemoveBookFromCart_DisplayedInCart()
        {
            cartPage.AddBookToCart();
            cartPage.ManageCart();
            int units = cartPage.GetUnits();
            cartPage.RemoveBook(units);

            Assert.IsTrue(cartPage.IsBookRemoved());
        }
    }
}
