using System;
using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class PostRequest : IExamplesProvider
    {        
        [Required]
        public string Name { get; set; }
        [Required]
        public string Last { get; set; }

        public object GetExamples()
        {
            return new PostRequest()
            {
                Name = "Test",
                Last = "Test"
            };
        }
    }
}
