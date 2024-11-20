using BookShoppingAutomatedTest.Pages;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingAutomatedTest.Tests
{
    public class LoginTests
    {
        private IWebDriver driver;
        LoginPage loginPage;

        [SetUp]
        public void Setup()
        {
            loginPage = new LoginPage(driver);
            driver = loginPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            loginPage.Visit("http://localhost:5232/");
        }


        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        // Codigo 001
        [Test]
        public void LoginWithValidCredentials_LogIntoAccount()
        {
            loginPage.Login("admin@gmail.com", "Admin@123");
            Assert.IsTrue(loginPage.IsLoggedIn());
        }

        // Codigo 002
        [Test]
        public void LoginWithWrongPassword_DisplayInvalidLoginAttempt()
        {
            loginPage.Login("admin@gmail.com", "password");
            Assert.IsTrue(loginPage.IsInvalidLoginAttempt());
        }


        // Codigo 004
        [Test]
        public void LogitnWithWrongEmail_DisplayInvalidLoginAttempt()
        {
            loginPage.Login("ycambro@gmail.com", "gtaf1234");
            Assert.IsTrue(loginPage.IsInvalidLoginAttempt());
        }

        // Codigo 005
        [Test]
        public void ForgotPassword_SendResetPasswordEmail()
        {
            loginPage.ForgotPassword("admin@gmail.com");
            Assert.IsTrue(loginPage.IsResetPasswordConfirmationMsg());
        }
    }
}
