using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// >>>>>> 1. Thêm dịch vụ cho Controllers và CORS <<<<<<
builder.Services.AddControllers(); // Đăng ký dịch vụ cho Controllers

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Cho phép bất kỳ nguồn gốc nào (chỉ dùng cho Dev)
              .AllowAnyHeader()
              .AllowAnyMethod(); // Cho phép POST, GET, v.v.
    });
});
// >>>>>> Kết thúc thêm dịch vụ <<<<<<


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer(); // Cần cho Swagger/OpenAPI
builder.Services.AddSwaggerGen(); // Cần cho Swagger/OpenAPI


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Sử dụng middleware Swagger
    app.UseSwaggerUI(); // Sử dụng middleware SwaggerUI
}

// app.UseHttpsRedirection(); // Tùy chọn, nếu bạn muốn dùng HTTPS. Hiện tại dùng HTTP cho đơn giản.

// >>>>>> 2. Sử dụng CORS middleware <<<<<<
// Đảm bảo UseRouting() (nếu có) đứng trước UseCors()
app.UseCors("AllowAll"); // Áp dụng policy "AllowAll"

// UseAuthentication(); // Nếu có
// UseAuthorization(); // Nếu có


// >>>>>> 3. Map Controllers <<<<<<
app.MapControllers(); // Ánh xạ các Controller dựa trên thuộc tính Route


// Phần WeatherForecast (tùy chọn, có thể xóa nếu không dùng)
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi(); // Cần cho Swagger/OpenAPI nếu giữ lại

// Dòng cuối cùng để chạy ứng dụng
app.Run();

// Định nghĩa Record WeatherForecast (nếu giữ lại endpoint /weatherforecast)
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}