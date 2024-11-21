using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V128.Runtime;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BookShoppingAutomatedTest.Pages
{
    public class CartPage : Base
    {
        By AddBtnLocator = By.XPath("/html/body/div[1]/div[2]/table/tbody/tr[2]/td[6]/a[1]");
        
        By ReduceBtnLocator = By.XPath("/html/body/div[1]/div[2]/table/tbody/tr[2]/td[6]/a[2]");

        By CartBtnLocator = By.XPath("//*[@id=\"navbarColor01\"]/ul[2]/li[1]/a");

        By TotalPriceItemLocator = By.XPath("/html/body/div[1]/div[2]/table/tbody/tr[2]/td[5]");

        By UnitPriceItemLocator = By.XPath("/html/body/div[1]/div[2]/table/tbody/tr[2]/td[4]");

        By BookInCartLocator = By.XPath("/html/body/div[1]/div[2]/table/tbody/tr[2]");

        By CartIsEmptyH5Locator = By.XPath("/html/body/div[1]/div[2]/h5");

        public CartPage(IWebDriver driver) : base(driver)
        {
        }

        public void EnsureLoggedIn(string username, string password)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(username, password);
        }

        public void ManageCart()
        {
            Click(CartBtnLocator);
        }

        public void AddBookToCart()
        {
            HomePage homePage = new HomePage(driver);
            homePage.SearchBook("Outlander");
            homePage.AddToCart();
        }

        public void AddBook()
        {
            Click(AddBtnLocator);
        }

        public void ReduceBook()
        {
            Click(ReduceBtnLocator);
        }

        public void RemoveBook(int units)
        {
            for (int i = 0; i < units; i++)
            {
                ReduceBook();
            }
        }

        public float GetTotalPrice()
        {
            String totalPrice = GetText(TotalPriceItemLocator);
            return float.Parse(totalPrice);
        }

        public int GetUnits()
        {
            String unitPrice = GetText(UnitPriceItemLocator);
            var match = System.Text.RegularExpressions.Regex.Match(unitPrice, @"X\s*(\d+)");
            return int.Parse(match.Groups[1].Value);
        }

        public bool IsBookAddedAgain(int units)
        {
            if (GetUnits() == units + 1)
            {
                return true;
            }
            return false;
        }

        public bool IsBookReduced(float initialPrice)
        {
            if (GetTotalPrice() == initialPrice)
            {
                return true;
            }
            return false;
        }

        public bool IsBookRemoved()
        {
            return driver.FindElement(CartIsEmptyH5Locator).Displayed;
        }

    }
}
