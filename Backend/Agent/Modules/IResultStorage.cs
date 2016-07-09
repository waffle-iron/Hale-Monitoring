using HaleLib.Modules;
using HaleLib.Modules.Actions;
using HaleLib.Modules.Checks;
using HaleLib.Modules.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Modules
{
    internal interface IResultStorage
    {
        void StoreResult(IModuleResultRecord record);
        void Persist();
        ResultRecordChunk Fetch(int maxRecords);
        void Clear(Guid[] uploaded);
    }

    internal static class ResultStorageExtensions
    {
        public static void StoreResult(this IResultStorage rs, QueuedModuleTask task, ModuleFunctionResult result)
        {
            rs.StoreResult(new ModuleResultRecord()
            {
                Module = result.Module,
                Runtime = result.Runtime,
                FunctionType = task.Task.FunctionType,
                Function = task.Task.Function,
                CompletionTime = task.Completed,
                Results = result.Results
            });
        }
    }
}
