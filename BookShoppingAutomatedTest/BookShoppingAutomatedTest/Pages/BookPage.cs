using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingAutomatedTest.Pages
{
    public class BookPage : Base
    {
        By DashboardOptionLocator = By.XPath("//*[@id=\"navbarColor01\"]/ul[1]/li[3]/a");
        By BooksOptionLocator = By.XPath("/html/body/div/div[1]/a[5]");

        By AddMoreBtnLocator = By.XPath("/html/body/div/div[2]/div/a");
        By BookNameInputLocator = By.Id("BookName");
        By AuthorInputLocator = By.Id("AuthorName");

        By GenreDropdownListLocator = By.Id("GenreId");
        By PriceInputLocator = By.Id("Price");

        By AddBtnLocator = By.XPath("/html/body/div/div[2]/div/div/form/div[6]/button");

        By BooknameFieldRequiredMsgLocator = By.XPath("/html/body/div/div[2]/div/div/form/div[1]/span");
        By AuthornameFieldRequiredMsgLocator = By.XPath("/html/body/div/div[2]/div/div/form/div[2]/span");
        By GenreFieldRequiredMsgLocator = By.XPath("/html/body/div/div[2]/div/div/form/div[3]/span");

        By PriceFieldRequiredMsgLocator = By.XPath("/html/body/div/div[2]/div/div/form/div[4]/span");

        By ResultsLocator = By.TagName("tr");

        By BackBtnLocator = By.XPath("/html/body/div/div[2]/div/div/a");

        By EditFirstBtnLocator = By.XPath("/html/body/div/div[2]/div/table/tbody/tr[2]/td[6]/a[1]");
        By UpdateBtnLocator = By.XPath("/html/body/div/div[2]/div/div/form/div[6]/button");
        By FirtsTableRowLocator = By.XPath("/html/body/div/div[2]/div/table/tbody/tr[2]");

        By FirstDeleteBtnLocator = By.XPath("/html/body/div/div[2]/div/table/tbody/tr[2]/td[6]/a[2]");

        public BookPage(IWebDriver driver) : base(driver)
        {
        }

        public void EnsureLoggedIn(string username, string password)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(username, password);
        }


        public void ManageBooks()
        {
            Click(DashboardOptionLocator);
            Click(BooksOptionLocator);

        }

        public void AddBook(String bookName, String author, String genre, String price  )
        {
            Click(AddMoreBtnLocator);
            Type(bookName, BookNameInputLocator);
            Type(author, AuthorInputLocator);

            IWebElement genreDropdown = FindElement(GenreDropdownListLocator);
            List<IWebElement> genres = genreDropdown.FindElements(By.TagName("option")).ToList();

            for(int i = 0; i < genres.Count; i++)
            {
                if (genres[i].Text == genre)
                {
                    genres[i].Click();
                    break;
                }
            }

            Clear(PriceInputLocator);

            Type(price, PriceInputLocator);
            Click(AddBtnLocator);
        }

        public void EditFirstBook(String newPrice, String newBookName, String author, String genre)
        {
            Click(EditFirstBtnLocator);

            if(newPrice != "")
            {
                Clear(PriceInputLocator);
                Type(newPrice, PriceInputLocator);
            }
            if(newBookName != "")
            {
                Clear(BookNameInputLocator);
                Type(newBookName, BookNameInputLocator);
            }
            if (author != "")
            {
                Clear(AuthorInputLocator);
                Type(author, AuthorInputLocator);
            }
            if (genre != "")
            {
                IWebElement genreDropdown = FindElement(GenreDropdownListLocator);
                List<IWebElement> genres = genreDropdown.FindElements(By.TagName("option")).ToList();

                for (int i = 0; i < genres.Count; i++)
                {
                    if (genres[i].Text == genre)
                    {
                        genres[i].Click();
                        break;
                    }
                }
            }

            Click(UpdateBtnLocator);

        }

        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        public void DeleteFirstBook()
        {
            Click(FirstDeleteBtnLocator);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => IsAlertPresent());
            IAlert alert = driver.SwitchTo().Alert();
            alert.Accept();

        }

        public int CountBook()
        {
            int numberOfRows = FindElements(ResultsLocator).Count;
            return numberOfRows - 1; // -1 because the first row is the header
        }

        public Boolean IsFirstPriceUpdated(String newPrice)
        {
            IWebElement firstRow = FindElement(FirtsTableRowLocator);
            List<IWebElement> cells = firstRow.FindElements(By.TagName("td")).ToList();

            return GetText(cells[4]) == newPrice;
        }


        public Boolean IsFormEmpty()
        {
            Boolean BookNameEmpty = GetText(BookNameInputLocator) == "";
            Boolean AuthorEmpty = GetText(AuthorInputLocator) == "";
            Boolean PriceEmpty = GetText(PriceInputLocator) == "";
            Console.Write(BookNameEmpty);
            Console.Write(AuthorEmpty);
            Console.Write(PriceEmpty);
            return BookNameEmpty && AuthorEmpty && PriceEmpty;
        }

        public Boolean IsBookNameFieldRequiredMsgDisplayed()
        {
            return IsDisplayed(BooknameFieldRequiredMsgLocator);
        }

        public Boolean IsGenreFieldRequiredMsgDisplayed()
        {
            return IsDisplayed(GenreFieldRequiredMsgLocator);
        }

        public Boolean IsAuthorFieldRequiredMsgDisplayed()
        {
            return IsDisplayed(AuthornameFieldRequiredMsgLocator);
        }

        public Boolean IsPriceFieldRequiredMsgDisplayed()
        {
            return IsDisplayed(PriceFieldRequiredMsgLocator);
        }

        public Boolean IsAllMsgDisplayed()
        {
            return IsBookNameFieldRequiredMsgDisplayed() 
                && IsAuthorFieldRequiredMsgDisplayed() 
                && IsGenreFieldRequiredMsgDisplayed() 
                && IsPriceFieldRequiredMsgDisplayed();
        }

        public Boolean IsNegativePriceMsgDisplayed(String book)
        {
            Click(BackBtnLocator);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => FindElements(ResultsLocator).Count > 0);
            List<IWebElement> rows = FindElements(ResultsLocator);
            for (int i = 1; i < rows.Count; i++)
            {
                List<IWebElement> cells = rows[i].FindElements(By.TagName("td")).ToList();


                if (cells.Count > 0)
                {
                    string bookName = cells[1].Text;
                    string priceText = cells[4].Text;

                    if (bookName.Equals(book, StringComparison.OrdinalIgnoreCase))
                    {
                        if (decimal.TryParse(priceText, out decimal price))
                        {
                            if (price < 0)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public Boolean IsBookRepeated(String book)
        {
            Click(BackBtnLocator);
            List<IWebElement> rows = FindElements(ResultsLocator);
            int counter = 0;
            for (int i = 1; i < rows.Count; i++)
            {
                List<IWebElement> cells = rows[i].FindElements(By.TagName("td")).ToList();
                if (cells.Count > 0)
                {
                    string bookName = cells[1].Text;

                    if (bookName.Equals(book, StringComparison.OrdinalIgnoreCase))
                    {
                        counter++;

                        if (counter > 1)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public Boolean IsFirstBookAuthorUpdated(String author)
        {
            IWebElement firstRow = FindElement(FirtsTableRowLocator);
            List<IWebElement> cells = firstRow.FindElements(By.TagName("td")).ToList();
            return GetText(cells[2]) == author;
        }

        public Boolean IsFirstBookNameUpdated(String book)
        {
            IWebElement firstRow = FindElement(FirtsTableRowLocator);
            List<IWebElement> cells = firstRow.FindElements(By.TagName("td")).ToList();
            return GetText(cells[1]) == book;
        }

    }
}
