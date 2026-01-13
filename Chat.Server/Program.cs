using Chat.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

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
