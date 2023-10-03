﻿using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Rystem.Api;

namespace Microsoft.AspNetCore.Builder
{
    public static class EndpointRouteBuilderRystemExtensions
    {
        public static IEndpointRouteBuilder UseEndpointApi(this IEndpointRouteBuilder builder)
        {
            if (builder is IApplicationBuilder app)
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            foreach (var endpoint in EndpointsManager.Endpoints)
            {
                Generics.WithStatic(
                typeof(EndpointRouteBuilderRystemExtensions),
                nameof(PrivateUseEndpointApi),
                endpoint.Type).Invoke(builder, endpoint);
            }
            return builder;
        }
        private static IEndpointRouteBuilder PrivateUseEndpointApi<T>(this IEndpointRouteBuilder builder, EndpointValue endpointValue)
            where T : class
        {
            var interfaceType = typeof(T);
            foreach (var method in endpointValue.Methods)
            {
                var endpointMethodValue = method.Value;
                List<Func<HttpContext, Task<object>>> retrievers = new();
                var nonPrimitivesCount = method.Value.Parameters.Count(x => !x.IsPrimitive);
                var isPost = nonPrimitivesCount > 0;
                var isMultipart = nonPrimitivesCount > 1;
                foreach (var parameter in method.Value.Parameters)
                {
                    var parameterName = (parameter.Info.GetCustomAttribute(typeof(ApiParameterNameAttribute)) as ApiParameterNameAttribute)?.Name ?? parameter.Name;
                    if (parameter.IsPrimitive)
                    {
                        retrievers.Add(context =>
                        {
                            var query = context.Request.Query[parameterName!].ToString();
                            var task = Task.FromResult((object)query.Cast(parameter.Type));
                            return task;
                        });
                    }
                    else if (isMultipart)
                    {
                        retrievers.Add(context =>
                        {
                            var value = context.Request.Form.First(x => x.Key == parameterName);
                            var body = value.Value.ToString();
                            return Task.FromResult(body.FromJson(parameter.Type)!);
                        });
                    }
                    else
                    {
                        retrievers.Add(async context =>
                        {
                            var value = await context.Request.Body.ConvertToStringAsync();
                            return value.FromJson(parameter.Type)!;
                        });
                    }
                }
                var currentMethod = method.Value.Method;
                endpointMethodValue.EndpointUri = $"api/{(endpointValue.EndpointName ?? interfaceType.Name)}/{(!string.IsNullOrWhiteSpace(endpointValue.FactoryName) ? $"{endpointValue.FactoryName}/" : string.Empty)}{endpointMethodValue?.Name ?? method.Key}";

                if (!isPost)
                {
                    builder
                        .MapGet(endpointMethodValue.EndpointUri, async (HttpContext context, [FromServices] T? service, [FromServices] IFactory<T>? factory) =>
                        {
                            return await ExecuteAsync(context, service, factory);
                        })
                        .AddAuthorization(endpointMethodValue.Policies);
                }
                else
                {
                    builder.MapPost(endpointMethodValue.EndpointUri, async (HttpContext context, [FromServices] T? service, [FromServices] IFactory<T>? factory) =>
                    {
                        return await ExecuteAsync(context, service, factory);
                    })
                    .AddAuthorization(endpointMethodValue.Policies);
                }

                async Task<object> ExecuteAsync(HttpContext context, [FromServices] T? service, [FromServices] IFactory<T>? factory)
                {
                    if (factory != null)
                        service = factory.Create(endpointValue.FactoryName);
                    var result = currentMethod.Invoke(service, await ReadParametersAsync(context));
                    if (result is Task task)
                        await task;
                    if (result is ValueTask valueTask)
                        await valueTask;
                    return ((dynamic)result!).Result;
                }

                async Task<object[]> ReadParametersAsync(HttpContext context)
                {
                    var parameters = new object[retrievers.Count];
                    var counter = 0;
                    foreach (var retriever in retrievers)
                    {
                        parameters[counter] = await retriever.Invoke(context);
                        counter++;
                    }
                    return parameters;
                }
            }
            return builder;
        }
        private static RouteHandlerBuilder AddAuthorization(this RouteHandlerBuilder router, string[]? policies)
        {
            if (policies == null)
                router.AllowAnonymous();
            else if (policies.Length == 0)
            {
                router.RequireAuthorization();
            }
            else
            {
                router.RequireAuthorization(policies);
            }
            return router;
        }
    }
}
