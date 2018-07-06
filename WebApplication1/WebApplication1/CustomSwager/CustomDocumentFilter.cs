using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace WebApplication1.CustomSwager
{
    public class CustomDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths.OrderByDescending(x => x.Key);
            swaggerDoc.Paths = paths.ToDictionary(x => x.Key, x => x.Value);

            var test = context.ApiDescriptions.Select(x => x.ActionDescriptor);
            foreach (var item in context.ApiDescriptions)
            {
                var tt = item.ControllerAttributes();
            }
        }

        //private static void AddControllerDescriptions(SwaggerDocument swaggerDoc, IEnumerable<ApiDescription> apiDescriptions)
        //{
            
        //    List<Tag> lst = new List<Tag>();
        //    var desc = apiDescriptions;
        //    ILookup<HttpControllerDescriptor, ApiDescription> apiGroups = desc.ToLookup(api => api.ActionDescriptor.ControllerDescriptor);
        //    foreach (var apiGroup in apiDescriptions)
        //    {
        //        string tagName = apiGroup.Key.ControllerName;
        //        var tag = new Tag { name = tagName };
        //        var apiDoc = doc.GetDocumentation(apiGroup.Key);
        //        if (!String.IsNullOrWhiteSpace(apiDoc))
        //            tag.description = apiDoc;
        //        lst.Add(tag);

        //    }
        //    if (lst.Count() > 0)
        //        swaggerDoc.tags = lst.ToList();
        //}

    }
}
