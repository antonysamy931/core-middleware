using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace MiddlewareHandler
{
    public class ValidateMiddleware
    {
        private readonly RequestDelegate _next;

        private XmlSchemaSet _schemaSet;

        private List<string> validationErrorMessages;

        private IConfiguration Configuration { get; set; }

        public ValidateMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.Configuration = configuration;
            this._next = next;
            this._schemaSet = new XmlSchemaSet();

            string[] filePaths = Directory.GetFiles((Directory.GetCurrentDirectory() + "/Schemas"), "*.xsd", SearchOption.AllDirectories);

            foreach (string file in filePaths)
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    _schemaSet.Add(XmlSchema.Read(reader, null));
                }
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            validationErrorMessages = null;
            var url = context.Request.Path;
            var ValidationHandler = new ValidationEventHandler(ValidationEventHandler);

            if (context.Request.Method.ToLower() == "post")
            {
                string ExpectedType = GetExpectedType(context.Request);
                if (string.IsNullOrEmpty(ExpectedType))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.ContentType = "application/json";
                    var errorMessage = new
                    {
                        error = "error message"
                    };
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(errorMessage), Encoding.UTF8);
                    return;
                }
                else
                {
                    string jsonBody = string.Empty;                    
                    using(var injectedRequestStream = new MemoryStream())
                    {
                        using (var bodyReader = new StreamReader(context.Request.Body))
                        {
                            jsonBody = bodyReader.ReadToEnd();                            
                            var bytesToWrite = Encoding.UTF8.GetBytes(jsonBody);
                            injectedRequestStream.Write(bytesToWrite, 0, bytesToWrite.Length);
                            injectedRequestStream.Seek(0, SeekOrigin.Begin);
                            context.Request.Body = injectedRequestStream;
                        }
                    }
                    if (!string.IsNullOrEmpty(jsonBody))
                    {
                        XDocument doc = ToXDocument(JsonConvert.DeserializeXmlNode(jsonBody, ExpectedType));
                        doc.Validate(_schemaSet, ValidationHandler);
                        if(validationErrorMessages != null && validationErrorMessages.Count > 0)
                        {
                            string ErrorMessage = string.Join(Environment.NewLine, validationErrorMessages.ToArray());
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            context.Response.ContentType = "application/json";
                            var errorMessage = new
                            {
                                Reason = ErrorMessage
                            };
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorMessage), Encoding.UTF8);
                            return;
                        }
                    }
                }
            }            
            await _next.Invoke(context);                       
        }

        private string GetExpectedType(HttpRequest request)
        {
            return Configuration[request.Path.Value.ToLower()];
        }

        private XDocument ToXDocument(XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }

        void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            String errorMessage = e.Message;            
            validationErrorMessages = validationErrorMessages ?? new List<string>();
            validationErrorMessages.Add(errorMessage);
        }

        string GetJsonString(string bodyText)
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Converters = new List<Newtonsoft.Json.JsonConverter> {
                    new Newtonsoft.Json.Converters.StringEnumConverter()
                }
            };
            return "";
        }
    }
}
