using Carter;

namespace Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            services.AddCarter();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        public static WebApplication UsePresentationServices(this WebApplication app)
        {
            app.MapCarter();

            app.UseSwagger();
            app.UseSwaggerUI();

            //app.UseAuthorization();

            return app;
        }
    }
}
