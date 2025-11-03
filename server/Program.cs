using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

builder.Services.AddOpenApi();

var services = builder.Services;
var configuration = builder.Configuration;

services.AddSecurityHeaderPolicies()
    .SetPolicySelector(ctx =>
    {
        if (ctx.HttpContext.Request.Path.StartsWithSegments("/api"))
        {
            return ApiSecurityHeadersDefinitions.GetHeaderPolicyCollection(builder.Environment.IsDevelopment());
        }

        return SecurityHeadersDefinitions.GetHeaderPolicyCollection(
          builder.Environment.IsDevelopment(),
          configuration["MicrosoftEntraID:Instance"]);
    });

services.AddScoped<MsGraphService>();
services.AddScoped<CaeClaimsChallengeService>();

services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
    options.Cookie.Name = "__Host-X-XSRF-TOKEN";
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

services.AddHttpClient();
services.AddOptions();

var scopes = configuration.GetValue<string>("DownstreamApi:Scopes");
string[] initialScopes = scopes!.Split(' ');

services.AddMicrosoftIdentityWebAppAuthentication(configuration, "MicrosoftEntraID")
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddMicrosoftGraph(defaultScopes: initialScopes)
    .AddInMemoryTokenCaches();

// If using downstream APIs and in memory cache, you need to reset the cookie session if the cache is missing
// If you use persistent cache, you do not require this.
// You can also return the 403 with the required scopes, this needs special handling for ajax calls
// The check is only for single scopes
services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme,
    options => options.Events = new RejectSessionCookieWhenAccountNotInCacheEvents(initialScopes));

services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

services.AddRazorPages().AddMvcOptions(options =>
{
    //var policy = new AuthorizationPolicyBuilder()
    //    .RequireAuthenticatedUser()
    //    .Build();
    //options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();

services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true;

    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseSecurityHeaders();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseNoUnauthorizedRedirect("/api");

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapNotFound("/api/{**segment}");

if (app.Environment.IsDevelopment())
{
    var uiDevServer = app.Configuration.GetValue<string>("UiDevServerUrl");
    if (!string.IsNullOrEmpty(uiDevServer))
    {
        app.MapReverseProxy();
    }
}

app.MapFallbackToPage("/_Host");

app.Run();
