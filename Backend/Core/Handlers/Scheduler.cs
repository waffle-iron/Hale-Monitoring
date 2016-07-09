using Hale.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale.Core.Handlers
{
    partial class CoreScheduler : Scheduler
    {
        protected override void RunTask(QueuedTask task)
        {
            var qiTask = (QueuedInternalTask)task;
            switch(qiTask.TaskType)
            {
                case InternalTaskType.CreateDistPackages:
                    createDistPackages();
                    break;
            }
            qiTask.Completed = DateTime.Now;
        }

        public void RunTask(InternalTaskType taskType)
        {
            var task = new InternalTask(taskType);
            EnqueueTask(task);
        }

        public void ScheduleTask(InternalTaskType taskType, TimeSpan interval)
        {
            var task = new InternalTask(taskType);
            ScheduleTask(task, interval);
        }
    }

    internal enum InternalTaskType
    {
        CreateDistPackages, VerifyAgentsAlive, CheckForCoreUpdates, CheckForAgentUpdates
    }

    internal class InternalTask : TaskBase
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

    internal class QueuedInternalTask : QueuedTask
    {
        public override string Id { get { return Task.ToString(); } }
        public InternalTask Task { get; set; }
        public InternalTaskType TaskType { get { return Task.Type; } }
    }

}
