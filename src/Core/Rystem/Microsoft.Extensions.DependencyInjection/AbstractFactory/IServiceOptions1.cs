﻿namespace Microsoft.Extensions.DependencyInjection
{
    public interface IServiceOptions<out TService> : IServiceOptions
        where TService : class
    {
        Func<TService> Build();
    }
}
