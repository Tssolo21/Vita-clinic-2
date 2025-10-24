using Microsoft.EntityFrameworkCore;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on 0.0.0.0:5000 for Replit
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000);
});

// Ensure database is created and tables exist
builder.Services.AddDbContext<VitaClinicDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Register Email and SMS services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS to allow all origins (Replit proxy environment)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Ensure database is created and tables exist
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<VitaClinicDbContext>();
        context.Database.EnsureCreated(); // Creates database and tables if they don't exist
        Console.WriteLine("Database initialized successfully.");
    }
    catch (Exception ex)
    {
        // Log the error but don't crash the application
        Console.WriteLine($"Database initialization warning: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS for all environments (must be early in pipeline)
app.UseCors("AllowAll");

// Serve static files from wwwroot
app.UseStaticFiles();

// Disable HTTPS redirection for Replit
// app.UseHttpsRedirection();

app.UseAuthorization();

// Redirect root to web interface
app.MapGet("/", () => Results.Redirect("/index.html"));

app.MapControllers();

app.Run();
