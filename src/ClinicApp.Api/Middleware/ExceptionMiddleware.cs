using ClinicApp.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace ClinicApp.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            // Domain kuralı ihlali — 400 Bad Request
            _logger.LogWarning("Domain exception: {Message}", ex.Message);
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (DataCorruptionException ex)
        {
            // Veri bütünlüğü sorunu — 500 Internal Server Error (kullanıcı hatası değil)
            _logger.LogCritical("Data corruption detected: {Message}", ex.Message);
            await WriteErrorResponse(context, HttpStatusCode.InternalServerError,
                "Bir sistem hatası oluştu. Lütfen daha sonra tekrar deneyiniz.");
        }
        catch (Exception ex)
        {
            // Beklenmeyen her şey — 500
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorResponse(context, HttpStatusCode.InternalServerError,
                "Beklenmeyen bir hata oluştu.");
        }
    }

    private static async Task WriteErrorResponse(
        HttpContext context,
        HttpStatusCode statusCode,
        string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var body = JsonSerializer.Serialize(new { error = message });
        await context.Response.WriteAsync(body);
    }
}
