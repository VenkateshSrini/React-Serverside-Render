using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Okta.AspNetCore;
using System.Security.Principal;
using Microsoft.Extensions.DependencyInjection;
using Project1.Environment;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddAuthentication();
builder.Services.Configure<CookiePolicyOptions>(options => {
    var utils = new CookieUtils();
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
    options.OnAppendCookie = cookieContext =>
        utils.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
    options.OnDeleteCookie = cookieContext =>
        utils.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
})
.ConfigureApplicationCookie(options => {
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Unspecified;
})
 .AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OktaDefaults.MvcAuthenticationScheme;
})
 .AddCookie()
.AddOktaMvc(new OktaMvcOptions
 {
     OktaDomain = builder.Configuration["Okta:OktaDomain"],
     ClientId = builder.Configuration["Okta:ClientId"],
     ClientSecret = builder.Configuration["Okta:ClientSecret"],
     CallbackPath = "/authorization-code/callback",
     PostLogoutRedirectUri= "/Account/PostLogout"

 });
builder.Services.AddControllersWithViews();


builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/build";
});
var app = builder.Build();
//comment this middleware for OKTA to work
//app.Use(async (context, next) => {
//    var userId = app.Configuration["Impersonation:UserId"];
//    var roles = app.Configuration["Impersonation:Roles"];
//    var identity = new GenericIdentity(userId);
//    var principal = new GenericPrincipal(identity, new string[1] { roles });
//    context.User = principal;
//    await next(context);
//});
// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseSpaStaticFiles();
app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        "default",
        "{controller=Home}/{action=Index}");
    endpoints.MapControllerRoute(
       name: "OktaRedirect", pattern: $"authorization-code/callback",
       defaults: "{controller=Home}/{action=Index}");
    endpoints.MapControllerRoute(
      "counter",
      "/counter",
      new { controller = "Home", action = "Index" });
    endpoints.MapControllerRoute(
      "fetch-data",
      "/fetch-data",
      new { controller = "Home", action = "Index" });
});

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        spa.UseReactDevelopmentServer(npmScript: "start");
    }
});

app.Run();
