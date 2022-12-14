using Microsoft.EntityFrameworkCore;
using Xarajat.Bot.Context;
using Xarajat.Bot.Entities;

namespace Xarajat.Bot.Repositories;

public class UserRepository
{
    private readonly XarajatDbContext _context;

    public UserRepository(XarajatDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByChatId(long chatId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUser(User user)
    {
        _context.Update(user);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteUSer(User user)
    {
        _context.Set<User>().Remove(user);
        await _context.SaveChangesAsync();
    }
} 