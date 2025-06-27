﻿// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.Google;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.IdentityModel.Tokens;
// using System.Text;
// using WebRestaurant.Data;
// using WebRestaurant.Services;

// var builder = WebApplication.CreateBuilder(args);

// // Thêm dòng này để đọc cấu hình từ appsettings.json
// builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// // Thêm DbContext
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// //// C?u hình EF Core v?i SQL Server
// //builder.Services.AddDbContext<AppDbContext>(options =>
// //    options.UseSqlServer(
// //        builder.Configuration.GetConnectionString("DefaultConnection"),
// //        sqlOptions => sqlOptions.EnableRetryOnFailure()
// //    )
// //);

// // C?u hình authentication v?i Cookie, JWT và Google
// var jwtSettings = builder.Configuration.GetSection("Jwt");
// var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
// })
// .AddCookie(options =>
// {
//     options.LoginPath = "/Auth/Login";
// })
// .AddJwtBearer(options =>
// {
//     options.RequireHttpsMetadata = false;
//     options.SaveToken = true;
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuerSigningKey = true,
//         IssuerSigningKey = new SymmetricSecurityKey(key),
//         ValidateIssuer = true,
//         ValidIssuer = jwtSettings["Issuer"],
//         ValidateAudience = true,
//         ValidAudience = jwtSettings["Audience"],
//         ValidateLifetime = true,
//         ClockSkew = TimeSpan.Zero
//     };
// })
// // ?? Thêm Google Authentication
// .AddGoogle(googleOptions =>
// {
//     var googleAuth = builder.Configuration.GetSection("Authentication:Google");
//     googleOptions.ClientId = googleAuth["ClientId"];
//     googleOptions.ClientSecret = googleAuth["ClientSecret"];
//     googleOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; 
// });

// builder.Services.AddControllersWithViews();
// builder.Services.AddScoped<JwtService>();

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAll", policy =>
//     {
//         policy.AllowAnyOrigin()
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//     });
// });

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     app.UseHsts();
// }

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c =>
//     {
//         c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebRestaurant API V1");
//     });
// }

// app.UseHttpsRedirection();
// app.UseStaticFiles();

// app.UseRouting();
// app.UseCors("AllowAll");
// app.UseAuthentication();
// app.UseAuthorization();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

// // Seed d? li?u n?u ch?a có
// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     DataSeeder.SeedData(dbContext);
// }

// var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
// app.Listen($"http://0.0.0.0:{port}");


// app.Run();


using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebRestaurant.Data;
using WebRestaurant.Services;

var builder = WebApplication.CreateBuilder(args);

// Đọc cấu hình
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Cấu hình server (chỉ dùng IIS khi production)
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.UseKestrel(options =>
    {
        options.ListenAnyIP(5000); // Port development
    });
}

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Tắt authentication tạm thời để test
// builder.Services.AddAuthentication(...);

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Bật middleware logging
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Path}");
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();