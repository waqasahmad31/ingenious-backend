using DAS.DataAccess.Helpers;
using Ingenious.Repositories;

namespace Ingenious.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureScopedServices(this IServiceCollection services)
        {
            services.AddScoped<ConnectionStrings>();

            services.AddScoped<DbHelper>();

            services.AddScoped<IAccountRepository, AccountRepository>();
            
            services.AddScoped<IAddressRepository, AddressRepository>();

            services.AddScoped<IEmailRepository, EmailRepository>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<IProductImageRepository, ProductImageRepository>();

            services.AddScoped<IProductVariationRepository, ProductVariationRepository>();

            services.AddScoped<IWishlistRepository, WishlistRepository>();

            services.AddScoped<ICartRepository, CartRepository>();

            services.AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}
