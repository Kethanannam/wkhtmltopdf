using DinkToPdf.Contracts;
using DinkToPdf;
using System.Runtime.InteropServices;

// 🔹 Load the Unmanaged wkhtmltopdf Library
var context = new CustomAssemblyLoadContext();
string libPath = Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll");
context.LoadUnmanagedLibrary(libPath);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

// Add Swagger/OpenAPI for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply CORS policy
app.UseCors("AllowAll");

// Use HTTPS redirection
app.UseHttpsRedirection();

// Enable Authorization middleware
app.UseAuthorization();
app.UseStaticFiles(); // Ensure static files (CSS, JS) are served correctly

// Map controllers
app.MapControllers();

// Start the application
app.Run();
