using Microsoft.AspNetCore.Authentication.Cookies;
using Models;
using Models.Entities;
using Repositories;
using Repositories.Interface;
using System.Security.Claims;

namespace WebApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			// Register the DBContext and Repository to the DI container
			builder.Services.AddScoped<DBContext, DBContext>();
			builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddScoped<IDishRepository, DishRepository>();
			builder.Services.AddScoped<ITableRepository, TableRepository>();
			builder.Services.AddScoped<IStorageRepository, StorageRepository>();
			builder.Services.AddScoped<IDishCategoryRepository, DishCategoryRepository>();
			builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
			builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
										
			// Add authentication services
			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.AccessDeniedPath = "/Account/RedirectBasedOnRole";
				});

			// Add authorization services
			builder.Services.AddAuthorizationBuilder()
							.AddPolicy("Staff", policy => policy.RequireClaim(ClaimTypes.Role,
								Role.Waitstaff.ToString(),
								Role.Chef.ToString(),
								Role.Accountant.ToString(),
								Role.Manager.ToString()));
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

			// Add authentication and authorization
			app.UseAuthentication();
			app.UseAuthorization();

			// Register the controller routes
			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();

		}
	}
}
