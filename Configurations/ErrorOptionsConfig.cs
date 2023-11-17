using FastEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ParkingLot.Configurations;

public static class ErrorOptionsConfig
{
    public static void Configure(this ErrorOptions errorOptions)
    {
        errorOptions.ResponseBuilder = (failures, ctx, statusCode) =>
        {
            return new ValidationProblemDetails(
                failures.GroupBy(f => f.PropertyName)
                    .ToDictionary(
                        keySelector: e => e.Key,
                        elementSelector: e => e.Select(m => m.ErrorMessage).ToArray()))
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred.",
                Status = statusCode,
                Instance = ctx.Request.Path,
                Extensions = { { "traceId", ctx.TraceIdentifier } }
            };
        };
    }
}