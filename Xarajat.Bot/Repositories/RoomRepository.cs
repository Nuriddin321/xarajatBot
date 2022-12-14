using Microsoft.EntityFrameworkCore;
using Xarajat.Bot.Context;
using Xarajat.Bot.Entities;

namespace Xarajat.Bot.Repositories;

public class RoomRepository
{
	private readonly XarajatDbContext _context;

	public RoomRepository(XarajatDbContext context)
	{
		_context = context;
	}

	public async Task<Room?> GetRoomById(int? id)
	{
		return await _context.Rooms.FindAsync(id);
	}
    public async Task<Room?> GetRoomByKey(string key)
    {
        return await _context.Rooms.FirstOrDefaultAsync(r => r.Key == key);
    }

    public async Task AddRoomAsync(Room	room) 
	{
		await _context.Rooms.AddAsync(room);
		await _context.SaveChangesAsync();
	}
	
	public async Task UpdateRoom(Room room)
	{
		_context.Update(room);
		await _context.SaveChangesAsync();
	}

    public async Task DeleteRoom(Room room)
    {
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
    }

}

