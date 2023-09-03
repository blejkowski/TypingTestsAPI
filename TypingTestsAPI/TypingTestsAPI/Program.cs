using TypingTestsAPI.Helpers;
using TypingTestsAPI.Models;
using TypingTestsAPI.Services;
var builder = WebApplication.CreateBuilder(args);

var origin = "_origin";
builder.Services.AddCors(p => p.AddDefaultPolicy(build =>
{
    build.WithOrigins("http://localhost:5002", "http://localhost:3000")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials();
}));

builder.Services.Configure<UserStoreSettings>(
    builder.Configuration.GetSection("UserStoreSettings")
    );
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SessionManager>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();



builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(86400);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
app.UseSession();

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();