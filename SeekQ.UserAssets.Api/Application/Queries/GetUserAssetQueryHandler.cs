using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Common.Exceptions;
using App.Common.SeedWork;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using SeekQ.UserAssets.Api.Models;

namespace SeekQ.UserAssets.Api.Application.Queries
{
    public class GetUserAssetQueryHandler
    {
        public class Query : IRequest<IEnumerable<UserAssetModel>>
        {
            public Query(Guid userId)
            {
                UserId = userId;
            }

            public Guid UserId { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<UserAssetModel>>
        {
            private CommonGlobalAppSingleSettings _commonGlobalAppSingleSettings;

            public Handler(CommonGlobalAppSingleSettings commonGlobalAppSingleSettings)
            {
                _commonGlobalAppSingleSettings = commonGlobalAppSingleSettings;
            }

            public async Task<IEnumerable<UserAssetModel>> Handle(
                Query query,
                CancellationToken cancellationToken)
            {
                try
                {
                    using (IDbConnection conn = new SqlConnection(_commonGlobalAppSingleSettings.MssqlConnectionString))
                    {
                        string sql =
                            @"
                        SELECT [Id]
                              ,[UserId]
                              ,[Url]
                              ,[Order]
                              ,[AssetTypeId]
                          FROM UserAssets
                        WHERE UserId = @UserId";

                        var result = await conn.QueryAsync<UserAssetModel>(sql, new { query.UserId });

                        return result.AsEnumerable();
                    }
                }
                catch (Exception e)
                {
                    throw new AppException(e.Message);
                }
            }
        }
    }
}
