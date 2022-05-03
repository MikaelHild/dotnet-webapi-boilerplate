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
            _currentUser = currentUser;
        }

        public async Task<TResponse> Handle(TRequest commandRequest, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (commandRequest.JobId == null) throw new InvalidOperationException("JobId can't be null");

            //TODO: Implement CommandSource
            var command = new Command(
                commandType: typeof(TRequest).FullName!,
                commandJson: JsonSerializer.Serialize((TRequest)commandRequest),
                jobId: commandRequest.JobId,
                source: CommandSource.Api,
                parentRunId: commandRequest.ParentCommandId,
                startedByUserId: commandRequest.CreatedByUserId,
                startedByUserName: commandRequest.CreatedByUserName);

            await _commandRepo.AddAsync(command);

            commandRequest.CommandId = command.Id;

            await _events.PublishAsync(new CommandStatusEvent(command.JobId, command.Id, command.Source.ToString(), command.CommandType, CommandStatus.Running));

            try
            {
                var response = await next();

                command.Finalize();
                await _commandRepo.UpdateAsync(command);

                await _events.PublishAsync(new CommandStatusEvent(command.JobId, command.Id, command.Source.ToString(), command.CommandType, CommandStatus.Success));

                return response;
            }
            catch (Exception ex)
            {
                command.Finalize(ex);
                await _commandRepo.UpdateAsync(command);

                await _events.PublishAsync(new CommandStatusEvent(command.JobId, command.Id, command.Source.ToString(), command.CommandType, CommandStatus.Error, ex.Message));

                throw;
            }
        }
    }
}

