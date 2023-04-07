using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppIdea.Areas.Identity.Data;
using Microsoft.CodeAnalysis.Options;
using AppIdea.Core.Repository;
using AppIdea.Repositories;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AppIdeaContextConnection") ?? throw new InvalidOperationException("Connection string 'AppIdeaContextConnection' not found.");

builder.Services.AddDbContext<AppIdeaContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<AppIdeaUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppIdeaContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
AddScope();

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

#region Authorization

AddAuthorizationPolicies(builder.Services);

#endregion

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
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();

void AddAuthorizationPolicies(IServiceCollection services)
{
    services.AddAuthorization(options => 
    { 
        /// add a polycy with the authorize name and require claim name is EmloyeeNumber
        options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
        options.AddPolicy("RequireUser", policy => policy.RequireRole("User"));
        options.AddPolicy("RequireManager", policy => policy.RequireRole("Manager"));

    });

   
}

void AddScope()
{
    ///register services 
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

}

