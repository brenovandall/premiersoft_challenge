using Api.Infrastructure;
using Carter;
using PremiersoftChallenge.Security;

namespace Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            services.AddCarter();
            services.AddEndpointsApiExplorer();
            services.AddAuthenticatedSwaggerGen();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.AddScoped<IdempotencyFilter>();

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
