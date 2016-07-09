using Dapper;
using Hale.Core.Entities.Checks;
using Hale.Core.Entities.Modules;
using Hale.Core.Handlers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale.Core.Contexts
{
    internal class Metrics : SqlHandler
    {
        internal void Create(Metric metric)
        {
            ConnectToDatabase();
            connection.Execute(
                "exec uspCreateMetric "
                + ", @resultId"
                + ", @target"
                + ", @rawValue"
                + ", @weight",
                new
                {
                    resultId = metric.ResultId
                    , target = metric.Target
                    , rawValue = metric.RawValue
                    , weight = metric.Weight
                });
        }
        internal void Update(Metric metric)
        {
            ConnectToDatabase();
            connection.Execute(
                "exec uspUpdateMetric "
                + ", @id"
                + ", @resultId"
                + ", @target"
                + ", @rawValue"
                + ", @weight",
                new
                {
                    id = metric.Id
                    ,resultId = metric.ResultId
                    ,target = metric.Target
                    ,rawValue = metric.RawValue
                    ,weight = metric.Weight
                });
        }
        internal void Delete(Metric metric)
        {
            ConnectToDatabase();
            connection.Execute("exec uspDeleteMetric @id",
                new
                {
                    id = metric.Id
                });
        }
        internal Metric Get(Metric metric)
        {
            ConnectToDatabase();
            return connection.Query<Metric>("exec uspGetMetric @id",
                new
                {
                    id = metric.Id
                })
                .First();
        }
        internal List<Metric> List(Result result)
        {
            ConnectToDatabase();
            return connection.Query<Metric>("exec uspListMetricsForResult @resultId",
                new
                {
                    id = result.Id
                })
                .ToList();
        }

        // FIXME[SA]: Add table Nodes.CheckAssignment
        
        // There is a gap in the data model here. We need a junction table to
        // assign checks + checkdetails to hosts who will run them.
        
        // internal List<Metric> List(Host host, CheckDetail detail)
    }
}
