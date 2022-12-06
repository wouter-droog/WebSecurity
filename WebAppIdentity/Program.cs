using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebAppIdentity.Data;
using WebAppIdentity.Data.Account;
using WebAppIdentity.Services;
using WebAppIdentity.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;

        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders() // this generate the email confirmation token
    ;

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// When SmtpSetting is requested it will be filled with the values from appsettings.json
builder.Services.Configure<SmtpSetting>(builder.Configuration.GetSection("SMTP"));

builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.AddRazorPages();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();