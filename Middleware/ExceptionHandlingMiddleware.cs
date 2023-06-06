using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using TreeNodes.Data;
using TreeNodes.Exceptions;
using TreeNodes.Models;

namespace TreeNodes.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ExceptionHandlingMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (SecureException secureException)
            {
                await HandleSecureException(context, secureException, "Secure");
            }
            catch (Exception exception)
            {
                await HandleException(context, exception, "Exception");
            }
        }

        private async Task HandleSecureException(HttpContext context, SecureException secureException, string eventType)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await LogExceptionAsync(dbContext, secureException, context, eventType);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                type = eventType,
                id = secureException.EventId,
                data = new
                {
                    message = secureException.Message
                }
            };

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

        private async Task HandleException(HttpContext context, Exception exception, string eventType)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await LogExceptionAsync(dbContext, exception, context, eventType);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var eventId = Guid.NewGuid().ToString();
            var response = new
            {
                type = eventType,
                id = eventId,
                data = new
                {
                    message = $"Internal server error ID = {eventId}"
                }
            };

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

        private async Task LogExceptionAsync(AppDbContext dbContext, Exception exception, HttpContext context, string eventType)
        {
            var exceptionLog = new ExceptionLog
            {
                EventType = eventType,
                EventId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now,
                QueryParameters = GetQueryParameters(context),
                BodyParameters = await GetBodyParameters(context),
                StackTrace = exception.StackTrace
            };

            dbContext.ExceptionLogs.Add(exceptionLog);
            await dbContext.SaveChangesAsync();
        }

        private string GetQueryParameters(HttpContext context)
        {
            var queryParameters = context.Request.Query
                .ToDictionary(q => q.Key, q => q.Value.ToString());

            return JsonConvert.SerializeObject(queryParameters);
        }

        private async Task<string> GetBodyParameters(HttpContext context)
        {
            context.Request.EnableBuffering();
            using var streamReader = new StreamReader(context.Request.Body, leaveOpen: true);
            var requestBody = await streamReader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            return requestBody;
        }
    }
}
