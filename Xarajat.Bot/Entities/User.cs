using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Xarajat.Bot.Entities;

//[Table("users")]
public class User
{
   // [Key]
    public int Id { get; set; }

    [Required]
    public long ChatId { get; set; }
    public int Step { get; set; }

    [Required]
    //[Column(TypeName = "nvarchar(50)")]
    public string? Name { get; set; }

    //[Column(TypeName = "nvarchar(50)")]
    public string? UserName { get; set; }

   // [Column(TypeName = "nvarchar(20)")]
    public string? Phone { get; set; }

    public DateTime CreatedDate { get; set; }
    public int? RoomId { get; set; }

    [ForeignKey("RoomId")]
    public virtual Room? Room { get; set; }
    public bool IsAdmin { get; set; }
    public virtual List<Outlay>? Outlays { get; set; }

    [NotMapped]
    public string? Fullname => UserName ?? Name;
} 