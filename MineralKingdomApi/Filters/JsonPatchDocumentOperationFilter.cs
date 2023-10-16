using Microsoft.AspNetCore.JsonPatch;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;

public class JsonPatchDocumentOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var parameter = context.ApiDescription.ActionDescriptor.Parameters
            .FirstOrDefault(p => p.ParameterType == typeof(JsonPatchDocument));

        if (parameter != null)
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json-patch+json"] = new OpenApiMediaType
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(JsonPatchDocument), context.SchemaRepository)
                    }
                }
            };

            // Add a description for JSON Patch
            operation.RequestBody.Description = "JSON Patch document. Use an array of operations to update the resource.";
        }
    }
}
