using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
  
        public static IServiceCollection AddAI(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }      
    }
}
