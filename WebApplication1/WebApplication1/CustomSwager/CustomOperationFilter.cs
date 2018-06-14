using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using Newtonsoft.Json;
using DTO;

namespace WebApplication1.CustomSwager
{
    public class CustomOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.HttpMethod == "POST")
            {
                var swagerattributes = context.ApiDescription.ControllerAttributes()
                    .Union(context.ApiDescription.ActionAttributes())
                    .OfType<SwagerApiAttribute>();

                foreach (var attr in swagerattributes)
                {
                    var schema = context.SchemaRegistry.GetOrRegister(attr.ApiRequestType);
                    var provider = (IExamplesProvider)Activator.CreateInstance(attr.ApiRequestType);

                    var parts = schema.Ref.Split('/');
                    var name = parts.Last();

                    var definitionToUpdate = context.SchemaRegistry.Definitions[name];

                    if (definitionToUpdate != null)
                    {
                        definitionToUpdate.Example = ((dynamic)FormatAsJson(provider))["application/json"];
                    }
                }
            }
        }

        private static object FormatAsJson(IExamplesProvider provider)
        {
            var examples = new Dictionary<string, object>
            {
                {
                    "application/json", provider.GetExamples()
                }
            };

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                //ContractResolver = new CamelCasePropertyNamesContractResolver(), 
                Converters = new List<JsonConverter>() { new Newtonsoft.Json.Converters.StringEnumConverter() },
                NullValueHandling = NullValueHandling.Ignore,
                //DefaultValueHandling = DefaultValueHandling.Ignore
            };

            var jsonString = JsonConvert.SerializeObject(examples, settings);

            return JsonConvert.DeserializeObject(jsonString);
        }
    }
}
