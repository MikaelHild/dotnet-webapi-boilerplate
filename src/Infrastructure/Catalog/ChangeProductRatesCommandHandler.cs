using FSH.WebApi.Application.Catalog.Brands;
using FSH.WebApi.Application.Catalog.Products;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Domain.Catalog;
using FSH.WebApi.Shared.Notifications;
using Hangfire.Console.Extensions;
using Hangfire.Console.Progress;
using Hangfire.Server;
using MediatR;

namespace FSH.WebApi.Infrastructure.Catalog;

public class ChangeProductRatesCommandHandler : IRequestHandler<ChangeProductRatesCommand>
{
    private readonly ISender _mediator;
    private readonly IProgressBarFactory _progressBar;
    private readonly PerformingContext _performingContext;
    private readonly INotificationSender _notifications;
    private readonly ICurrentUser _currentUser;
    private readonly IProgressBar _progress;
    private readonly IRepository<Product> _repository;

    public ChangeProductRatesCommandHandler(
        ISender mediator,
        IProgressBarFactory progressBar,
        PerformingContext performingContext,
        INotificationSender notifications,
        ICurrentUser currentUser,
        IRepository<Product> repository)
    {
        _mediator = mediator;
        _progressBar = progressBar;
        _performingContext = performingContext;
        _notifications = notifications;
        _currentUser = currentUser;
        _progress = _progressBar.Create();
        _repository = repository;
    }

    public async Task<Unit> Handle(ChangeProductRatesCommand request, CancellationToken cancellationToken)
    {

        await NotifyAsync("Your job processing has started", 0, cancellationToken);

        var products = await _repository.ListAsync();

        /* Products could be updated directly using the repository.
         * 
        foreach (var product in products)
        {
            product.Rate = product.Rate * request.Percent;
        }

        await _repository.SaveChangesAsync(cancellationToken);
        */

        int index = 0;
        foreach (var product in products)
        {
            await _mediator.Send(
                new UpdateProductRequest
                {
                    Id = product.Id,
                    Name = product.Name,
                    Rate = product.Rate * (decimal)request.Percent,
                    BrandId = product.BrandId,
                    DeleteCurrentImage = false,
                });

            index++;

            await NotifyAsync("Progress: ", products.Count > 0 ? (index * 100 / products.Count) : 0, cancellationToken);
        }

        await NotifyAsync("Job successfully completed", 100, cancellationToken);

        return Unit.Value;
    }

    private async Task NotifyAsync(string message, int progress, CancellationToken cancellationToken)
    {
        _progress.SetValue(progress);
        await _notifications.SendToUserAsync(
            new JobNotification()
            {
                JobId = _performingContext.BackgroundJob.Id,
                Message = message,
                Progress = progress
            },
            _currentUser.GetUserId().ToString(),
            cancellationToken);
    }
}