using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.Data
{
    public class DataTable<T> : DataChannel, QuikConnector.Data.IDataTable<T> where T : new()
    {
        PropertyInfo[] properties;

        public List<T> Rows { get; protected set; }

        public event EventHandler<List<T>> Updated;

        public DataTable()
        {
            properties = typeof(T).GetProperties();

            Rows = new List<T>();
        }

        protected override void ProcessTable(XlTable xt)
        {
            Rows.Clear();

            for (int row = 0; row < xt.Rows; row++)
            {
                T item = new T();

                for (int col = 0; col < xt.Columns; col++)
                {
                    xt.ReadValue();

                    switch (xt.ValueType)
                    {
                        case XlTable.BlockType.Float:
                            properties[col].SetValue(item, (decimal)xt.FloatValue);
                            break;

                        case XlTable.BlockType.String:
                            if (xt.StringValue != string.Empty)
                                properties[col].SetValue(item, xt.StringValue);
                            break;

                        default:
                            break;
                    }
                }

                Rows.Add(item);
            }

            OnUpdated(this, Rows);
        }

        protected virtual void OnUpdated(object sender, List<T> e)
        {
            if (Updated != null) Updated(sender, e);
        }
    }
}
