using Dapper;
using Hale_Core.Entities.Checks;
using Hale_Core.Handlers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale_Core.Contexts
{
    internal class Checks : SqlHandler
    {
        internal void Create(Check check)
        {
            ConnectToDatabase();
            connection.Execute(
                "exec uspCreateCheck @identifier",
                new
                {
                    identifier = check.Identifier
                }
            );
        }
        internal void Update(Check check)
        {
            ConnectToDatabase();
            connection.Execute("exec uspUpdateCheck @id @identifier",
                new
                {
                    id = check.Id
                    , identifier = check.Identifier
                        
                }
            );

        }
        internal void Delete(Check check)
        {
            ConnectToDatabase();
            connection.Execute("exec uspDeleteCheck @id",
                new
                {
                    id = check.Id
                }
            );
        }
        internal Check Get(Check check)
        {
            ConnectToDatabase();
            if (check.Id == 0)
            {
                return connection.Query<Check>("exec uspGetCheckByIdentifier @identifier",
                new
                {
                    identifier = check.Identifier
                })
                .First();
            }
            else
            {
                return connection.Query<Check>("exec uspGetCheck @id",
                    new
                    {
                        id = check.Id
                    })
                    .First();
            }
        }
        internal List<Check> List()
        {
            ConnectToDatabase();
            try
            {
                return connection.Query<Check>("exec uspListChecks").ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
    }
}
