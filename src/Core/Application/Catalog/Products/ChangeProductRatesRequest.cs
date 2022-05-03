using System;
using FSH.WebApi.Domain.Command;

namespace FSH.WebApi.Application.Catalog.Products
{
    public class ChangeProductPricesRequest : IRequest<string>
    {
        public double PercentualChange { get; set; }
        public Guid BrandId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public ChangeProductPricesRequest(Guid brandId, double percentualChange, DateTime startTime, DateTime? endTime)
        {
            PercentualChange = percentualChange;
            StartTime = startTime;
            EndTime = endTime;
            BrandId = brandId;
        }
    }

    public class ChangeProductRatesRequestHandler : IRequestHandler<ChangeProductPricesRequest, string>
    {
        private readonly IJobService _jobService;
        private readonly ICurrentUser _currentUser;

        public ChangeProductRatesRequestHandler(IJobService jobService, ICurrentUser currentUser)
        {
            _jobService = jobService;
            _currentUser = currentUser;
        }

        public  Task<string> Handle(ChangeProductPricesRequest request, CancellationToken cancellationToken)
        {

            var jobId = _jobService.EnqueueCommand(
                new ChangeProductRatesCommand(
                    request.PercentualChange,
                    request.BrandId,
                    _currentUser.GetUserId().ToString(),
                    _currentUser.Name));

            if (request.EndTime.HasValue)
            {

                var futureJobId = _jobService.EnqueueCommandAt(
                    new ChangeProductRatesCommand(
                        percentage: request.PercentualChange,
                        brandId: request.BrandId,
                        userId: _currentUser.GetUserId().ToString(),
                        userName: _currentUser.Name,
                        revertChange: true
                        ),
                        request.EndTime.Value);
                
            }

            return Task.FromResult(jobId);
        }
    }
}

