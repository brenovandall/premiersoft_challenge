using PremiersoftChallenge.Security;
using Carter;

namespace Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            services.AddCarter();
            services.AddEndpointsApiExplorer();
            services.AddAuthenticatedSwaggerGen();

            return services;
        }

        public static WebApplication UsePresentationServices(this WebApplication app)
        {
            app.MapCarter();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();

            app.UseAuthorization();

            return app;
        }
    }
}
