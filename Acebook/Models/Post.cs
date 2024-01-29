namespace acebook.Models;
using System.ComponentModel.DataAnnotations;

public class Post
{
  [Key]
  public int Id {get; set;}
  public string? Content {get; set;}
  public int UserId {get; set;}
  public User? User {get; set;}

  public DateTime Time {get; set;}


  public Post(string Content, int user_id, User user, DateTime time) {
    this.Content = Content;
    this.UserId = user_id;
    this.User = user;
    this.Time = time;
  }

  public Post() {
    
  }
}