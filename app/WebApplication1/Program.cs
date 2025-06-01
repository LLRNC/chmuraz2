using System.Text.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
Console.WriteLine($"[START] {DateTime.Now} | Autor: Michał Lorenc | Port: {port}");

app.MapGet("/", () => Results.Content(@"
  <html>
    <head><meta charset='UTF-8'></head>
    <body>
      <form action='/weather' method='get'>
        <select name='city'>
          <option value='Warsaw'>Warszawa</option>
          <option value='Berlin'>Berlin</option>
          <option value='Paris'>Paryż</option>
        </select>
        <button type='submit'>Pokaż pogodę</button>
      </form>
    </body>
  </html>", "text/html; charset=utf-8"));

app.MapGet("/weather", async (string city) =>
{
    var apiKey = "6923e0a4771e18b38bedb4d88a7b1804";
    using var client = new HttpClient();
    var response = await client.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric");
    using var doc = JsonDocument.Parse(response);
    var temp = doc.RootElement.GetProperty("main").GetProperty("temp").GetDecimal();
    return Results.Text($"Temperatura w {city}: {temp}°C");
});

app.Run($"http://0.0.0.0:{port}");
