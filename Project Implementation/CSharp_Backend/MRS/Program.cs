using MRS.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using MRS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation; 

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");


builder.Services.AddControllersWithViews();
builder.Services.AddApplicationService();
builder.Services.AddApplicationDbContexts(builder.Configuration);
builder.Services.AddApplicationIdentity();
builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation() 
    .AddMvcLocalization(LanguageViewLocationExpanderFormat.Suffix);

var app = builder.Build();
//app.PrepareDatabase();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Area",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();