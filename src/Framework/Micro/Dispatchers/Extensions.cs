﻿using Micro.Attributes;
using Micro.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Dispatchers;

public static class Extensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>))
                .WithoutAttribute<DecoratorAttribute>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>))
                .WithoutAttribute<DecoratorAttribute>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>))
                .WithoutAttribute<DecoratorAttribute>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    public static IServiceCollection AddDispatchers(this IServiceCollection services)
        => services
            .AddSingleton<IDispatcher, InMemoryDispatcher>()
            .AddSingleton<ICommandDispatcher, InMemoryCommandDispatcher>()
            .AddSingleton<IEventDispatcher, InMemoryEventDispatcher>()
            .AddSingleton<IQueryDispatcher, InMemoryQueryDispatcher>();
}