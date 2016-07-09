using Dapper;
using Hale_Core.Entities.Nodes;
using Hale_Core.Handlers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale_Core.Contexts
{
    internal class HostDetails : SqlHandler
    {
        public void Create(Host host, HostDetail detail)
        {
            ConnectToDatabase();
            connection.Execute("exec uspCreateHostDetail @id @key @value",
                new
                {
                    id = host.Id,
                    key = detail.Key,
                    value = detail.Value
                }
            );
        }

        public void Update(Host host, HostDetail detail)
        {
            ConnectToDatabase();
            connection.Execute("exec uspUpdateHostDetail @id @key @value",
                new
                {
                    id = host.Id,
                    key = detail.Key,
                    value = detail.Value
                }
            );
        }

        public void Delete(Host host, HostDetail detail)
        {
            ConnectToDatabase();
            connection.Execute("exec uspDeleteHostDetail @id @key",
                new
                {
                    id = host.Id,
                    key = detail.Key
                }
            );
        }
        public HostDetail Get(Host host, HostDetail detail)
        {
            ConnectToDatabase();
            return connection.Query<HostDetail>("exec uspGetHostDetail @id @key",
                new
                {
                    id = host.Id,
                    key = detail.Key
                })
                .First();
            
        }

        public List<HostDetail> List(Host host)
        {
            ConnectToDatabase();
            return connection.Query<HostDetail>("exec uspListHostDetails @id",
                new
                {
                    id = host.Id
                })
                .ToList();
        }
    }
}
