using System;
namespace FSH.WebApi.Application.Common.Commands
{
    public enum CommandStatus
    {
        Enqueued,
        Running,
        Success,
        Error        
    }
}

