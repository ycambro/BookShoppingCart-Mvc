using BookShoppingAutomatedTest.Pages;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingAutomatedTest.Tests
{
    public class GenreTests
    {
        private IWebDriver driver;
        GenrePage genrePage;


        [SetUp]
        public void SetUp()
        {
            genrePage = new GenrePage(driver);
            driver = genrePage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            genrePage.Visit("http://localhost:5232/");

            genrePage.EnsureLoggedIn("admin@gmail.com", "Admin@123");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        // Codigo 049
        [Test]
        public void AddNewGenre_DisplayedInList()
        {
            genrePage.ManageGenre();
            genrePage.AddGenre("Drama");
            Boolean isGenreInputEmpty = genrePage.IsGenereNameEmpty();


            Assert.IsTrue(isGenreInputEmpty && genrePage.IsGenreAdded("Drama"));
        }

    }
}
