using Micro.API.AsyncApi;
using Micro.API.CORS;
using Micro.API.Exceptions;
using Micro.API.Networking;
using Micro.API.Swagger;
using Micro.Auth;
using Micro.Contexts;
using Micro.Dispatchers;
using Micro.HTTP;
using Micro.HTTP.LoadBalancing;
using Micro.HTTP.ServiceDiscovery;
using Micro.Messaging;
using Micro.Messaging.RabbitMQ;
using Micro.Observability.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Framework;

public static class Extensions
{
    public static WebApplicationBuilder AddMicroFramework(this WebApplicationBuilder builder)
    {
        var appOptions = builder.Configuration.GetSection("app").BindOptions<AppOptions>();
        var appInfo = new AppInfo(appOptions.Name, appOptions.Version);
        builder.Services.AddSingleton(appInfo);

        RenderLogo(appOptions);

        builder
            .AddLogging()
            .Services
            .AddErrorHandling()
            .AddHandlers()
            .AddDispatchers()
            .AddContexts()
            .AddMemoryCache()
            .AddHttpContextAccessor()
            .AddMicro(builder.Configuration)
            .AddAuth(builder.Configuration)
            .AddCorsPolicy(builder.Configuration)
            .AddSwaggerDocs(builder.Configuration)
            .AddAsyncApiDocs(builder.Configuration)
            .AddHeadersForwarding(builder.Configuration)
            .AddMessaging(builder.Configuration)
            .AddRabbitMQ(builder.Configuration)
            .AddConsul(builder.Configuration)
            .AddFabio(builder.Configuration)
            .AddLogger(builder.Configuration);

        builder.Services.AddHttpClient();

        return builder;
    }

    public static WebApplication UseMicroFramework(this WebApplication app)
    {
        app
            .UseHeadersForwarding()
            .UseCorsPolicy()
            .UseErrorHandling()
            .UseSwaggerDocs()
            .UseAuthentication()
            .UseContexts()
            .UseRouting()
            .UseAuthorization()
            .UseEndpoints(endpoints => endpoints.MapAsyncApiDocs(app.Configuration));

        return app;
    }

    private static void RenderLogo(AppOptions app)
    {
        if (string.IsNullOrWhiteSpace(app.Name))
        {
            return;
        }

        Console.WriteLine(Figgle.FiggleFonts.Slant.Render($"{app.Name} {app.Version}"));
    }
}