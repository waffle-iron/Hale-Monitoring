using Hale.Core.Entities.Nodes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hale.Core.Handlers;

namespace Hale.Core.Contexts
{
    internal class Hosts : SqlHandler
    {
        private readonly int _idNotSet = 0;

        public void Create (Host host)
        {
            ConnectToDatabase();
            connection.Execute("exec uspCreateHost @name, @hostname, @ip",
                new
                {
                    name = host.Name
                    , hostname = host.HostName
                    , ip = host.Ip
                }
            );
        }

        // FIXME[SA]: Is there a better way to do this which lets us stick to only the Update method?
        // Overloading with bool does not cut it as we have two different overloads and do not want to have two different bools to set.
        public void Update (Host host)
        {
            ConnectToDatabase();
            connection.Execute("exec uspUpdateHost"
                + " @id"
                + ", @name"
                + ", @hostname"
                + ", @ip"
                + ", @guid",

                new
                {
                    id = host.Id,
                    name = host.Name,
                    hostname = host.HostName,
                    ip = host.Ip,
                    guid = host.Guid,
                }
                );
        }

        // Part of the fixme above.
        public void UpdateRsa(Host host)
        {
            ConnectToDatabase();
            connection.Execute("exec uspUpdateHostRsaKey @id, @rsa",
                new
                {
                    id = host.Id
                    ,
                    rsa = host.RsaKey
                });
        }

        // Part of the fixme above.
        public void UpdateStatus(Host host)
        {
            ConnectToDatabase();
            connection.Execute("exec uspUpdateHostStatus @id, @status",
                new
                {
                    id = host.Id,
                    status = host.Status
                });
        }

        public void Delete (Host host)
        {
            ConnectToDatabase();
            connection.Execute("exec uspDeleteHost @id",
                new
                {
                    id = host.Id
                }
            );
            
        }

        public Host Get (Host host)
        {
            ConnectToDatabase();
            if (host.Id != _idNotSet)
            {
                return connection.Query<Host>("exec uspGetHost @id",
                    new
                    {
                        id = host.Id
                    }
                ).First();
            }
            else if (host.Guid != null)
            {
                return connection.Query<Host>("exec uspGetHostByGuid @guid",
                    new
                    {
                        guid = host.Guid
                    }
                ).First();
            }
            else
            {
                throw new Exception("No suitable selectors found in Host search object.");
            }
        }

        public List<Host> List()
        {
            ConnectToDatabase();
            return connection.Query<Host>("exec uspListHosts").ToList();
        }
    }
}
