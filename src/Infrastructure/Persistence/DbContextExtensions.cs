using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyWebApi.Infrastructure.Persistence;

public static class DbContextExtensions
{
    public static async Task<List<T>> SqlQueryAsync<T>(this DbContext db, string sql, object[] parameters = null, CancellationToken cancellationToken = default) where T : class
    {
        if (parameters is null)
        {
            parameters = new object[] { };
        }

        if (typeof(T).GetProperties().Any())
        {
            return await db.Set<T>().FromSqlRaw(sql, parameters).ToListAsync(cancellationToken);
        }
        else
        {
            await db.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
            return default;
        }
    }

    public static DataTable CreateDataTable<T>(this IEnumerable<T> cd) //where T : class
    {
        // create data table to insert items
        var dt = new DataTable("Data");
        //Get the public prop to insert
        foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            dt.Columns.Add(prop.Name, prop.PropertyType);
        }
        // Create new DataRow objects and add to DataTable.
        foreach (var conversionData in cd)
        {
            var row = dt.NewRow();
            foreach (var column in dt.Columns)
            {
                var columnName = column.ToString();
                row[columnName] = GetPropValue(conversionData, columnName);
            }
            dt.Rows.Add(row);
        }
        return dt;
    }

    private static object GetPropValue(object src, string propName) => src.GetType().GetProperty(propName).GetValue(src, null);
}
