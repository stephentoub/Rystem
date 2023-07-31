﻿namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtesions
    {
        public static bool TryAddFactory<TService, TImplementation>(this IServiceCollection services,
            Func<IServiceProvider, TService> implementationFactory,
           string? name = null,
           ServiceLifetime lifetime = ServiceLifetime.Transient)
           where TService : class
           where TImplementation : class, TService
        {
            var check = true;
            services.AddEngineFactory<TService, TImplementation>(name, lifetime, null, implementationFactory, () => InformThatItsAlreadyInstalled(ref check), null);
            return check;
        }
        public static bool TryAddFactory<TService, TImplementation, TOptions>(this IServiceCollection services,
            Func<IServiceProvider, TService> implementationFactory,
            Action<TOptions> createOptions,
            string? name = null,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class
            where TImplementation : class, TService, IServiceForFactoryWithOptions<TOptions>
            where TOptions : class, new()
        {
            var check = true;
            services.AddFactory<TService, TImplementation, TOptions>(createOptions, name, lifetime, null, implementationFactory, () => InformThatItsAlreadyInstalled(ref check));
            return check;
        }
        public static bool TryAddFactory<TService, TImplementation, TOptions, TBuiltOptions>(this IServiceCollection services,
            Func<IServiceProvider, TService> implementationFactory,
            Action<TOptions> createOptions,
            string? name = null,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class
            where TImplementation : class, TService, IServiceForFactoryWithOptions<TBuiltOptions>
            where TOptions : class, IOptionsBuilder<TBuiltOptions>, new()
            where TBuiltOptions : class
        {
            var check = true;
            services.AddFactory<TService, TImplementation, TOptions, TBuiltOptions>(createOptions, name, lifetime, null, implementationFactory, () => InformThatItsAlreadyInstalled(ref check));
            return check;
        }

        public static async Task<bool> TryAddFactoryAsync<TService, TImplementation, TOptions, TBuiltOptions>(this IServiceCollection services,
            Func<IServiceProvider, TService> implementationFactory,
            Action<TOptions> createOptions,
            string? name = null,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class
            where TImplementation : class, TService, IServiceForFactoryWithOptions<TBuiltOptions>
            where TOptions : class, IOptionsBuilderAsync<TBuiltOptions>, new()
            where TBuiltOptions : class
        {
            var check = true;
            await services
                .AddFactoryAsync<TService, TImplementation, TOptions, TBuiltOptions>(createOptions, name, lifetime, null, implementationFactory, () => InformThatItsAlreadyInstalled(ref check));
            return check;
        }
    }
}
