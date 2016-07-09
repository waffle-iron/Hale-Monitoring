using Dapper;
using Hale_Core.Entities.Modules;
using Hale_Core.Handlers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale_Core.Contexts
{
    internal class Modules : SqlHandler
    {
        public Module Create(Module module)
        {
            ConnectToDatabase();
            var id = connection.Query<int>("INSERT INTO [Modules].[Modules] ([Identifier],[Version]) VALUES (@identifier, dbo.usfSerializeVersion(@maj, @min, @rev)); SELECT SCOPE_IDENTITY()",
                new {
                    identifier = module.Identifier,
                    maj = module.Version.Major,
                    min = module.Version.Minor,
                    rev = module.Version.Revision });
            module.Id = id.First();
            return module;
        }

        public List<Module> List()
        {
            ConnectToDatabase();
            try
            {
                return connection.Query<Module>("SELECT * FROM Modules.Modules").ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public Module Get(Module module)
        {
            ConnectToDatabase();
            var obj = new
            {
                identifier = module.Identifier,
                maj = module.Version.Major,
                min = module.Version.Minor,
                rev = module.Version.Revision
            };
            var result = connection.Query<Module>("SELECT m.Id, m.Identifier, v.Major, v.Minor, v.Revision FROM Modules.Modules m CROSS APPLY dbo.usfDeserializeVersion(m.[Version]) AS v WHERE m.Identifier=@identifier AND m.Version=dbo.usfSerializeVersion(@maj, @min, @rev)", 
                obj);
            if (result.Count() > 0) return result.First();
            return Create(module);
        }
    }

    internal class ModuleFunctions : SqlHandler
    {
        public Function Create(Function function)
        {
            ConnectToDatabase();
            var id = connection.Query<int>("INSERT INTO [Modules].[Functions] ([Name],[Type],[ModuleId]) VALUES (@name, @type, @moduleId); SELECT SCOPE_IDENTITY()",
                new
                {
                    name = function.Name,
                    type = function.Type,
                    moduleId = function.ModuleId
                });
            function.Id = id.First();
            return function;
        }

        internal Function Get(Function function)
        {
            ConnectToDatabase();
            var result = connection.Query<Function>("SELECT * FROM Modules.Functions WHERE Name=@name AND Type=@type AND ModuleId=@moduleId",
                new
                {
                    Name = function.Name,
                    Type = function.Type,
                    ModuleId = function.ModuleId
                });
            if (result.Count() > 0) return result.Single();
            return Create(function);
        }
    }

    internal class Results : SqlHandler
    {
        public Result Create(Result result)
        {
            ConnectToDatabase();
            var id = connection.Query<int>("INSERT INTO [Modules].[Results] ([HostId],[ModuleId],[FunctionId],[Target],[ExecutionTime],[Message],[Exception]) VALUES (@hostId,@moduleId,@functionId,@target,@executionTime,@message,@exception); SELECT SCOPE_IDENTITY()",
                new
                {
                    hostId = result.HostId,
                    moduleId = result.ModuleId,
                    functionId = result.FunctionId,
                    target = result.Target,
                    executionTime = result.ExecutionTime,
                    message = result.Message,
                    exception = result.Exception
                });
            result.Id = id.First();
            return result;
        }

    }
}
