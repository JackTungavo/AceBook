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
  public int PostParentId {get; set;}
  public string UsersLiked {get; set;}
  public int Likes {get; set;}

  public Post(string Content, int user_id, User user, DateTime time, int postParentId, string usersLiked, int likes) {
    this.Content = Content;
    this.UserId = user_id;
    this.User = user;
    this.Time = time;
    this.PostParentId = postParentId;
    this.UsersLiked = usersLiked;
    this.Likes = likes;
  }

  public void LikePost(int userId)
  {
    if (!this.UsersLiked.Contains("'"+userId.ToString()+"'"))  // 'userid'
    {
      this.UsersLiked += $"'{userId.ToString()}'";
      Console.WriteLine($"Post Liked! - POSTID: {this.Id}");
    } 
    else 
    {
      this.UsersLiked = this.UsersLiked.Replace($"'{userId.ToString()}'","");
      Console.WriteLine($"Post Un-Liked! - POSTID: {this.Id}");
    }
  }

  public Post() {
    
  }
}