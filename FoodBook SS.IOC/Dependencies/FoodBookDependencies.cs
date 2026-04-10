using FoodBook_SS.Application.Interfaces;
using FoodBook_SS.Application.Services;
using FoodBook_SS.Domain.Repository;
using FoodBook_SS.Infrastructure.Adapters;
using FoodBook_SS.Infrastructure.Security;
using FoodBook_SS.Infrastructure.Services;
using FoodBook_SS.Persistence.Base;
using FoodBook_SS.Persistence.Context;
using FoodBook_SS.Persistence.Repositories.Audit;
using FoodBook_SS.Persistence.Repositories.Menu;
using FoodBook_SS.Persistence.Repositories.Order;
using FoodBook_SS.Persistence.Repositories.Payment;
using FoodBook_SS.Persistence.Repositories.Reservation;
using FoodBook_SS.Persistence.Repositories.Restaurant;
using FoodBook_SS.Persistence.Repositories.Review;
using FoodBook_SS.Persistence.Repositories.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FoodBook_SS.IOC
{

    public static class FoodBookDependencies
    {
        public static IServiceCollection AddFoodBookServices(
            this IServiceCollection services,
            IConfiguration config,
            string connectionString)
        {
            services.AddDbContext<FoodBookDbContext>(opt =>
                opt.UseSqlServer(connectionString));

            AddRepositories(services);
            AddServices(services);
            AddSecurity(services);
            return services;
        }

        private static void AddRepositories(IServiceCollection s)
        {
           
            s.AddScoped<IUserRepository, UserRepository>();
            s.AddScoped<IRestaurantRepository, RestaurantRepository>();
            s.AddScoped<IReservationRepository, ReservationRepository>();
            s.AddScoped<IOrderRepository, OrderRepository>();
            s.AddScoped<IPaymentRepository, PaymentRepository>();
            s.AddScoped<IMenuRepository, MenuRepository>();
            s.AddScoped<IReviewRepository, ReviewRepository>();
            s.AddScoped<IAuditRepository, AuditRepository>();
        }

        private static void AddServices(IServiceCollection s)
        {
            s.AddScoped<IUserService, UserService>();
            s.AddScoped<IRestaurantService, RestaurantService>();
            s.AddScoped<IReservationService, ReservationService>();
            s.AddScoped<IOrderService, OrderService>();
            s.AddScoped<IPaymentService, PaymentService>();
            s.AddScoped<IMenuService, MenuService>();
            s.AddScoped<IReviewService, ReviewService>();
            s.AddScoped<IAuditService, AuditService>();
            s.AddScoped<INotificationService, NotificationService>();
            s.AddSingleton<NotificationInbox>();
            s.AddScoped<IPaymentGateway, StripePaymentAdapter>();
            s.AddScoped<INotificationSender, EmailNotificationAdapter>();
        }

        private static void AddSecurity(IServiceCollection s)
        {
            s.AddScoped<IJwtTokenService, JwtTokenService>();
            s.AddScoped<IPasswordHasher, PasswordHasher>();
        }
    }
}
