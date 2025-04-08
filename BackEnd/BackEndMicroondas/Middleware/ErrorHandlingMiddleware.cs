namespace BackEndMicroondas.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Registra a exception com detalhes (stacktrace, inner exception, etc.)
                File.AppendAllText("logs.txt", $"{DateTime.Now}: {ex}\n");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Ocorreu um erro. Tente novamente mais tarde.");
            }
        }
    }
}
