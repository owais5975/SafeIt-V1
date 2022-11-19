using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using EBMS.API.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SafeIt;
using SafeIt.Entities;
using SafeIt.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddMvc();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddDbContextPool<ApplicationDBContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("cs")));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opt =>
{
    opt.ExpireTimeSpan = TimeSpan.FromDays(1);
});
builder.Services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; });
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: "CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseNotyf();
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
