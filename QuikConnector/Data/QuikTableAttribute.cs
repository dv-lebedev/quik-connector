using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.Data
{
    public class QuikTableAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
