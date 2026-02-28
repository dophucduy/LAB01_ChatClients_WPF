using Chat.Server.Hubs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Supabase;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel for large file uploads (up to 1.5 GB)
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1_610_612_736; // 1.5 GB
});

builder.Services.AddRazorPages();

builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 1024 * 1024; // 1 MB for SignalR messages
}).AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// Add Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
    });

// Add Supabase
var supabaseUrl = "https://urdnxbibssomkutsebpa.supabase.co";
var supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVyZG54Ymlic3NvbWt1dHNlYnBhIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjgyODcwMDQsImV4cCI6MjA4Mzg2MzAwNH0.HDkgllmieLgTmaHwazF3AGCBYcr1Hbqqavlpo4giC6E";

builder.Services.AddScoped<Supabase.Client>(_ => 
    new Supabase.Client(supabaseUrl, supabaseKey));

var app = builder.Build();

// Ensure uploads directory exists
var uploadsPath = Path.Combine(app.Environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
Directory.CreateDirectory(uploadsPath);

app.Urls.Add("http://0.0.0.0:5000");

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapHub<ChatHub>("/chathub");

// ============ File Upload API ============
app.MapPost("/api/upload", async (HttpContext context) =>
{
    if (!context.User.Identity?.IsAuthenticated ?? true)
        return Results.Unauthorized();

    var form = await context.Request.ReadFormAsync();
    var file = form.Files.FirstOrDefault();
    
    if (file == null || file.Length == 0)
        return Results.BadRequest("No file uploaded");

    // Generate unique filename
    var originalName = file.FileName;
    var uniqueName = $"{Guid.NewGuid()}_{originalName}";
    var filePath = Path.Combine(uploadsPath, uniqueName);

    // Stream file to disk (supports large files)
    await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);
    await file.CopyToAsync(stream);

    var fileUrl = $"/uploads/{uniqueName}";
    
    return Results.Ok(new
    {
        fileName = originalName,
        fileUrl = fileUrl,
        fileSize = file.Length
    });
}).DisableAntiforgery();

// ============ Auth API ============
app.MapPost("/api/auth/login", async (HttpContext context, Supabase.Client supabase) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"].ToString().Trim();
    var password = form["password"].ToString();

    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        return Results.BadRequest("Username and password are required");

    try
    {
        var existingUser = await supabase.From<Chat.Server.Models.User>()
            .Where(u => u.Username == username)
            .Single();

        if (existingUser == null)
            return Results.BadRequest("User not found. Please register first.");

        if (existingUser.Password != password)
            return Results.BadRequest("Invalid password!");

        // Create auth cookie
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("UserId", existingUser.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return Results.Ok(new { message = "Login successful" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Login failed: {ex.Message}");
    }
}).DisableAntiforgery();

app.MapPost("/api/auth/register", async (HttpContext context, Supabase.Client supabase) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"].ToString().Trim();
    var password = form["password"].ToString();

    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        return Results.BadRequest("Username and password are required");

    try
    {
        var existingUser = await supabase.From<Chat.Server.Models.User>()
            .Where(u => u.Username == username)
            .Single();

        if (existingUser != null)
            return Results.BadRequest("Username already exists!");

        var newUser = new Chat.Server.Models.User
        {
            Username = username,
            Password = password,
            Created_At = DateTime.Now,
            Is_Online = true
        };

        var result = await supabase.From<Chat.Server.Models.User>().Insert(newUser);
        var insertedUser = result.Models.First();

        // Auto-login after registration
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("UserId", insertedUser.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return Results.Ok(new { message = "Registration successful" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Registration failed: {ex.Message}");
    }
}).DisableAntiforgery();

app.MapGet("/api/auth/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/Login");
});

// ============ File Download API ============
app.MapGet("/api/download/{fileName}", async (string fileName, HttpContext context) =>
{
    var filePath = Path.Combine(uploadsPath, fileName);
    if (!File.Exists(filePath))
        return Results.NotFound("File not found");

    // Extract original filename (remove GUID prefix)
    var originalName = fileName;
    var underscoreIndex = fileName.IndexOf('_');
    if (underscoreIndex > 0 && Guid.TryParse(fileName[..underscoreIndex], out _))
    {
        originalName = fileName[(underscoreIndex + 1)..];
    }

    var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);
    return Results.File(stream, "application/octet-stream", originalName);
});

Console.WriteLine("=== Internal Chat Server (Web App) ===");
Console.WriteLine("Server running at: http://localhost:5000");
Console.WriteLine("Web Chat: http://localhost:5000/Chat");
Console.WriteLine("Login:    http://localhost:5000/Login");
Console.WriteLine("Features: Real-time chat, Emoji, File sharing, Image preview");
Console.WriteLine("=======================================");

app.Run();