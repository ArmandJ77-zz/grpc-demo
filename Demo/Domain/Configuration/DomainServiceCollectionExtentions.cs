using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Configuration
{
    public static class DomainServiceCollectionExtentions
    {
        public static IServiceCollection AddDomain(this IServiceCollection container, IConfiguration config)
        {
            var settings = new DomainSettings();
            config.Bind(settings);

            container
                // Register a Mediatr PipelineBehaviour which checks a Fluent.ValidationContext for validation errors and 
                // throws a ValidationException if validation.failures.Any
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>))
                ;

            return container;
        }
    }
}
