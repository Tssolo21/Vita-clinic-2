using Microsoft.EntityFrameworkCore;
using VitaClinic.WebAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Ensure database is created and tables exist
builder.Services.AddDbContext<VitaClinicDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173") // Vite dev server
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Ensure database is created and tables exist
// Ensure database is created and tables exist
builder.Services.AddDbContext<VitaClinicDbContext>();

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
    app.UseCors("AllowReactApp");
}

// Serve static files from wwwroot
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

// Redirect root to web interface
app.MapGet("/", () => Results.Redirect("/index.html"));

app.MapControllers();

app.Run();
