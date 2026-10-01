var builder = WebApplication.CreateBuilder(args);

// Add MVC controllers with views
builder.Services.AddControllersWithViews();

// 1. Session State
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 2. In-Memory Cache (IMemoryCache)
builder.Services.AddMemoryCache();

// 3. Response Caching
builder.Services.AddResponseCaching();

// 4. HTTP Client for async API requests
builder.Services.AddHttpClient();

// 5. HttpContextAccessor (useful for session/cookie in services or layouts)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Order is critical: Session -> ResponseCaching -> Auth
app.UseSession();
app.UseResponseCaching();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Course}/{action=Index}/{id?}");

app.Run();