using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Serialization;
using TechStore_BE.DataConnection;
using TechStore_BE.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("QLSaling"));
});

builder.Services.Configure<Email>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<EmailService>(); //register EmailService in system DI

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".WebProject.Session";
    options.IdleTimeout = TimeSpan.FromHours(3);
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

// Add JSON Serializer settings
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    });

var app = builder.Build();

// Thêm đoạn này TRƯỚC UseRouting
app.UseCors("AllowOrigin"); // ✅ Bật CORS sớm

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseStaticFiles();


app.UseRouting(); 
app.UseSession();
app.UseAuthorization();

app.MapControllers(); 

app.Run();
