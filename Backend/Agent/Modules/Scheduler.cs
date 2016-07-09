using Agent.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Timers = System.Timers;
using Agent.Communication;
using HaleLib.Modules;
using HaleLib.ModuleLoader;
using HaleLib.Modules.Checks;
using Agent.Modules;
using HaleLib.Modules.Info;
using HaleLib.Modules.Actions;
using HaleLib.Utilities;
using HaleLib;

namespace Agent
{
    class AgentScheduler: Scheduler
    {
        public AgentScheduler()
        {
            var env = ServiceProvider.GetService<EnvironmentConfig>();
            env.ModulePath = Path.Combine(env.DataPath, "Modules");

            ProcessConfig();

            PrintTasks();
        }

        private void ProcessInternalTasks(AgentConfig config)
        {
            foreach (InternalTaskType itt in Enum.GetValues(typeof(InternalTaskType)))
            {
                var key = itt.ToString();
                key = key.Substring(0, 1).ToLower() + key.Substring(1); // Convert to dict key
                if (!config.Tasks[key].Enabled)
                    continue;
                ScheduleTask(new InternalTask(itt),
                    config.Tasks[key].Interval);
                if (config.Tasks[key].Startup)
                    EnqueueTask(new InternalTask(itt));
            }
        }

        public void ProcessConfig()
        {
            var config = ServiceProvider.GetServiceCritical<AgentConfig>();

            //_verifyChecks(config.Checks);

            foreach (var kvpCheck in config.Checks)
            {
                var check = kvpCheck.Value;

                var task = new ModuleTask();
                task.Function = check.Check;
                task.Module = check.Module;
                task.FunctionType = ModuleFunctionType.Check;
                task.Settings = check;

                ScheduleTask(task, check.Interval);
            }

            foreach (var kvpInfo in config.Info)
            {
                var info = kvpInfo.Value;

                var task = new ModuleTask();
                task.Function = info.Info;
                task.Module = info.Module;
                task.FunctionType = ModuleFunctionType.Info;
                task.Settings = info;

                if (task.Settings.Startup)
                    EnqueueTasks(new List<TaskBase>() { task });

                ScheduleTask(task, info.Interval);
            }

            foreach (var kvpAction in config.Actions)
            {
                var action = kvpAction.Value;

                var task = new ModuleTask();
                task.Function = action.Action;
                task.Module = action.Module;
                task.FunctionType = ModuleFunctionType.Action;
                task.Settings = action;

                ScheduleTask(task, action.Interval);
            }

            ProcessInternalTasks(config);
        }

       
        protected override void RunTask(QueuedTask queuedTask)
        {
            _log.Info($"Running task {queuedTask.Id}");
            if (queuedTask.GetType() == typeof(QueuedModuleTask))
            {
                RunModuleTask(queuedTask as QueuedModuleTask);
            }
            else
            {
                if (queuedTask.GetType() == typeof(QueuedInternalTask))
                {
                    var task = (QueuedInternalTask)queuedTask;
                    switch(task.TaskType)
                    {
                        case InternalTaskType.PersistResults:
                            internalTaskPersistResults();
                            break;
                        case InternalTaskType.UploadResults:
                            internalTaskUploadResults();
                            break;
                        case InternalTaskType.SendHeartbeat:
                            internalTaskSendHeartbeat();
                            break;
                        default:
                        break;
                    }
                }
            }
        }

        private void internalTaskSendHeartbeat()
        {
            var nemesis = ServiceProvider.GetService<NemesisController>();
            if (nemesis == null) return;
            _log.Debug("Sending hearbeat to Core...");
            nemesis.SendCommand("heartbeat"); // TODO: Handle errors? -NM 2016-02-07
        }

        private void internalTaskPersistResults()
        {
            var resultStorage = ServiceProvider.GetService<IResultStorage>();
            if (resultStorage == null) return;

            resultStorage.Persist();
        }

        private void internalTaskUploadResults()
        {
            var nemesis = ServiceProvider.GetService<NemesisController>();
            if (nemesis == null) return;

            var resultStorage = ServiceProvider.GetService<IResultStorage>();
            if (resultStorage == null) return;

            var records = resultStorage.Fetch(10);
            var uploaded = nemesis.UploadResults(records);

            if(uploaded != null)
                resultStorage.Clear(uploaded);
        }

