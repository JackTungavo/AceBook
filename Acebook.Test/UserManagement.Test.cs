using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace Acebook.Tests
{
  public class UserManagement
  {
    ChromeDriver driver;

    [SetUp]
    public void Setup()
    {
      driver = new ChromeDriver();
    }

    [TearDown]
    public void TearDown() {
      driver.Quit();
    }

    [Test]
    public void SignUp_ValidCredentials_RedirectToSignIn()
    {
      driver.Navigate().GoToUrl("http://127.0.0.1:5287/signup");
      // IWebElement signUpButton = driver.FindElement(By.Id("signup"));
      // signUpButton.Click();
      IWebElement nameField = driver.FindElement(By.Id("name"));
      nameField.SendKeys("francine");
      IWebElement emailField = driver.FindElement(By.Id("email"));
      emailField.SendKeys("francine@email.com");
      IWebElement passwordField = driver.FindElement(By.Id("password"));
      passwordField.SendKeys("12345678");
      IWebElement submitButton = driver.FindElement(By.Id("submit"));
      submitButton.Click();
      string currentUrl = driver.Url;
      Assert.AreEqual("http://127.0.0.1:5287/signin", currentUrl);
    }

    [Test]
    public void SignIn_ValidCredentials_RedirectToPosts() {
      driver.Navigate().GoToUrl("http://127.0.0.1:5287/signup");
      // IWebElement signUpButton = driver.FindElement(By.Id("signup"));
      // signUpButton.Click();
      IWebElement nameField = driver.FindElement(By.Id("name"));
      nameField.SendKeys("francine");
      IWebElement emailField = driver.FindElement(By.Id("email"));
      emailField.SendKeys("francine@email.com");
      IWebElement passwordField = driver.FindElement(By.Id("password"));
      passwordField.SendKeys("12345678");
      IWebElement submitButton = driver.FindElement(By.Id("submit"));
      submitButton.Click();

      driver.Navigate().GoToUrl("http://127.0.0.1:5287/signin");
      emailField = driver.FindElement(By.Id("email"));
      emailField.SendKeys("francine@email.com");
      passwordField = driver.FindElement(By.Id("password"));
      passwordField.SendKeys("12345678");
      submitButton = driver.FindElement(By.Id("submit"));
      submitButton.Click();
      string currentUrl = driver.Url;
      Assert.AreEqual("http://127.0.0.1:5287/posts", currentUrl);
    }
  }
}