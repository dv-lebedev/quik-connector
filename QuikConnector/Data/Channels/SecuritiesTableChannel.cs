using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.Data.Channels
{
    public class SecuritiesTableChannel : DataChannel
    {
        public Dictionary<string, Security> Values { get; protected set; }

        public SecuritiesTableChannel()
        {
            Values = new Dictionary<string, Security>();
        }

        public Security this[string key]
        {
            get
            {
                if (Values.ContainsKey(key))
                {
                    return Values[key];
                }
                else
                {
                    Values.Add(key, new Security());
                    return Values[key];
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
                            mtrow.SetValue(col, xt.FloatValue);
                            break;
                        case XlTable.BlockType.String:
                            mtrow.SetValue(col, xt.StringValue);
                            break;
                        default:
                            break;
                    }
                }

                if (Values.ContainsKey(mtrow.SecCode))
                {
                    Values[mtrow.SecCode].Update(mtrow);
                }
                else
                {
                    Values.Add(mtrow.SecCode, mtrow);
                }
            }
        }
    }
}
