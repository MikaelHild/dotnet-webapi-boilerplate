using System;
using FSH.WebApi.Application.Common.Commands;
using FSH.WebApi.Domain.Command;

namespace FSH.WebApi.Application.Catalog.Products
{
    public class ChangeProductRatesRequest : IRequest<string>
    {
        public double PercentualChange { get; set; }

        public ChangeProductRatesRequest(double percentualChange)
        {
            PercentualChange = percentualChange;
        }
    }

    public class ChangeProductRatesCommand : ICommand
    {

        public double Percent { get; set; }
        public string? JobId { get; set; }
        public int RunId { get; set; }
        public int? ParentRunId { get; set; }
        public int SourceId { get; set; }
        public CommandStatus Status { get; set; }
        public string StartedByUser { get; set; }
        public string StartedByUserName { get; set; }

        public ChangeProductRatesCommand(double percentage, string userId, string userName)
        {
            Percent = percentage;
            StartedByUser = userId;
            StartedByUserName = userName;
        }


    }

    public class ChangeProductRatesRequestHandler : IRequestHandler<ChangeProductRatesRequest, string>
    {
        private readonly IJobService _jobService;
        private readonly ICurrentUser _currentUser;

        public ChangeProductRatesRequestHandler(IJobService jobService, ICurrentUser currentUser)
        {
            _jobService = jobService;
            _currentUser = currentUser;
        }

        public  Task<string> Handle(ChangeProductRatesRequest request, CancellationToken cancellationToken)
        {
            var jobId = _jobService.EnqueueCommand(
                new ChangeProductRatesCommand(
                    request.PercentualChange,
                    _currentUser.GetUserId().ToString(),
                    _currentUser.Name));

            return Task.FromResult(jobId);
        }
    }
}

