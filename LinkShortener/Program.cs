using LinkShortener.Configuration;
using LinkShortener.Services;
using LinkShortener.Stores;
using LinkShortener.Validators;
using Microsoft.Extensions.Internal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<ITokenValidator, TokenValidator>();
builder.Services.AddTransient<ITokenGenerator, TokenGenerator>();
builder.Services.AddTransient<IUriValidator, SchemeValidator>();
builder.Services.AddTransient<ILinkValidator, LinkValidator>();
builder.Services.AddSingleton<IShortenedLinkStore, InMemoryShortenedLinkStore>();
builder.Services.AddScoped<ISystemClock, SystemClock>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var tokenOptions = builder.Configuration.GetSection("TokenOptions");
builder.Services.Configure<TokenOptions>(tokenOptions);

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