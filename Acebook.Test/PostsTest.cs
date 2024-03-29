using OpenQA.Selenium.Internal;

namespace Acebook.Test;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;
using acebook.Models;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.Support.UI;



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
        Assert.That(currentUrl, Is.EqualTo("http://127.0.0.1:5287/posts"));
        IWebElement title = driver.FindElement(By.Id("title"));
        Assert.That(title.GetAttribute("innerHTML"), Is.EqualTo("Posts"));
    }

    [Test]

    public void WhenUserCreatesNewPostItAppearsOnPage()
    {
        string currentUrl = driver.Url;
        IWebElement emailField = driver.FindElement(By.Name("content"));
        emailField.SendKeys("Harry is sitting on his throne.");
        IWebElement submitButton = driver.FindElement(By.Id("submit_button"));
        submitButton.Click();
        Assert.That(currentUrl, Is.EqualTo("http://127.0.0.1:5287/posts"));
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
            if (dbContext.Posts != null) {
                foreach (Post post in dbContext.Posts)
                {
                    idNum = post.Id;
                }
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

    [Test]

    public void CommentAppearOnPostPageWhenAddedToPost()
    {  
        // Create a new post.
        IWebElement newPostField = driver.FindElement(By.Name("content"));
        newPostField.SendKeys("Test Post");
        IWebElement submitButton = driver.FindElement(By.Id("submit_button"));
        submitButton.Click();
        // Go to post page.
        IWebElement commentButton = driver.FindElement(By.CssSelector("a.btn.btn-primary"));
        commentButton.Click();
        string currentUrl = driver.Url;
        // Get new post ID to go to correct page.
        AcebookDbContext dbContext = new AcebookDbContext();
        int postId = 0;
        if (dbContext.Posts != null)
        {
            foreach (Post post in dbContext.Posts) {
                postId = post.Id;
            }
        }
        
        Assert.That(currentUrl, Is.EqualTo($"http://127.0.0.1:5287/posts/{postId}"));

        // Add a new comment.
        IWebElement newCommentField = driver.FindElement(By.Name("content"));
        newCommentField.SendKeys("Test Comment");
        IWebElement submitCommentButton = driver.FindElement(By.Id("comment_button"));
        submitCommentButton.Click();
        // Find new comment on page.
        IList <IWebElement> elements = driver.FindElements(By.TagName("p"));
        bool isFound = false;
        foreach(IWebElement e in elements) {
        Console.WriteLine(e.Text);
        if (e.Text == "Test Comment")
        {
            isFound = true;
            break;
        }
        }
        Assert.That(isFound, Is.EqualTo(true));
        
    //  Like a post
    IWebElement likePostButton = driver.FindElement(By.XPath("//input[@name='LikePostButton']"));
    likePostButton.Click();

    // Re-locate the button element to capture the updated text value
    likePostButton = driver.FindElement(By.XPath("//input[@name='LikePostButton']"));

    Assert.That(likePostButton.GetAttribute("value"), Is.EqualTo("Likes: 1"));


    // Like a comment
    IWebElement likeCommentButton = driver.FindElement(By.XPath("//input[@name='LikeCommentButton']"));
    likeCommentButton.Click();

    // Re-locate the button element to capture the updated text value
    likeCommentButton = driver.FindElement(By.XPath("//input[@name='LikeCommentButton']"));

    Assert.That(likeCommentButton.GetAttribute("value"), Is.EqualTo("Likes: 1"));
    }



    //Do not turn this into a test
    private void SignInUser()
        {
            driver.Navigate().GoToUrl("http://127.0.0.1:5287/signup");
            
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
            if (dbContext.Posts != null) {dbContext.Posts.RemoveRange(dbContext.Posts);}
            if (dbContext.Users != null) {dbContext.Users.RemoveRange(dbContext.Users);}
            if (dbContext.Messages != null) {dbContext.Messages.RemoveRange(dbContext.Messages);}
            dbContext.SaveChanges();
        }
    }
}