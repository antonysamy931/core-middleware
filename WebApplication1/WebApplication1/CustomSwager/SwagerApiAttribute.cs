using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.CustomSwager
{
    public class SwagerApiAttribute : Attribute
    {
        public SwagerApiAttribute(Type requestType) => this.ApiRequestType = requestType;
        public Type ApiRequestType { get; private set; }
    }
}
