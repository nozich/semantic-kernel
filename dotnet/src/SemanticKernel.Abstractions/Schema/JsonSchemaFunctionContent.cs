﻿// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace - Using the main namespace
namespace Microsoft.SemanticKernel;
#pragma warning restore IDE0130

/// <summary>
/// A class to describe the content of a response/return type from an SKFunction, in a Json Schema friendly way.
/// </summary>
public sealed class JsonSchemaFunctionContent
{
    /// <summary>
    /// The Json Schema for applivation/json responses.
    /// </summary>
    [JsonPropertyName("application/json")]
    public JsonSchemaResponse JsonResponse { get; } = new JsonSchemaResponse();
}
