namespace Xarajat.Bot.Entities;

public class Room
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Key { get; set; }
    public RoomStatus? Status { get; set; }

    public virtual List<User>? Users { get; set; }
    public virtual List<Outlay>? Outlays { get; set; }
}