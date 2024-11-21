using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingAutomatedTest.Pages
{
    public class ManageAccountPage : Base
    {
        By ManageAccountLocator =  By.XPath("//*[@id=\"username\"]");
        By PhoneNumberLocator = By.Id("Input_PhoneNumber");
        By BtnUpdateProfile = By.Id("update-profile-button");
        By ProfileUpdatedMessage = By.XPath("/html/body/div/div[2]/div/div[2]/div[1]");
        By PhoneNumberContainsALetterMessage = By.XPath("//*[@id=\"profile-form\"]/div[2]/span");
        By ManageAccountPasswordLocator = By.XPath("//*[@id=\"change-password\"]");
        By ManageAccountCurrentPasswordLocator = By.Id("Input_OldPassword");
        By ManageAccountNewPasswordLocator = By.Id("Input_NewPassword");
        By ManageAccountRepeatNewPasswordLocator = By.Id("Input_ConfirmPassword");
        By ManageAccountPasswordUpdatedMessage = By.XPath("/html/body/div/div[2]/div/div[2]/div[1]");

        By BtnUpdatePassword = By.XPath("//*[@id=\"change-password-form\"]/button");


        public ManageAccountPage(IWebDriver driver) : base(driver)
        {


        }
        public void EnsureLoggedIn(string username, string password)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(username, password);
        }

        public void ManageAccountPhoneNumber(string phoneNumber) 
        {
            Click(ManageAccountLocator);
            Type(phoneNumber, PhoneNumberLocator);
            Click(BtnUpdateProfile);
        }

        public void ManageAccountPassword(string password, string newPassword) 
        {
            Click(ManageAccountLocator);
            Click(ManageAccountPasswordLocator);
            Type(password, ManageAccountCurrentPasswordLocator);
            Type(newPassword, ManageAccountNewPasswordLocator);
            Type(newPassword, ManageAccountRepeatNewPasswordLocator);
            Click(BtnUpdatePassword);

        }

        public bool ProfileUpdated() 
        {
            return IsDisplayed(ProfileUpdatedMessage);

        }

        public bool PhoneNumberWithLetter() 
        {
            return IsDisplayed(PhoneNumberContainsALetterMessage);
        }

        public bool ChangePasswordCorrectly() 
        {
            return IsDisplayed(ManageAccountPasswordUpdatedMessage);
        }
    }
}
