﻿using System.Text.Json.Serialization;

namespace Rystem.PlayFramework
{
    public sealed class ToolNumberProperty : ToolProperty
    {
        private const string DefaultTypeName = "number";
        public ToolNumberProperty()
        {
            Type = DefaultTypeName;
        }
        [JsonPropertyName("multipleOf")]
        public double? MultipleOf { get; set; }
        [JsonPropertyName("minimum")]
        public double? Minimum { get; set; }
        [JsonPropertyName("maximum")]
        public double? Maximum { get; set; }
        [JsonPropertyName("exclusiveMinimum")]
        public bool? ExclusiveMinimum { get; set; }
        [JsonPropertyName("exclusiveMaximum")]
        public bool? ExclusiveMaximum { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
