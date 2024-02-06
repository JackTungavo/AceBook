namespace acebook.Models;
using System.ComponentModel.DataAnnotations;

public class Message
{
  [Key]
  public int Id {get; set;}
  public string? Content {get; set;}
  public DateTime Time {get; set;}
  public int User1Id {get; set;}
  public int User2Id {get; set;}

  public Message(int ID, string content, DateTime TIME, int user1id, int user2id) {
    this.Id = ID;
    this.Content = content;
    this.Time = TIME;
    this.User1Id = user1id;
    this.User2Id = user2id;
  }

  public Message() {
    
  }
}