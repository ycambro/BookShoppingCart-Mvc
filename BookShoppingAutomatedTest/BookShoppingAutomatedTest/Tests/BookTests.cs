using BookShoppingAutomatedTest.Pages;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingAutomatedTest.Tests
{

    
    public class BookTests
    {

        private IWebDriver driver;
        BookPage bookPage;

        [SetUp]
        public void SetUp()
        {
            bookPage = new BookPage(driver);
            driver = bookPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            bookPage.Visit("http://localhost:5232/");

            bookPage.EnsureLoggedIn("admin@gmail.com", "Admin@123");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }


        // Codigo 035
        [Test]
        public void RegisterBookWithValidData_IsRegistered()
        {
            bookPage.ManageBooks();
            bookPage.AddBook("Twilight", "Stephenie Meyer", "Romance", "233");
            Assert.IsTrue(bookPage.IsFormEmpty());
        }

        // Codigo 036
        [Test]
        public void RegisterBookWithEmptyForm_DisplayAllMsg()
        {
            bookPage.ManageBooks();
            bookPage.AddBook("", "", "", "");
            Assert.IsTrue(bookPage.IsAllMsgDisplayed());
        }

        // Codigo 037
        [Test]
        public void RegisterBookWithNoGenre_DisplayGenreMsg()
        {
            bookPage.ManageBooks();
            bookPage.AddBook("The little prince", "Antoine de Saint", "", "123");
            Assert.IsTrue(bookPage.IsGenreFieldRequiredMsgDisplayed());
        }

        // Codigo 038
        [Test]
        public void RegisterBookWithoutBookname_DisplayBooknameMsg()
        {
            bookPage.ManageBooks();
            bookPage.AddBook("", "Isabel Chaves", "Programming", "345");
            Assert.IsTrue(bookPage.IsBookNameFieldRequiredMsgDisplayed());
        }

        // Codigo 039
        [Test]
        public void RegisterBookWithoutAuthor_DisplayAuthorMsg()
        {
            bookPage.ManageBooks();
            bookPage.AddBook("Diary of a Wimpy Kid", "", "Thriller", "110");
            Assert.IsTrue(bookPage.IsAuthorFieldRequiredMsgDisplayed());
        }

        // Codigo 040
        [Test]
        public void RegisterBookWithoutPrice_DisplayPriceMsg()
        {
            bookPage.ManageBooks();
            bookPage.AddBook("The Hill", "Isaac Mora", "Thriller", "");
            Assert.IsTrue(bookPage.IsPriceFieldRequiredMsgDisplayed());
        }


        // Codigo 041
        [Test]
        public void RegisterBookWithNegativePrice_DisplayPriceMsg()
        {
            bookPage.ManageBooks();
            bookPage.AddBook("Angels and Demons", "Francisco Paredes", "Thriller", "-235");
            Assert.IsTrue(bookPage.IsNegativePriceMsgDisplayed("Angels and Demons"));
        }

        // Codigo 042
        [Test]
        public void RegisterBookWithExistingBook_DisplayBookExistsMsg()
        {
            bookPage.ManageBooks();
            bookPage.AddBook("Pride and Prejudice", "Jane Austen", "Romance", "13");
            Assert.IsFalse(bookPage.IsBookRepeated("Pride and Prejudice"));
        }

        // Codigo 044
        [Test]
        public void UpdatePriceWithIntegerValueFirstBook_DisplayBookWithnewPrice()
        {
            bookPage.ManageBooks();
            bookPage.EditFirstBook("13", "", "", "");
            Thread.Sleep(2000);
            Assert.IsTrue(bookPage.IsFirstPriceUpdated("13"));
        }

        // Codigo 046
        [Test]
        public void UpdatePriceWithFloatValueFirstBook_DisplayBookWithnewPrice()
        {
            bookPage.ManageBooks();
            bookPage.EditFirstBook("10,99", "", "", "");
            Thread.Sleep(2000);
            Assert.IsTrue(bookPage.IsFirstPriceUpdated("10,99"));
        }

        // Codigo 047
        [Test] 
        public void UpdateAuthor_DisplayBookWithNewAuthor()
        {
            bookPage.ManageBooks();
            bookPage.EditFirstBook("", "", "Andrea Perez", "");
            Thread.Sleep(2000);
            Assert.IsTrue(bookPage.IsFirstBookAuthorUpdated("Andrea Perez"));
        }

        // Codigo 048
        [Test]
        public void UpdateBookName_DisplayBookWithNewName()
        {
            bookPage.ManageBooks();
            bookPage.EditFirstBook("", "Harry potter", "", "");
            Thread.Sleep(2000);
            Assert.IsTrue(bookPage.IsFirstBookNameUpdated("Harry potter"));
        }

        // Codigo 043
        [Test]
        public void DeleteBook_DeletedBookIsNotDisplayed()
        {
            bookPage.ManageBooks();
            int booksBefore = bookPage.CountBook();
            bookPage.DeleteFirstBook();
            Thread.Sleep(2000);
            int booksAfter = bookPage.CountBook();
            Assert.IsTrue(booksBefore > booksAfter);
        }

    }
}
