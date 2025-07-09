// using Microsoft.AspNetCore.Authentication.Cookies;
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
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebRestaurant.Data;
using WebRestaurant.Services;

var builder = WebApplication.CreateBuilder(args);

// Đọc cấu hình từ appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Cấu hình Kestrel để bind tới 0.0.0.0 và PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "8090"; // Mặc định 8080 nếu không có PORT
builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port)); // Bind tới 0.0.0.0
});

builder.WebHost.UseIIS();

// Thêm DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Data Protection
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/app/DataProtectionKeys"));

// Cấu hình authentication với Cookie, JWT và Google
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("Jwt:Key is not configured in appsettings.json.");
}
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Auth/Login";
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
})
.AddGoogle(googleOptions =>
{
    var googleAuth = builder.Configuration.GetSection("Authentication:Google");
    googleOptions.ClientId = googleAuth["ClientId"];
    googleOptions.ClientSecret = googleAuth["ClientSecret"];
    googleOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<JwtService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebRestaurant API V1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed dữ liệu nếu chưa có
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DataSeeder.SeedData(dbContext);
}

app.Run();
