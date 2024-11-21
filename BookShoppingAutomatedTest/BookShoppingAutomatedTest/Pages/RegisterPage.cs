using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingAutomatedTest.Pages
{
    public class RegisterPage : Base
    {
        By RegisterOptionLocator = By.XPath("/html/body/nav/div/div/ul[2]/li[2]/a");
        By EmailLocator = By.Id("Input_Email");
        By PasswordLocator = By.Id("Input_Password");
        By ConfirmPasswordLocator = By.Id("Input_ConfirmPassword");
        By RegisterBtnLocator = By.Id("registerSubmit");
        By EmailRequiredMessage = By.XPath("//*[@id=\"registerForm\"]/div[1]/span");
        By AlphanumericPasswordWarning = By.XPath("//*[@id=\"registerForm\"]/div[1]");
        By RegisterConfirmationMessageLocator = By.Id("/html/body/div/h1");
        By RegisterPasswordShortMessageLocator = By.Id("/html/body/div/div[2]/div[1]/form/div[2]/span");
        By PasswordRequiredMessage = By.XPath("//*[@id=\"registerForm\"]/div[2]/span");
        By DifferentPasswordWarning = By.XPath("//*[@id=\"registerForm\"]/div[3]/span");
        By EmailAlreadyTakenMessage = By.XPath("//*[@id=\"registerForm\"]/div[1]/ul/li");
        

        By InvalidLoginAttemptLocator = By.XPath("//*[@id=\"account\"]/div[1]/ul/li");

        public RegisterPage(IWebDriver driver) : base(driver)
        {

        }

        public void Register(string email, string password, string confirmPassword)
        {
            Click(RegisterOptionLocator);

            Type(email, EmailLocator);

            Type(password, PasswordLocator);

            Type(confirmPassword, ConfirmPasswordLocator);

            Click(RegisterBtnLocator);
        }

        public bool IsLoggedIn()
        {
            return IsDisplayed(RegisterConfirmationMessageLocator);
        }

        public bool IsInvalidLoginAttempt()
        {
            return IsDisplayed(InvalidLoginAttemptLocator);
        }

        public bool IsShortPassword()
        {
            return IsDisplayed(RegisterPasswordShortMessageLocator);
        }

        public bool AlphaNumericPassword() 
        {
            return IsDisplayed(AlphanumericPasswordWarning);
        }

        public bool NoPassword() 
        {
            return IsDisplayed(PasswordRequiredMessage);
        }

        public bool DifferentPassword() 
        {
            return IsDisplayed(DifferentPasswordWarning);
        }

        public bool NoEmail() 
        {
            return IsDisplayed(EmailRequiredMessage);
        }

        public bool EmailAlreadyTaken() 
        {
            return IsDisplayed(EmailAlreadyTakenMessage);
        }
    }

}
