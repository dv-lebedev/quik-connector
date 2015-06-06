using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.Exceptions
{
    public class AttributeNotFoundException : Exception
    {
        private Type attributeType;

        public override string Message
        {
            get
            {
                return attributeType.Name + " is not implemented";
            }
        }

        public AttributeNotFoundException(Type attributeType)
        {
            this.attributeType = attributeType;
        }
    }
}
