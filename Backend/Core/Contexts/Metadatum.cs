using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hale_Core.Entities.Shared;
using Hale_Core.Handlers;

namespace Hale_Core.Contexts
{
    internal class Metadatum : SqlHandler
    {

        internal Metadata Get(Metadata payload)
        {
            ConnectToDatabase();
            return connection.Query<Metadata>("uspGetMetadata @type @attribute",
                new
                {
                    payload.Type,
                    payload.Attribute
                }).First();
        }

        internal List<Metadata> List(Metadata payload)
        {
            ConnectToDatabase();
            return connection.Query<Metadata>("exec uspListMetadataByType @type",
                new
                {
                    payload.Type
                })
                .ToList();
        }

        internal void Create(Metadata payload)
        {
            ConnectToDatabase();
            connection.Execute("exec uspCreateMetadata @type, @attribute, @label, @description, @required, @protected",
                new
                {
                    payload.Type,
                    payload.Attribute,
                    payload.Label,
                    payload.Description,
                    payload.Required,
                    payload.Protected
                });
        }

        internal void Update(Metadata payload)
        {
            ConnectToDatabase();
            connection.Execute("exec uspUpdateMetadata @id @type, @attribute, @label, @description, @required, @protected",
                new
                {
                    payload.Id,
                    payload.Type,
                    payload.Attribute,
                    payload.Label,
                    payload.Description,
                    payload.Required,
                    payload.Protected
                });

        }

        internal void Delete(Metadata payload)
        {
            ConnectToDatabase();
            connection.Execute("exec uspDeleteMetadata @id",
                new
                {
                    payload.Id
                });

        }
    }
}
