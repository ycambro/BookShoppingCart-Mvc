using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingAutomatedTest.Pages
{
    public class GenrePage : Base
    {

        By DashboardOptionLocator = By.XPath("//*[@id=\"navbarColor01\"]/ul[1]/li[3]/a");
        By GenreOptionLocator = By.XPath("/html/body/div/div[1]/a[4]");

        By AddMoreBtnLocator = By.XPath("/html/body/div/div[2]/div/a");
        By GenreNameInputLocator = By.Id("GenreName");
        By AddBtnLocator = By.XPath("/html/body/div/div[2]/div/div/form/div[2]/button");

        By BackBtnLocator = By.XPath("/html/body/div/div[2]/div/div/a");

        By ResultsLocator = By.TagName("tr");


        public GenrePage(IWebDriver driver) : base(driver)
        {
        }

        public void EnsureLoggedIn(string username, string password)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(username, password);
        }

        public void ManageGenre()
        {
            Click(DashboardOptionLocator);
            Click(GenreOptionLocator);
        }

        public void AddGenre(String genreName)
        {
            Click(AddMoreBtnLocator);
            Type(genreName, GenreNameInputLocator);
            Click(AddBtnLocator);
        }

        public Boolean IsGenreAdded(String genreName)
        {
            Click(BackBtnLocator);
            Thread.Sleep(2000);
            List<IWebElement> rows = FindElements(ResultsLocator);

            IWebElement FirstCell = rows[rows.Count-1].FindElement(By.TagName("td"));
            String genre = FirstCell.Text;
            if (genre == genreName)
            {
                return true;
            }

            return false;
        }

        public Boolean IsGenereNameEmpty()
        {
            return GetText(GenreNameInputLocator) == "";
        }


    }
}
