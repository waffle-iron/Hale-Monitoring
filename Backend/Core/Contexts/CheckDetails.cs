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
    internal class CheckDetails : SqlHandler
    {
        internal void Create(CheckDetail detail)
        {
            ConnectToDatabase();
            try
            {
                connection.Execute(
                    "exec uspCreateCheckDetail "
                    + ", @checkId"
                    + ", @version"
                    + ", @name"
                    + ", @description"
                    + ", @activated",
                    new
                    {
                        checkId = detail.CheckId
                        , version = detail.Version
                        , name = detail.Name
                        , description = detail.Description
                        , activated = detail.Activated
                    });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
        internal void Update(CheckDetail detail)
        {
            ConnectToDatabase();
            try
            {
                connection.Execute("exec uspUpdateCheckDetail "
                    + ", @id"
                    + ", @checkId"
                    + ", @version"
                    + ", @name"
                    + ", @description"
                    + ", @activated",
                    new
                    {
                        checkId = detail.CheckId
                        ,version = detail.Version
                        ,name = detail.Name
                        ,description = detail.Description
                        ,activated = detail.Activated
                    });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
        internal void Delete(CheckDetail detail)
        {
            ConnectToDatabase();
            try
            {
                connection.Execute("exec uspDeleteCheckDetail @id",
                    new
                    {
                        id = detail.Id
                    });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
        internal CheckDetail Get(CheckDetail detail)
        {
            ConnectToDatabase();
            if (detail.Id == 0)
            {
                return connection.Query<CheckDetail>("exec uspGetCheckDetail @checkId @version",
                    new
                    {
                        checkId = detail.CheckId,
                        version = detail.Version
                    })
                    .First();
            }
            else
            {
                return connection.Query<CheckDetail>("exec uspGetCheckDetail @id",
                    new
                    {
                        id = detail.Id
                    })
                    .First();
            }
        }
        internal List<CheckDetail> List()
        {
            ConnectToDatabase();
            try
            {
                return connection.Query<CheckDetail>("exec uspListCheckDetails").ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
        internal List<CheckDetail> List(Check check)
        {
            ConnectToDatabase();
            try
            {
                return connection.Query<CheckDetail>("exec uspListCheckDetailsByCheck "
                    + "@checkId",
                    new
                    {
                        checkId = check.Id
                    }).ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
    }
}