        private void RunModuleTask(QueuedModuleTask queuedTask)
        {
            var config = ServiceProvider.GetService<AgentConfig>();
            if (config == null) return;

            var resultStorage = ServiceProvider.GetService<IResultStorage>();
            if (resultStorage == null) return;

            var task = queuedTask.Task;
            try {
                var checkPath = _getModulePath(task.Module);
                var dll = Path.Combine(checkPath, config.Modules[task.Module].Dll);

                if (!File.Exists(dll))
                {
                    throw new FileNotFoundException($"Check DLL \"{dll}\" not found!");
                }

                switch(task.FunctionType)
                {
                    case ModuleFunctionType.Check: {
                        var functionResult = ModuleLoader.ExecuteCheckFunction(dll, checkPath, task.Function, (CheckSettings)task.Settings);

                        queuedTask.Completed = DateTime.Now;

                        foreach (var kvpResult in functionResult.CheckResults)
                        {
                            var result = kvpResult.Value;
                            var target = kvpResult.Key;
                            if (result.RanSuccessfully)
                                _log.Info($"Task {task}({result.Target}) returned the raw values {result.RawValues}");
                            else
                                _log.Warn($"Task {task}({result.Target}) executed with error: {result.ExecutionException.Message}");
                            _log.Info($"  -> {result.Message}");
                        }
                        resultStorage.StoreResult(queuedTask, functionResult);
                    } break;
                    case ModuleFunctionType.Info: {
                        var functionResult = ModuleLoader.ExecuteInfoFunction(dll, checkPath, task.Function, (InfoSettings)task.Settings);

                        queuedTask.Completed = DateTime.Now;

                        foreach (var kvpResult in functionResult.InfoResults)
                        {
                            var result = kvpResult.Value;
                            var target = kvpResult.Key;
                            if (result.RanSuccessfully || result.ExecutionException == null)
                            {
                                _log.Info($"Task {task}({result.Target}) executed successfully!");
                                foreach(var item in result.Items)
                                {
                                    _log.Debug($" - {item.Key} = {item.Value}");
                                }
                            }
                            else
                                _log.Warn($"Task {task}({result.Target}) executed with error: {result.ExecutionException.Message}");
                            _log.Info($"  -> {result.Message}");
                        }
                        resultStorage.StoreResult(queuedTask, functionResult);
                    } break;
                    case ModuleFunctionType.Action: {
                        var functionResult = ModuleLoader.ExecuteActionFunction(dll, checkPath, task.Function, (ActionSettings)task.Settings);

                        queuedTask.Completed = DateTime.Now;

                        foreach (var kvpResult in functionResult.ActionResults)
                        {
                            var result = kvpResult.Value;
                            var target = kvpResult.Key;
                            if (result.RanSuccessfully)
                                _log.Info($"Task {task}({result.Target}) executed successfully!");
                            else
                                _log.Warn($"Task {task}({result.Target}) executed with error: {result.ExecutionException.Message}");
                            _log.Info($"  -> {result.Message}");
                        }
                        resultStorage.StoreResult(queuedTask, functionResult);
                    } break;
                }

                _log.Info($"The task {task} completed in { (queuedTask.Completed - queuedTask.Added).TotalSeconds.ToString("F2")} second(s)");


            }
            catch (Exception x)
            {
                _log.Error($"Error running Task {task}({string.Join(",", task.Targets)}): {x.Message}");
            }

        }

        private string _getModulePath(string check)
        {
            var env = ServiceProvider.GetServiceCritical<EnvironmentConfig>();
            return Path.Combine(env.ModulePath, check.Substring(0, check.LastIndexOf('.')),
                check.Substring(check.LastIndexOf('.') + 1));
        }

        private new void _updateQueue()
        {
            if (TaskTimers.Count > 0)
            {
                foreach (var timer in TaskTimers)
                {
                    timer.Value.Stop();
                }
                TaskTimers.Clear();

            }

            foreach (var kvpCheckTask in ScheduleTasks)
            {
                try {
                    var timer = new Timers.Timer(kvpCheckTask.Key.TotalMilliseconds); // Maximum interval is 24 days (Int.Max milliseconds)

                    timer.Elapsed += delegate { EnqueueTasks(kvpCheckTask.Value); };

                    timer.Start();

                    TaskTimers.Add(kvpCheckTask.Key, timer);
                }
                catch (Exception x)
                {
                    _log.Error($"Could not add Task interval {kvpCheckTask.Key}: {x.Message}");
                }
            }

        }

        private void _verifyChecks(Dictionary<string, ModuleSettingsBase> checks)
        {
            foreach (var check in checks)
            {
                var checkName = check.Key;
                var checkSettings = check.Value;

                // Todo: Check if each check DLL is present and presents itself according to spec @todo -NM
            }
        }

        private new void EnqueueTasks(List<TaskBase> tasks)
        {
            _log.Debug($"Enqueueing {tasks.Count} task(s).");
            foreach (var task in tasks)
            {
                QueuedTask queuedTask = null;
                if (task.GetType() == typeof(ModuleTask))
                {
                    queuedTask = new QueuedModuleTask() { Task = (ModuleTask)task };
                }
                else if (task.GetType() == typeof(InternalTask))
                {
                    queuedTask = new QueuedInternalTask() { Task = (InternalTask)task };
                }
                else
                {
                    throw new Exception($"Internal scheduler error: Unknown Task Type {task.GetType().ToString()}.");
                }
                if (TaskQueue.Contains(queuedTask))
                {
                    // Todo: Handle dead-locked tasks @todo -NM
                    _log.Warn($"Skipping task {task}, previous task not completed.");
                }
                else
                {
                    queuedTask.Added = DateTime.Now;
                    TaskQueue.Enqueue(queuedTask);
                }
            }
        }
    }

    internal class ModuleTask: TaskBase
    {
        public string Module;
        public Version ModuleVersion;
        public string Function;
        public string[] Targets { get { return Settings.Targets.ToArray();  } }
        public ModuleFunctionType FunctionType;
        public ModuleSettingsBase Settings;
        public override string ToString()
        {
            return $"<{Module}[{FunctionType}]{Function}({String.Join(",", Targets)})>";
        }
        public override QueuedTask ToQueued()
        {
            return new QueuedModuleTask() { Task = this };
        }
    }

    internal class QueuedModuleTask : QueuedTask
    {
        public override string Id { get { return Task.ToString(); } }
        public ModuleTask Task;
    }

    internal class InternalTask: TaskBase
    {
        public InternalTaskType Type { get; set; }
        public InternalTask() { }
        public InternalTask(InternalTaskType type)
        {
            Type = type;
        }
        public override string ToString()
        {
            return $"<com.itshale.agent[internal]{Type.ToString().ToLower()}()>";
        }
        public override QueuedTask ToQueued()
        {
            return new QueuedInternalTask() { Task = this };
        }
    }

    internal class QueuedInternalTask: QueuedTask
    {
        public override string Id { get { return Task.ToString(); } }
        public InternalTask Task { get; set; }
        public InternalTaskType TaskType { get { return Task.Type; } }
    }

    internal enum InternalTaskType
    {
        UploadResults, PersistResults, SendHeartbeat
    }


}
