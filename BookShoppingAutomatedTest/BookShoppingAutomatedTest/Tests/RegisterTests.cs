using BookShoppingAutomatedTest.Pages;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingAutomatedTest.Tests
{
    public class RegisterTests
    {
        private IWebDriver driver;
        RegisterPage RegisterPage;

        [SetUp]
        public void Setup()
        {
            RegisterPage = new RegisterPage(driver);
            driver = RegisterPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            RegisterPage.Visit("http://localhost:5232/");
        }
        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
        [Test]
        //Codigo 007
        public void PasswordAlphaNumeric() 
        {
            RegisterPage.Register("Leo1@gmail.com", "Leo123", "Leo123");
            Assert.IsTrue(RegisterPage.AlphaNumericPassword());
        }
        [Test]

        //Codigo 008
        public void PasswordWithNoCapitalLetter() 
        {
            RegisterPage.Register("Leo1@gmail.com", "leo123.", "leo123.");
            Assert.IsTrue(RegisterPage.AlphaNumericPassword());
        }
        [Test]

        //Codigo 010
        public void PasswordAllCapitalLetter() 
        {
            RegisterPage.Register("Leo1@gmail.com", "LEO123.", "LEO123.");
            Assert.IsTrue(RegisterPage.AlphaNumericPassword());
        }

        [Test]
        //Codigo 011
        public void PasswordTooShort() 
        {
            RegisterPage.Register("Leo1@gmail.com", "L.13", "L.13");
            Assert.IsFalse(RegisterPage.IsShortPassword());
        }
        [Test]

        //Codigo 012
        public void PasswordRequired() 
        {
            RegisterPage.Register("Leo1@gmail.com", "", "Leo123.");
            Assert.IsTrue(RegisterPage.NoPassword());

        }

        [Test]
        //Codigo 013
        public void NoEmail() 
        {
            RegisterPage.Register("", "Leo123.", "Leo123.");
            Assert.IsTrue(RegisterPage.NoEmail());
        }
        [Test]
        //Codigo 014
        public void DifferentPasswords() 
        {
            RegisterPage.Register("Leo1@gmail.com", "Leo123..", "Leo123.");
            Assert.IsTrue(RegisterPage.DifferentPassword());
        }
        [Test]
        public void EmailAlreadyTaken() 
        {
            RegisterPage.Register("leo@gmail.com", "Leo123.", "Leo123.");
            Assert.IsTrue(RegisterPage.EmailAlreadyTaken());
        }

        [Test]
        public void NoConfirmedPassword() 
        {
            RegisterPage.Register("leo@gmail.com", "Leo123.", "");
            Assert.IsTrue(RegisterPage.DifferentPassword());
        }
        [Test]
        public void NoInputData() 
        {
            RegisterPage.Register("", "", "");
            Assert.IsTrue(RegisterPage.NoEmail());
            Assert.IsTrue(RegisterPage.NoPassword());
        }
    }
}
