using System;
namespace QuikConnector.Data
{
    public interface IDataTable<T>
     where T : new()
    {
        System.Collections.Generic.List<T> Rows { get; }
        event EventHandler<System.Collections.Generic.List<T>> Updated;
    }
}
