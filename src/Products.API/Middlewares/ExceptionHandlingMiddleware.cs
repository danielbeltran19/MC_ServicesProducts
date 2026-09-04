using System.Net;
using Microsoft.AspNetCore.Mvc;
using Products.Domain.Exceptions;

namespace Products.API.Middlewares;

/// <summary>
/// Traduce excepciones de dominio a respuestas HTTP semánticas usando el
/// formato estándar ProblemDetails, en lugar de dejar que el error de
/// negocio se filtre como un 500 genérico.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (ProductNotFoundException ex)
        {
            await WriteProblemAsync(context, HttpStatusCode.NotFound, "Recurso no encontrado", ex.Message);
        }
        catch (InsufficientStockException ex)
        {
            await WriteProblemAsync(context, HttpStatusCode.BadRequest, "Stock insuficiente", ex.Message);
        }
        catch (DomainException ex)
        {
            // InvalidStockOperationException, InvalidProductDataException, etc.
            await WriteProblemAsync(context, HttpStatusCode.BadRequest, "Solicitud inválida", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado procesando {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblemAsync(context, HttpStatusCode.InternalServerError, "Error interno del servidor",
                "Ocurrió un error inesperado. Intenta nuevamente más tarde.");
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, HttpStatusCode statusCode, string title, string detail)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}