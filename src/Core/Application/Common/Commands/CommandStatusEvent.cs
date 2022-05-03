using System;
using FSH.WebApi.Shared.Events;

namespace FSH.WebApi.Application.Common.Commands
{
    public class CommandStatusEvent : IEvent, INotificationMessage
    {
        public string JobId { get; }
        public int RunId { get; }
        public int SourceId { get; }
        public string CommandType { get; }
        public CommandStatus Status { get; }
        public string? Error { get; }

        public CommandStatusEvent(string jobId, int runId, int sourceId, string commandType, CommandStatus status, string? error = null)
        {
            JobId = jobId;
            RunId = runId;
            SourceId = sourceId;
            CommandType = commandType;
            Status = status;
            Error = error;
        }
    }
}

