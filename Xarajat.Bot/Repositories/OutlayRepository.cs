using Microsoft.EntityFrameworkCore;
using Xarajat.Bot.Context;
using Xarajat.Bot.Entities;

namespace Xarajat.Bot.Repositories;

public class OutlayRepository
{
	private readonly XarajatDbContext _context;

	public OutlayRepository(XarajatDbContext context)
	{
		_context = context;
	}

	public async Task AddOutlayAsync(Outlay outlay)
	{
		await _context.Outlays.AddAsync(outlay);
		await _context.SaveChangesAsync();
	}
	
	public async Task UpdateOutlayAsync(Outlay outlay)
	{
		_context.Update(outlay);
		await _context.SaveChangesAsync();
	}

}

