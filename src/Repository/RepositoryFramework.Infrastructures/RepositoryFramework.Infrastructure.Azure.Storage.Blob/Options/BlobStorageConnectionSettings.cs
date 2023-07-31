﻿using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;

namespace RepositoryFramework.Infrastructure.Azure.Storage.Blob
{
    public class BlobStorageConnectionSettings
    {
        public Uri? EndpointUri { get; set; }
        public string? ManagedIdentityClientId { get; set; }
        public string? ConnectionString { get; set; }
        public string? ContainerName { get; set; }
        public string? Prefix { get; set; }
        public BlobClientOptions? ClientOptions { get; set; }
        internal Type ModelType { get; set; } = null!;
    }
}
