using BookShoppingAutomatedTest.Pages;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingAutomatedTest.Tests
{

    
    public class HomeTests
    {

        private IWebDriver driver;
        HomePage homePage;

        [SetUp]
        public void SetUp()
        {
            homePage = new HomePage(driver);
            driver = homePage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            homePage.Visit("http://localhost:5232/");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        // Codigo 051
        [Test]
        public void SearchExistingBookByName_DisplayedInList()
        {
            homePage.SearchBook("Me Before You");

            Assert.IsTrue(homePage.isTheSearched("Me Before You"));
        }

        // Codigo 052
        [Test]
        public void SearchNonExistingBook_DisplayedInList()
        {
            homePage.SearchBook("The Lord of the Rings");

            Assert.IsFalse(homePage.isTheSearched("The Lord of the Rings"));
        }

        // Codigo 053
        [Test]
        public void SearchBookWithJustAWord_DisplayedInList()
        {
            homePage.SearchBook("The");

            Assert.IsTrue(homePage.IsElementPresent());
        }

        // Codigo 054
        [Test]
        public void SearchBookWithJustAnIncompleteWord_DisplayedInList()
        {
            homePage.SearchBook("Me Befo");

            Assert.IsTrue(homePage.isTheSearched("Me Befo"));
        }

        // Codigo 055
        [Test]
        public void SearchBookWithAnEmptyString_DisplayedInList()
        {
            homePage.SearchBook("");

            Assert.IsTrue(homePage.IsElementPresent());
        }


        // Codigo 056
        [Test]
        public void SearchBookByGenreAssigned_DisplayedInList()
        {
            homePage.SearchBookByGenre("Romance");

            Assert.IsTrue(homePage.IsElementPresent());
        }

        // Codigo 057
        [Test]
        public void SearchBookByGenreNotAssigned_DisplayedInList()
        {
            homePage.SearchBookByGenre("Drama");

            Assert.IsFalse(homePage.IsElementPresent());
        }

        // Codigo 058
        [Test]
        public void SearchBookByGenreWithTheDefaultOptionGenre_DisplayedInList()
        {
            homePage.SearchBookByGenre("Genre");

            Assert.IsTrue(homePage.IsElementPresent());
        }

        // Codigo 059
        [Test]
        public void SearchBookByGenreAndName_DisplayedInList()
        {
            homePage.SearchBookByGenreAndName("Romance", "Pride and Prejudice");

            Assert.IsTrue(homePage.isTheSearched("Pride and Prejudice"));
        }

        // Codigo 060
        [Test]
        public void SearchExistingBookByNameButGenreNotAssignedForIt_DisplayedInList()
        {
            homePage.SearchBookByGenreAndName("Romance", "Die Hard");

            Assert.IsFalse(homePage.isTheSearched("Die Hard"));
        }

        // Codigo 061
        [Test]
        public void SearchNonExistingBookByNameAndGenre_DisplayedInList()
        {
            homePage.SearchBookByGenreAndName("Action", "The Lord of the Rings");

            Assert.IsFalse(homePage.isTheSearched("The Lord of the Rings"));
        }

        // Codigo 062
        [Test]
        public void SearchBookByGenreAndNameWithJustAWord_DisplayedInList()
        {
            homePage.SearchBookByGenreAndName("Romance", "Pride");

            Assert.IsTrue(homePage.isTheSearched("Pride"));
        }

        // Codigo 063
        [Test]
        public void SearchBookByGenreAndNameWithJustAnIncompleteWord_DisplayedInList()
        {
            homePage.SearchBookByGenreAndName("Romance", "Prid");

            Assert.IsTrue(homePage.isTheSearched("Prid"));
        }

        // Codigo 064
        [Test]
        public void SearchBookByNameWithAWordThatIsInTheTitleButNoInTheBeggining_DisplayedInList()
        {
            homePage.SearchBook("Before");

            Assert.IsFalse(homePage.isTheSearched("Me Before You"));
        }

        // Codigo 065
        [Test]
        public void SearchBookByNameThenReset_DisplayedInList()
        {
            homePage.SearchBook("Me Before You");
            homePage.ResetSearch(); 

            Assert.IsTrue(homePage.IsElementPresent());
        }

        // Codigo 066
        [Test]
        public void SearchBookByGenreThenReset_DisplayedInList()
        {
            homePage.SearchBookByGenre("Romance");
            homePage.ResetSearch();

            Assert.IsTrue(homePage.IsElementPresent());
        }

        // Codigo 067
        [Test]
        public void SearchBookByGenreAndNameThenReset_DisplayedInList()
        {
            homePage.SearchBookByGenreAndName("Romance", "Pride and Prejudice");
            homePage.ResetSearch();

            Assert.IsTrue(homePage.IsElementPresent());
        }

        // Codigo 068
        [Test]
        public void AddBookToCartWithoutLogin_DisplayedLogginPage()
        {
            homePage.AddToCart();
            Assert.True(homePage.IsLoginPage());
        }

        // Codigo 069
        [Test]
        public void GoToCartWithoutLogin_DisplayedLogginPage()
        {
            homePage.GoToCart();
            Assert.True(homePage.IsLoginPage());
        }

        // Codigo 070
        [Test]
        public void AddBookToCartANonAddedBookWithLogin_DisplayedCartPage()
        {
            homePage.EnsureLoggedIn("admin@gmail.com", "Admin@123");
            homePage.AddToCart();
            Assert.IsTrue(homePage.IsCartCountIncremented(1));
        }

        // Codigo 071
        [Test]
        public void AddBookToCartAnAddedBookWithLogin_DisplayedCartPage()
        {
            homePage.EnsureLoggedIn("admin@gmail.com", "Admin@123");
            homePage.SearchBook("Pride and Prejudice");
            homePage.AddToCart();
            homePage.AddToCart();
            Assert.IsFalse(homePage.IsCartCountIncremented(2));
        }

        // Codigo 072
        [Test]
        public void BookWithNoStock_DisplayOutOfStockSpan()
        {
            homePage.SearchBook("Me Before You");
            Assert.IsTrue(homePage.IsOutOfStock());
        }

    }
}
