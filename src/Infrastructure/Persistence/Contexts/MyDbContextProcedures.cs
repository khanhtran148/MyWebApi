using MyWebApi.Application.Abstractions;

namespace MyWebApi.Infrastructure.Persistence.Contexts;

public class MyDbContextProcedures : IMyDbContextProcedures
{
    private readonly MyDbContext _context;

    public MyDbContextProcedures(MyDbContext context)
    {
        _context = context;
    }

    // public virtual async Task<List<GetSummaryCostResult>> GetSummaryCostAsync(int? LoggedEmployeeId, int? ProjectId, string ProjectsIdHavingAcccess, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
    // {
    //     var parameterreturnValue = new SqlParameter { ParameterName = "returnValue", Direction = System.Data.ParameterDirection.Output, SqlDbType = System.Data.SqlDbType.Int, };
    //
    //     var sqlParameters = new[]
    //     {
    //         new SqlParameter { ParameterName = "LoggedEmployeeId", Value = LoggedEmployeeId ?? Convert.DBNull, SqlDbType = System.Data.SqlDbType.Int, }, new SqlParameter { ParameterName = "ProjectId", Value = ProjectId ?? Convert.DBNull, SqlDbType = System.Data.SqlDbType.Int, }, new SqlParameter
    //         {
    //             ParameterName = "ProjectsIdHavingAcccess", Size = -1, Value = ProjectsIdHavingAcccess ?? Convert.DBNull, SqlDbType = System.Data.SqlDbType.VarChar,
    //         },
    //         parameterreturnValue,
    //     };
    //     var _ = await _context.SqlQueryAsync<GetSummaryCostResult>("EXEC @returnValue = [PM].[GetSummaryCost] @LoggedEmployeeId, @ProjectId, @ProjectsIdHavingAcccess", sqlParameters, cancellationToken);
    //
    //     returnValue?.SetValue(parameterreturnValue.Value);
    //
    //     return _;
    // }
}
