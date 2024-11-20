using BookShoppingAutomatedTest.Pages;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingAutomatedTest.Tests
{
    public class StockTests
    {
        private IWebDriver driver;
        StockPage stockPage;

        [SetUp]
        public void Setup()
        {
            stockPage = new StockPage(driver);
            driver = stockPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            stockPage.Visit("http://localhost:5232/");

            stockPage.EnsureLoggedIn("admin@gmail.com", "Admin@123");

        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        // Codigo 032
        [Test]
        public void UpdateStockWithEmptyString_DisplayMsg()
        {
            stockPage.ManageStock();
            stockPage.UpdateStock("");
            Assert.IsTrue(stockPage.IsInvalidStockDisplayed());
        }

        // Codigo 033
        [Test]
        public void SearchExistingBook_DisplayedResults()
        {
            stockPage.ManageStock();
            stockPage.SearchStock("The Notebook");
            Assert.IsTrue(stockPage.IsValidResultSearch("The Notebook"));
        }

        // Codigo 034
        [Test]
        public void SearchNonExistingBook_DisplayEmptyResults()
        {
            stockPage.ManageStock();
            stockPage.SearchStock("Narnia");
            Assert.IsTrue(stockPage.IsEmptyTable());
        }


        
    }
}
