using System.ComponentModel.DataAnnotations.Schema;

namespace Xarajat.Bot.Entities;

public class Outlay
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public int Cost { get; set; }

    public int? UserId { get; set; }
    public virtual User? User { get; set; }

    public int? RoomId { get; set; }
    public virtual Room? Room { get; set; }

    [NotMapped]
    public string ToReadable => $"{User.Fullname}\n {Cost}, {Description}";
}
