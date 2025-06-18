using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TestApp.Data;
using TestApp.DbContext;
using TestApp.Repositories;
using TestApp.Repositories.Interfaces;
using TestApp.Services;

var builder = WebApplication.CreateBuilder(args);

// 1) Register framework services
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// 2) Register DbContext
builder.Services.AddDbContext<MainContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3) Register application services and repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserReactionsRepositories, UserReactionsRepository>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PasswordService>();

// 4) Configure form options for large file uploads
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 524_288_000; // 500 MB
});

// 5) Configure authentication (Cookies for UI, JWT for APIs)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

builder.Services.AddAuthentication(options =>
{
    // Default scheme for browser/Razor Pages
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    // Default challenge for APIs
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opts =>
{
    opts.LoginPath = "/Login";
    opts.LogoutPath = "/Logout";
    opts.ExpireTimeSpan = TimeSpan.FromMinutes(30);
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = false,
        ValidateLifetime = true,
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// 6) Seed database
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    DatabaseSeeder.Seed(serviceProvider);
}

// 7) Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 8) Map endpoints
app.MapControllers();
app.MapRazorPages();

// 9) Redirect root URL to Login
app.MapGet("/", context =>
{
    context.Response.Redirect("/Login");
    return Task.CompletedTask;
});

app.Run();