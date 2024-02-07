namespace Acebook.Test;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

public class LandingPageTests
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
  public void LandingPage_ShowsWelcomeMessage()
  {
    driver.Navigate().GoToUrl("http://127.0.0.1:5287");
    IWebElement greeting = driver.FindElement(By.Id("greeting"));
    Assert.AreEqual("Welcome To Acebook", greeting.GetAttribute("innerHTML"));
  }

  [Test]
  public void LandingPage_ContainsSignInLink()
  {
      driver.Navigate().GoToUrl("http://127.0.0.1:5287");
      IWebElement signInLink = driver.FindElement(By.Id("signin"));
      Assert.NotNull(signInLink);
  }

  [Test]
  public void SignInButton_GoesToSignInPage()
  {
    driver.Navigate().GoToUrl("http://127.0.0.1:5287");
    IWebElement signInButton = driver.FindElement(By.Id("signin"));
    signInButton.Click();
    string currentUrl = driver.Url;

    Assert.That(currentUrl, Is.EqualTo($"http://127.0.0.1:5287/signin"));
  }

  [Test]
  public void SignUpButton_GoesToSignUpPage()
  {
    driver.Navigate().GoToUrl("http://127.0.0.1:5287");
    IWebElement signUpLink = driver.FindElement(By.Id("signup"));
    signUpLink.Click();
    string currentUrl = driver.Url;

    Assert.That(currentUrl, Is.EqualTo($"http://127.0.0.1:5287/signup"));
  }


}