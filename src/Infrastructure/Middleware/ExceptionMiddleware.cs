using System.Net;
using FSH.WebApi.Application.Common.Exceptions;
using FSH.WebApi.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Serilog;
using Serilog.Context;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FSH.WebApi.Infrastructure.Middleware;

internal class ExceptionMiddleware : IMiddleware
{
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;
    private readonly ISerializerService _jsonSerializer;

    public ExceptionMiddleware(
        ICurrentUser currentUser,
        IStringLocalizer<ExceptionMiddleware> localizer,
        ISerializerService jsonSerializer)
    {
        _currentUser = currentUser;
        _t = localizer;
        _jsonSerializer = jsonSerializer;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var response = context.Response;

            if (exception is ValidationException ex && !response.HasStarted)
            {
                // short-cut for validation exceptions
                response.ContentType = "application/json";
                response.StatusCode = 400;
                var problemDetails = new ValidationProblemDetails(
                    ex.Errors.GroupBy(e => e.PropertyName).ToDictionary(e => e.Key, e => e.Select(e => e.ErrorMessage).ToArray()));
                await response.WriteAsync(JsonSerializer.Serialize(problemDetails));
                return;
            }

            string email = _currentUser.GetUserEmail() is string userEmail ? userEmail : "Anonymous";
            var userId = _currentUser.GetUserId();
            string tenant = _currentUser.GetTenant() ?? string.Empty;
            if (userId != Guid.Empty) LogContext.PushProperty("UserId", userId);
            LogContext.PushProperty("UserEmail", email);
            if (!string.IsNullOrEmpty(tenant)) LogContext.PushProperty("Tenant", tenant);
            string errorId = Guid.NewGuid().ToString();
            LogContext.PushProperty("ErrorId", errorId);
            LogContext.PushProperty("StackTrace", exception.StackTrace);
            var errorResult = new ErrorResult
            {
                Source = exception.TargetSite?.DeclaringType?.FullName,
                Exception = exception.Message.Trim(),
                ErrorId = errorId,
                SupportMessage = _t["Provide the ErrorId {0} to the support team for further analysis.", errorId]
            };
            errorResult.Messages.Add(exception.Message);
            if (exception is not CustomException && exception.InnerException != null)
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
            }

            switch (exception)
            {
                case CustomException e:
                    errorResult.StatusCode = (int)e.StatusCode;
                    if (e.ErrorMessages is not null)
                    {
                        errorResult.Messages = e.ErrorMessages;
                    }

                    break;

                case KeyNotFoundException:
                    errorResult.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    errorResult.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            Log.Error($"{errorResult.Exception} Request failed with Status Code {response.StatusCode} and Error Id {errorId}.");
            if (!response.HasStarted)
            {
                response.ContentType = "application/json";
                response.StatusCode = errorResult.StatusCode;
                await response.WriteAsync(_jsonSerializer.Serialize(errorResult));
            }
            else
            {
                Log.Warning("Can't write error response. Response has already started.");
            }
        }
    }
}