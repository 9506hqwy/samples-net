using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrators", policy =>
        policy.AddRequirements(new Aaa.Authorization.Requirement.Administrators()));
});

builder.Services.AddSingleton<IAuthorizationHandler, Aaa.Authorization.Handler.Administrators>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!").RequireAuthorization("Administrators");

app.MapPost("/login", async context =>
{
    if (!context.Request.Form.TryGetValue("username", out var username) || string.IsNullOrEmpty(username))
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.Name, username!),
    };

    var identity = new ClaimsIdentity(
        claims,
        CookieAuthenticationDefaults.AuthenticationScheme);

    var principal = new ClaimsPrincipal(identity);
    await context.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal
    ).ConfigureAwait(false);
});

app.MapPost("/logout", async context =>
{
    await context
        .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)
        .ConfigureAwait(false);
}).RequireAuthorization();

app.Run();
