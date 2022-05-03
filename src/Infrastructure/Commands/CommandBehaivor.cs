using System;
using System.Text.Json;
using FSH.WebApi.Application.Common.Commands;
using FSH.WebApi.Application.Common.Events;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Domain.Command;
using MediatR;

namespace FSH.WebApi.Infrastructure.Commands
{
    internal class CommandBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICommand
    {
        private readonly IRepository<Command> _commandRepo;
        private readonly IEventPublisher _events;
        private readonly ICurrentUser _currentUser;

        public CommandBehavior(IRepository<Command> commandRepo, IEventPublisher events, ICurrentUser currentUser)
        {
            _commandRepo = commandRepo;
            _events = events;
            _currentUser = _currentUser;
        }

        public async Task<TResponse> Handle(TRequest command, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var commandRun = new Command(
                typeof(TRequest).FullName!,
                JsonSerializer.Serialize(command),
                command.JobId ?? throw new InvalidOperationException("JobId can't be null"),
                command.SourceId,
                command.ParentRunId,
                command.StartedByUser,
                command.StartedByUserName);

            await _commandRepo.AddAsync(commandRun);

            command.RunId = commandRun.Id;

            await _events.PublishAsync(new CommandStatusEvent(commandRun.JobId, commandRun.Id, commandRun.SourceId, commandRun.CommandType, CommandStatus.Running));

            try
            {
                var response = await next();

                commandRun.Finalize();
                await _commandRepo.UpdateAsync(commandRun);

                await _events.PublishAsync(new CommandStatusEvent(commandRun.JobId, commandRun.Id, commandRun.SourceId, commandRun.CommandType, CommandStatus.Success));

                return response;
            }
            catch (Exception ex)
            {
                commandRun.Finalize(ex);
                await _commandRepo.UpdateAsync(commandRun);

                await _events.PublishAsync(new CommandStatusEvent(commandRun.JobId, commandRun.Id, commandRun.SourceId, commandRun.CommandType, CommandStatus.Error, ex.Message));

                throw;
            }
        }
    }
}

