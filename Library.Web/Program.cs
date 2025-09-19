var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<Library.Web.Services.BookService>();
builder.Services.AddScoped<Library.Web.Services.IBookService>(sp =>
{
    // Use named client if needed; here we use default HttpClient
    var http = sp.GetRequiredService<HttpClient>();
    var logger = sp.GetRequiredService<ILogger<Library.Web.Services.BookService>>();
    return new Library.Web.Services.BookService(http, logger);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
