using Chat.Server.Hubs;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// Add Supabase
var supabaseUrl = "https://urdnxbibssomkutsebpa.supabase.co";
var supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVyZG54Ymlic3NvbWt1dHNlYnBhIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjgyODcwMDQsImV4cCI6MjA4Mzg2MzAwNH0.HDkgllmieLgTmaHwazF3AGCBYcr1Hbqqavlpo4giC6E";

builder.Services.AddScoped<Supabase.Client>(_ => 
    new Supabase.Client(supabaseUrl, supabaseKey));

var app = builder.Build();
app.Urls.Add("http://0.0.0.0:5000");  // 0.0.0.0 = t?t c? interfaces
app.UseRouting();
app.MapHub<ChatHub>("/chathub");

Console.WriteLine("=== Internal Chat Server ===");
Console.WriteLine("Server running at: http://localhost:5000");
Console.WriteLine("Multiple users can connect to chat internally");
Console.WriteLine("Features: Real-time chat, Emoji reactions");
Console.WriteLine("============================");

app.Run();