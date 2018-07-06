using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class PostRequest : IExamplesProvider
    {        
        [Required]
        public string Name { get; set; }
        [Required]
        public string Last { get; set; }
        public List<DocumentInfo> Documents { get; set; }        

        public object GetExamples()
        {
            return new PostRequest()
            {
                Name = "Test",
                Last = "Test",
                Documents = new List<DocumentInfo>()
                {
                    new DocumentInfo()
                    {
                        Name = "Ja",
                        Description = "JJJJ"
                    },
                    new DocumentInfo()
                    {
                        Name = "ka",
                        Description = "KKKK"
                    }
                }
            };
        }
    }

    public class DocumentInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
