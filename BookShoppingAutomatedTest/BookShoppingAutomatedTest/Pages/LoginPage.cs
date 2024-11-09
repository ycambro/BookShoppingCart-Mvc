using NUnit.Framework.Constraints;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingAutomatedTest.Pages
{
    public class LoginPage : Base
    {
        By LoginOptionLocator = By.XPath("//*[@id=\"navbarColor01\"]/ul[2]/li[3]/a");
        By EmailLocator = By.Id("Input_Email");
        By PasswordLocator = By.Id("Input_Password");
        By LogInSubmitBtnLocator = By.Id("login-submit");

        By UsernameDisplayedLocator = By.XPath("//*[@id=\"username\"]");

        By InvalidLoginAttemptLocator = By.XPath("//*[@id=\"account\"]/div[1]/ul/li");

        By FloatingMessageOnEmailLocator = By.XPath("//*[@id=\"account\"]/div[2]/span");

        By ForgotPasswordLocator = By.Id("forgot-password");
        By ResetPasswordBtnLocator = By.XPath("/html/body/div/div[2]/div/form/button");
        By ResetPasswordConfirmationMsgLocator = By.XPath("/html/body/div/h1");

        public LoginPage(IWebDriver driver) : base(driver)
        {

        }

        public void Login(string email, string password)
        {
            Click(LoginOptionLocator);

            Type(email, EmailLocator);

            Type(password, PasswordLocator);

            Click(LogInSubmitBtnLocator);
        }

        public bool IsLoggedIn()
        {
            return IsDisplayed(UsernameDisplayedLocator);
        }

        public bool IsInvalidLoginAttempt()
        {
            return IsDisplayed(InvalidLoginAttemptLocator);
        }

        public bool IsFloatingMessageOnEmail()
        {
            return IsDisplayed(FloatingMessageOnEmailLocator);
        }

        public void ForgotPassword(string email)
        {
            Click(LoginOptionLocator);

            Click(ForgotPasswordLocator);

            Type(email, EmailLocator);

            Click(ResetPasswordBtnLocator);
        }

        public bool IsResetPasswordConfirmationMsg()
        {
            return IsDisplayed(ResetPasswordConfirmationMsgLocator);
        }
    }
}
