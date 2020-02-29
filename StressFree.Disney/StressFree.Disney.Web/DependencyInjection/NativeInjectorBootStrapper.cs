using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StressFree.Disney.Application;
using StressFree.Disney.Data;
using StressFree.Disney.Entities;

namespace StressFree.Disney.Web.DependencyInjection
{
    public class NativeInjectorBootStrapper
    {
        /// <summary>
        /// Resolver la dependencia de los servicios
        /// </summary>
        /// <param name="services"></param>
        public void RegisterServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSingleton(configuration);

            services.Configure<SettingsModel>(configuration.GetSection("SearchPuzzle"));

            services.AddScoped<IWordApplication, WordApplication>();
            services.AddScoped<IWordData, WordData>();
        }
    }
}
