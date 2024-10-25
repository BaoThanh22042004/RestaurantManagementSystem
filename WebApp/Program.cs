using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Models;
using Models.Entities;
using Repositories;
using Repositories.Interface;
using System.Security.Claims;
using WebApp.Utilities;

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
            builder.Services.AddScoped<IStorageLogRepository, StorageLogRepository>();
            builder.Services.AddScoped<IFeedBackRepository, FeedBackRepository>();

            builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

			// Register Singleton services
			builder.Services.AddSingleton<UserClaimManager>();
            builder.Services.AddSingleton<InformationManager>();
            builder.Services.AddSingleton<FileUploadManager>();

			// Add authentication services
			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.LoginPath = "/Account/Login";
					options.AccessDeniedPath = "/Account/RedirectBasedOnRole";
					options.LogoutPath = "/Account/Logout";
					options.Events.OnValidatePrincipal = async context =>
					{
						var pendingUsersManager = context.HttpContext.RequestServices.GetRequiredService<UserClaimManager>();
						if (!pendingUsersManager.IsEmpty)
						{
							var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
							if (userId != null && pendingUsersManager.ContainsKey(userId))
							{
								var userClaim = pendingUsersManager.Get(userId);
								if (userClaim != null)
								{
									context.ReplacePrincipal(userClaim);
									context.ShouldRenew = true;
									pendingUsersManager.Remove(userId);
								}
								else
								{
									context.RejectPrincipal();
									await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
								}
							}
						}
					};
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
