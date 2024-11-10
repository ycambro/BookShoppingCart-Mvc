using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingAutomatedTest.Pages
{
    public class StockPage : Base
    {

        By DashboardOptionLocator = By.XPath("//*[@id=\"navbarColor01\"]/ul[1]/li[3]/a");
        By StockOptionLocator = By.XPath("/html/body/div/div[1]/a[3]");
        By FirstUpdateStockBtnLocator = By.XPath("/html/body/div/div[2]/div/table/tbody/tr[2]/td[3]/a");

        By QuantityInputLocator = By.Id("Quantity");
        By UpdateStockBtnLocator = By.XPath("/html/body/div/div[2]/div/div/form/div[2]/button");

        By InvalidQuantityMsgLocator = By.XPath("/html/body/div/div[2]/div/div/form/div[1]/span");

        By SearchBookStockInputLocator = By.XPath("/html/body/div/div[2]/div/form/input");
        By FindBtnLocator = By.XPath("/html/body/div/div[2]/div/form/button");
        By ResultSearchLocator = By.XPath("/html/body/div/div[2]/div/table/tbody/tr[2]/td[1]");

        By ResultsLocator = By.TagName("tr");


        public StockPage(IWebDriver driver) : base(driver)
        {
            
        }

        public void EnsureLoggedIn(string username, string password)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(username, password);
        }

        public void ManageStock()
        {
            Click(DashboardOptionLocator);
            Click(StockOptionLocator);
        }

        public void UpdateStock(String quantiyToUpdate)
        {
            Click(FirstUpdateStockBtnLocator);
            Clear(QuantityInputLocator);
            Type(quantiyToUpdate, QuantityInputLocator);
            Click(UpdateStockBtnLocator);
        }

        public Boolean IsInvalidStockDisplayed()
        {
            return IsDisplayed(InvalidQuantityMsgLocator);
        }

        public void SearchStock(String book)
        {
            Type(book, SearchBookStockInputLocator);
            Click(FindBtnLocator);
        }


        public Boolean IsValidResultSearch(String book)
        {
            List<IWebElement> rows = FindElements(ResultsLocator);
            for (int i = 1; i < rows.Count; i++)
            {
                IWebElement FirstCell = rows[i].FindElement(By.TagName("td"));
                String bookName = FirstCell.Text;
                if(!bookName.Contains(book, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }



        public Boolean IsEmptyTable()
        {
            int NumberOfElements = FindElements(ResultsLocator).Count();
            return NumberOfElements == 1;
        }



    }
}
