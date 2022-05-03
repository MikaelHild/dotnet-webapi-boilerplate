using System;
namespace FSH.WebApi.Domain.Command
{
    public abstract class CommandBase : BaseEntity<int>, IAggregateRoot
    {
        public string CommandType { get; set; }

        public override string ToString()
        {
            return this.CommandType.Split('.').Last();
        }
    }
    public class Command : CommandBase
    {
        public string CommandType { get; private set; } = default!;
        public string CommandJson { get; private set; } = default!;
        public string JobId { get; private set; } = default!;
        public CommandSource Source { get; private set; }
        public int? ParentRunId { get; private set; }
        public string? EntityId { get; set; }
        public string StartedByUserId { get; private set; } = default!;
        public string StartedByUserName { get; private set; } = default!;
        public DateTimeOffset StartTime { get; private set; }
        public DateTimeOffset? EndTime { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? Exception { get; private set; }
        public bool Dismissed { get; private set; }

        public Command(string commandType, string commandJson, string jobId, CommandSource source, int? parentRunId, string startedByUserId, string startedByUserName)
            : this(commandType, commandJson, jobId, source, parentRunId, startedByUserId,startedByUserName, DateTimeOffset.UtcNow, null, null, false)
        {
        }

        public Command(string commandType, string commandJson, string jobId, CommandSource source, int? parentRunId, string startedByUserId,string startedByUserName, DateTimeOffset startTime, string? errorMessage, string? exception, bool dismissed)
        {
            CommandType = commandType;
            CommandJson = commandJson;
            JobId = jobId;
            Source = source;
            ParentRunId = parentRunId;
            StartedByUserId = startedByUserId;
            StartedByUserName = startedByUserName;
            StartTime = startTime;
            ErrorMessage = errorMessage;
            Exception = exception;
            Dismissed = dismissed;
        }

        //Todo: Use method to set user information.
        public void SetUser(string userId, string userName)
        {
            StartedByUserId = userId;
            StartedByUserName = userName;
        }

        public void Finalize(Exception? ex = null)
        {
            EndTime = DateTimeOffset.UtcNow;
            if (ex is not null)
            {
                ErrorMessage = ex.Message;
                Exception = ex.ToString();
            }
        }

    }

    public enum CommandExecutionType
    {
        Once,
        Recurring
    }

    public enum CommandRecurrenceType
    {
        Monthly,
        Weekly,
        Daily,
        Hourly,
        Minutes
    }
}

