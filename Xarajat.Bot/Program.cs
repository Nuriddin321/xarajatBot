using Microsoft.EntityFrameworkCore;
using Xarajat.Bot.Repositories;
using Xarajat.Bot.Services;
using Xarajat.Data.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<XarajatDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"));
    options.UseLazyLoadingProxies();

});


builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RoomRepository>(); 
builder.Services.AddScoped<OutlayRepository>();
builder.Services.AddScoped<TelegramBotService>();
builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
