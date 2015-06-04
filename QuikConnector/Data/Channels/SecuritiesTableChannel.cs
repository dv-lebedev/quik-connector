using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace QuikConnector.Data.Channels
{
    public class SecuritiesTableChannel : DataChannel
    {
        private Dictionary<string, SecurityContainer> Securities { get;  set; }

        private PropertyInfo[] SecurityProperties;

        public SecuritiesTableChannel()
        {
            Securities = new Dictionary<string, SecurityContainer>();

            SecurityProperties = typeof(Security).GetProperties();
        }

        public SecurityContainer this[string key]
        {  
            get
            {
                if (Securities.ContainsKey(key))
                {
                    return Securities[key];
                }
                else
                {
                    Securities.Add(key, new SecurityContainer());
                    return Securities[key];
                }
            }
        }

        protected override void ProcessTable(XlTable xt)
        {
            for (int row = 0; row < xt.Rows; row++)
            {
                Security mtrow = new Security();

                for (int col = 0; col < xt.Columns; col++)
                {
                    xt.ReadValue();
                    
                    switch (xt.ValueType)
                    {
                        case XlTable.BlockType.Float:
                            SecurityProperties[col].SetValue(mtrow,(decimal) xt.FloatValue);
                            break;
                        case XlTable.BlockType.String:
                            if (xt.StringValue != string.Empty)
                            {
                                SecurityProperties[col].SetValue(mtrow, xt.StringValue);
                            }
                            break;
                        default:
                            break;
                    }
                }

                if (Securities.ContainsKey(mtrow.Code))
                {
                    Securities[mtrow.Code].Value = mtrow;
                }
                else
                {
                    Securities.Add(mtrow.Code, new SecurityContainer { Value = mtrow });
                }
            }
        }
    }
}
