using BookShoppingAutomatedTest.Pages;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingAutomatedTest.Tests
{
    public class ManageAccountTests
    {
        private IWebDriver driver;
        ManageAccountPage manageAccountPage;

        [SetUp]
        public void Setup()
        {
            manageAccountPage = new ManageAccountPage(driver);
            driver = manageAccountPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            manageAccountPage.Visit("https://localhost:7158/Home/Index");
            manageAccountPage.EnsureLoggedIn("leo@gmail.com", "Leo123.");
        }
        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
        [Test]
        //Codigo 018
        public void AddPhoneNumberCorrectly() 
        {
            manageAccountPage.ManageAccountPhoneNumber("88882222");
            Assert.IsTrue(manageAccountPage.ProfileUpdated());
        }
        [Test]
        //Codigo 021
        public void AddPhoneNumberWithPlusCorrectly() 
        {
            manageAccountPage.ManageAccountPhoneNumber("+50688888888");
            Assert.IsTrue(manageAccountPage.ProfileUpdated());
        }
        [Test]
        //Codigo 022
        public void PhoneNumberWithALetter()
        {
            manageAccountPage.ManageAccountPhoneNumber("84433560a");
            Assert.IsTrue(manageAccountPage.PhoneNumberWithLetter());
        }
        [Test]
        //Codigo 023
        public void SaveNumberSeparatedWithSpaceBetween() 
        {
            manageAccountPage.ManageAccountPhoneNumber("8443 3560");
            Assert.IsTrue(manageAccountPage.ProfileUpdated());
        }
        [Test]
        //Codigo 024
        public void ChangePassword() 
        {
            manageAccountPage.ManageAccountPassword("Leo123.", "Leo1234.");
            Assert.IsTrue(manageAccountPage.ChangePasswordCorrectly());

        }
    }
}
