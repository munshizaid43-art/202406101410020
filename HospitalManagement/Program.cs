using HospitalManagement.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services.
builder.Services.AddControllersWithViews();

// Add IMemoryCache service.
builder.Services.AddMemoryCache();

// Add HTTP client factory.
builder.Services.AddHttpClient();

// Add session services.
builder.Services.AddSession();

// Add response caching service.
builder.Services.AddResponseCaching();

var app = builder.Build();

// Enable response caching middleware.
app.UseResponseCaching();

// Enable static files such as CSS.
app.UseStaticFiles();

// Enable routing.
app.UseRouting();

// Enable session.
app.UseSession();

// Enable authorization.
app.UseAuthorization();

// Configure the default MVC route.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Doctor}/{action=Index}/{id?}"
);

// Start the application.
app.Run();