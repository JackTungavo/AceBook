namespace acebook.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

public class User
{
  [Key]
  public int Id {get; set;}
  public string? Name {get; set;}
  public string? Email {get; set;}
  public string? Password {get; set;}
  public ICollection<Post>? Posts {get; set;}

  public string? Bio {get; set;}

  public string? ProfileImage {get; set;}

public User(string name, string email, string password, ICollection<Post> posts, string bio, string profileImage)
{
  this.Name = name;
  this.Email = email;
  this.Password = password;
  this.Posts = posts;
  this.Bio = bio;
  this.ProfileImage = profileImage;
}

public User() {
  
}

}

