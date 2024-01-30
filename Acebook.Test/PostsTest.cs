using OpenQA.Selenium.Internal;

namespace Acebook.Test;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;

public class PostsTests
{
    ChromeDriver driver;

    [SetUp]

    public void Setup()
    {
        driver = new ChromeDriver();
        SignInUser();
    }

    [TearDown]
    public void TearDown() 
    {
    driver.Quit();
    }

    [Test]

    public void IndexPageHasPostInTitle()
    {
        
        string currentUrl = driver.Url;
        Assert.AreEqual("http://127.0.0.1:5287/posts", currentUrl);
        IWebElement title = driver.FindElement(By.Id("title"));
        Assert.AreEqual("Posts", title.GetAttribute("innerHTML"));
    }

    [Test]

    public void WhenUserCreatesNewPostItAppearsOnPage()
    {
        string currentUrl = driver.Url;
        IWebElement emailField = driver.FindElement(By.Name("content"));
        emailField.SendKeys("Harry is sitting on his throne.");
        IWebElement submitButton = driver.FindElement(By.Id("submit_button"));
        submitButton.Click();
        Assert.AreEqual("http://127.0.0.1:5287/posts", currentUrl);
        IWebElement content = driver.FindElement(By.TagName("p"));
        IList < IWebElement > elements = driver.FindElements(By.TagName("p"));
        bool isFound = false;
        foreach(IWebElement e in elements) {
        if (e.Text == "Harry is sitting on his throne.")
        {
            isFound = true;
            break;
        }
        }
        Assert.AreEqual(isFound, true);
    }

    private void SignInUser()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5287/signin");
            IWebElement emailField = driver.FindElement(By.Id("email"));
            emailField.SendKeys("francine@email.com");

            IWebElement passwordField = driver.FindElement(By.Id("password"));
            passwordField.SendKeys("12345678");

            IWebElement submitButton = driver.FindElement(By.Id("submit"));
            submitButton.Click();
        }
}