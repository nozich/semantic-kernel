﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.Models;
using Microsoft.SemanticKernel.Text;

namespace Microsoft.SemanticKernel.TemplateEngine;

/// <summary>
/// Prompt template configuration.
/// </summary>
public class PromptTemplateConfig
{
    /// <summary>
    /// Semantic Kernel template format.
    /// </summary>
    public const string SemanticKernelTemplateFormat = "semantic-kernel";

    /// <summary>
    /// Input parameter for semantic functions.
    /// </summary>
    public class InputParameter
    {
        /// <summary>
        /// Name of the parameter to pass to the function.
        /// e.g. when using "{{$input}}" the name is "input", when using "{{$style}}" the name is "style", etc.
        /// </summary>
        [JsonPropertyName("name")]
        [JsonPropertyOrder(1)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Parameter description for UI apps and planner. Localization is not supported here.
        /// </summary>
        [JsonPropertyName("description")]
        [JsonPropertyOrder(2)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Default value when nothing is provided.
        /// </summary>
        [JsonPropertyName("defaultValue")]
        [JsonPropertyOrder(3)]
        public string DefaultValue { get; set; } = string.Empty;
    }

    /// <summary>
    /// Input configuration (list of all input parameters for a semantic function).
    /// </summary>
    public class InputConfig
    {
        /// <summary>
        /// Gets or sets the list of input parameters.
        /// </summary>
        [JsonPropertyName("parameters")]
        [JsonPropertyOrder(1)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<InputParameter> Parameters { get; set; } = new();
    }

    /// <summary>
    /// Format of the prompt template e.g. f-string, semantic-kernel, handlebars, ...
    /// </summary>
    [JsonPropertyName("template_format")]
    [JsonPropertyOrder(1)]
    public string TemplateFormat { get; set; } = SemanticKernelTemplateFormat;

    /// <summary>
    /// Description
    /// </summary>
    [JsonPropertyName("description")]
    [JsonPropertyOrder(2)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Input configuration (that is, list of all input parameters).
    /// </summary>
    [JsonPropertyName("input")]
    [JsonPropertyOrder(3)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InputConfig Input { get; set; } = new();

    /// <summary>
    /// Model request settings.
    /// </summary>
    [JsonPropertyName("models")]
    [JsonPropertyOrder(4)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<AIRequestSettings> ModelSettings { get; set; } = new();

    /// <summary>
    /// Return the default <see cref="AIRequestSettings"/>
    /// </summary>
    public AIRequestSettings GetDefaultRequestSettings()
    {
        return this.ModelSettings.FirstOrDefault() ?? new AIRequestSettings();
    }

    #region Obsolete
    /// <summary>
    /// Schema - Not currently used.
    /// </summary>
    [JsonPropertyName("schema")]
    [Obsolete("Type property is no longer used. This will be removed in a future release.")]
    public int Schema { get; set; } = 1;

    /// <summary>
    /// Type, such as "completion", "embeddings", etc.
    /// </summary>
    /// <remarks>TODO: use enum</remarks>
    [JsonPropertyName("type")]
    [Obsolete("Type property is no longer used. This will be removed in a future release.")]
    public string Type { get; set; } = "completion";

    /// <summary>
    /// Completion configuration parameters.
    /// </summary>
    [JsonPropertyName("completion")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Obsolete("Completion is no longer no longer supported. Use PromptTemplateConfig.ModelSettings collection instead. This will be removed in a future release.")]
    public AIRequestSettings? Completion
    {
        get { return this.GetDefaultRequestSettings(); }
        set
        {
            if (value is not null)
            {
                this.ModelSettings.Add(value);
            }
        }
    }

    /// <summary>
    /// Default AI services to use.
    /// </summary>
    [JsonPropertyName("default_services")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Obsolete("DefaultServices property is not being used. This will be removed in a future release.")]
    public List<string> DefaultServices { get; set; } = new();
    #endregion

    /// <summary>
    /// Creates a prompt template configuration from JSON.
    /// </summary>
    /// <param name="json">JSON of the prompt template configuration.</param>
    /// <returns>Prompt template configuration.</returns>
    /// <exception cref="ArgumentException">Thrown when the deserialization returns null.</exception>
    public static PromptTemplateConfig FromJson(string json)
    {
        var result = Json.Deserialize<PromptTemplateConfig>(json);
        return result ?? throw new ArgumentException("Unable to deserialize prompt template config from argument. The deserialization returned null.", nameof(json));
    }

    internal static PromptTemplateConfig ToPromptTemplateConfig(PromptFunctionModel semanticFunctionConfig)
    {
        return new PromptTemplateConfig()
        {
            TemplateFormat = semanticFunctionConfig.TemplateFormat,
            Description = semanticFunctionConfig.Description,
            Input = new InputConfig()
            {
                Parameters = semanticFunctionConfig.InputParameters.Select(p => new InputParameter()
                {
                    Name = p.Name,
                    Description = p.Description,
                    DefaultValue = p.DefaultValue
                }).ToList()
            },
            ModelSettings = semanticFunctionConfig.ModelSettings
        };
    }
}
