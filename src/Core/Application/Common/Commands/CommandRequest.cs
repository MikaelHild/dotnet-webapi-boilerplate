using System;
namespace FSH.WebApi.Application.Common.Commands
{
    public abstract class CommandRequest : ICommand
    {

        public CommandRequest(string userId, string userName = "")
        {
            CreatedByUserId = userId;
            CreatedByUserName = userName;
        }

        public int CommandId { get; set; }
        public string? JobId { get; set; }
        public int? ParentCommandId { get; set; }
        public string? RecurringJobId { get; set; }
        public CommandStatus Status { get; set; }
        public DateTime? ExecutionTime { get; set; }
        public string? CronExpression { get; set; }

        public string CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; }
    }
}

