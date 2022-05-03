using System;
using FSH.WebApi.Domain.Command;

namespace FSH.WebApi.Application.Common.Commands
{
    public interface ICommand : IRequest
    {
        int CommandId { get; set; }
        string? JobId { get; set; }
        int? ParentCommandId { get; }
        string? RecurringJobId { get; }
        CommandStatus Status { get; set; }
        DateTime? ExecutionTime { get; set; }
        string? CronExpression { get; }
        string CreatedByUserId { get; }
        string CreatedByUserName { get; }
    }

}

