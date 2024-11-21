using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingAutomatedTest.Pages
{
    public class HomePage : Base
    {
        By SearchBar = By.XPath("/html/body/div[1]/div[2]/form/div[2]/div/input");

        By SearchBtn = By.XPath("/html/body/div[1]/div[2]/form/div[3]/button");

        By ResetBtn = By.XPath("/html/body/div[1]/div[2]/form/div[3]/a");

        By GenreOptionLocator = By.XPath("/html/body/div[1]/div[2]/form/div[1]/select");

        By BooksContainer = By.XPath("/html/body/div[1]/div[3]");

        By BooksDisplayed = By.XPath("/html/body/div[1]/div[3]/div");

        By AddToCartBtn = By.XPath("/html/body/div[1]/div[3]/div[1]/div/button");

        By LoginH1 = By.XPath("/html/body/div[1]/h1");

        By CartBtn = By.XPath("//*[@id=\"navbarColor01\"]/ul[2]/li[1]/a");

        By AddToCartBtns = By.XPath("/html/body/div[1]/div[3]/div/div/button");

        By CartCount = By.XPath("//*[@id=\"cartCount\"]");

        By OutOfStockSpan = By.XPath("/html/body/div[1]/div[3]/div/div/span");



        public HomePage(IWebDriver driver) : base(driver)
        {
        }

        public void EnsureLoggedIn(string username, string password)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(username, password);
        }

        public void SearchBook(String bookName)
        {
            Type(bookName, SearchBar);
            Click(SearchBtn);
        }

        public void ResetSearch()
        {
            Click(ResetBtn);
        }

        public void SelectGenre(String genre)
        {
            SelectElement select = new SelectElement(driver.FindElement(GenreOptionLocator));
            select.SelectByText(genre);
        }

        public void SearchBookByGenre(String genre)
        {
            SelectGenre(genre);
            Click(SearchBtn);
        }

        public void SearchBookByGenreAndName(String genre, String bookName)
        {
            SelectGenre(genre);
            Type(bookName, SearchBar);
            Click(SearchBtn);
        }

        public bool IsElementPresent()
        {
            return driver.FindElements(BooksDisplayed).Any();
        }

        public bool isTheSearched(String bookName)
        {
            List<IWebElement> books = FindElements(BooksDisplayed);
            foreach (IWebElement book in books)
            {
                if (book.Text.Contains(bookName))
                {
                    return true;
                }
            }
            return false;
        }

        public void AddToCart()
        {
            List<IWebElement> cartBtns = FindElements(AddToCartBtns);
            cartBtns[0].Click();
        }

        public void GoToCart()
        {
            Click(CartBtn);
        }

        public bool IsLoginPage()
        {
            Thread.Sleep(2000);
            return driver.FindElement(LoginH1).Displayed;
        }

        public bool IsCartCountIncremented(int times)
        {
            return driver.FindElement(CartCount).Text == times.ToString();
        }

        public bool IsOutOfStock()
        {
            return driver.FindElement(OutOfStockSpan).Displayed;
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

        

    }
}
