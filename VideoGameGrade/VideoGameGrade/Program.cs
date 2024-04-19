<<<<<<< Updated upstream
=======
using Microsoft.EntityFrameworkCore;
using VideoGameGrade.Pages;

>>>>>>> Stashed changes
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
<<<<<<< Updated upstream
=======
builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid")); //SendGrid configuration
builder.Services.AddDbContext<VideoGameGrade.Classes.AppDbContext>
    (options =>
     
    builder.Configuration.GetConnectionString
    ("DefaultConnection")
    );
>>>>>>> Stashed changes

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
