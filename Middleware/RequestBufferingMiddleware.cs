namespace TreeNodes.Middleware
{
    public class RequestBufferingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestBufferingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();
            await _next(context);
        }
    }
}
