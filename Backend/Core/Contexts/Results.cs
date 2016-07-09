using Hale_Core.Entities.Checks;
using Hale_Core.Handlers;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Hale_Core.Entities.Nodes;

namespace Hale_Core.Contexts
{
    internal class Results_ : SqlHandler
    {

        internal Results_()
        {

        }

        internal void Create(Result_ result)
        {
            ConnectToDatabase();
            connection.Execute(
                "exec uspCreateCheckResult "
                + " @hostId"
                + ", @checkId"
                + ", @checkDetailId"
                + ", @resultType"
                + ", @executionTime"
                + ", @message"
                + ", @exception"
                + ", @target",
                new
                {
                    hostId = result.HostId
                    , checkId = result.CheckId
                    , checkDetailId = result.CheckDetailId
                    , resultType = result.ResultType
                    , executionTime = result.ExecutionTime
                    , message = result.Message
                    , exception = result.Exception
                    , target = result.Target
                });
        }
        internal void Update(Result_ result)
        {
            ConnectToDatabase();
            connection.Execute("exec uspUpdateCheckResult"
                + " @id"
                + ", @hostId"
                + ", @checkId"
                + ", @checkDetailId"
                + ", @resultType"
                + ", @executionTime"
                + ", @message"
                + ", @exception"
                + ", @target",
                new
                {
                    result.Id
                    , result.HostId
                    , result.CheckId
                    , result.CheckDetailId
                    , result.ResultType
                    , result.ExecutionTime
                    , result.Message
                    , result.Exception
                    , result.Target
                });
        }
        internal void Delete(Result_ result)
        {
            ConnectToDatabase();
            connection.Execute("exec uspDeleteCheckResult @id",
                new
                {
                    id = result.Id
                });
        }
        internal Result_ Get(Result_ result)
        {
            ConnectToDatabase();
            return connection.Query<Result_>("exec uspGetCheckResult @id",
                new
                {
                    id = result.Id
                }).First();
        }
        internal List<Result_> List(Host host, Check check)
        {
            ConnectToDatabase();
            return connection.Query<Result_>("exec uspListCheckResultsHostCheck @hostId @checkId",
                new
                {
                    hostId = host.Id
                    , checkId = check.Id
                }).ToList();
        }
    }
}
