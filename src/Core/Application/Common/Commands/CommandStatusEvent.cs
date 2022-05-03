using System;
using FSH.WebApi.Shared.Events;

namespace FSH.WebApi.Application.Common.Commands
{
    public class CommandStatusEvent : IEvent, INotificationMessage
    {
        public string JobId { get; }
        public int RunId { get; }
        public string Source { get; }
        public string CommandType { get; }
        public CommandStatus Status { get; }
        public string? Error { get; }

        public CommandStatusEvent(string jobId, int runId, string source, string commandType, CommandStatus status, string? error = null)
        {
            JobId = jobId;
            RunId = runId;
            Source = source;
            CommandType = commandType;
            Status = status;
            Error = error;
        }
    }
}

