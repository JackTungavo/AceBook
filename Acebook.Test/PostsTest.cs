using OpenQA.Selenium.Internal;

namespace Acebook.Test;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;
using acebook.Models;
using Microsoft.EntityFrameworkCore;


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
        CleanupDatabase();
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
        Assert.That(isFound, Is.EqualTo(true));
    }

    [Test]
    public void WhenClickingOnPostItTakesYouToPostInDetails(){
        IWebElement passwordField = driver.FindElement(By.Name("content"));
        passwordField.SendKeys("Post");

        IWebElement submitButton = driver.FindElement(By.Id("submit_button"));
        submitButton.Click();
        IWebElement commentButton = driver.FindElement(By.CssSelector("a.btn.btn-primary"));
        commentButton.Click();

        string currentUrl = driver.Url;
        int idNum = 0;
        using (var dbContext = new AcebookDbContext())
        {
            foreach (Post post in dbContext.Posts)
            {
                idNum = post.Id;
            }
        }

        Assert.That($"http://127.0.0.1:5287/posts/{idNum}", Is.EqualTo(currentUrl));


        IList <IWebElement> elements = driver.FindElements(By.TagName("p"));
        bool isFound = false;
        foreach(IWebElement e in elements) {
        if (e.Text == "Content: Post")
        {
            isFound = true;
            break;
        }
        }
        Assert.That(isFound, Is.EqualTo(true));
    }

    //Do not turn this into a test
    private void SignInUser()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5287");
            IWebElement signUpButton = driver.FindElement(By.Id("signup"));
            signUpButton.Click();
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
        }
    private void CleanupDatabase()
    {
        using (var dbContext = new AcebookDbContext())
        {
            dbContext.Posts.RemoveRange(dbContext.Posts);
            dbContext.Users.RemoveRange(dbContext.Users);

            dbContext.SaveChanges();
        }
    }
}