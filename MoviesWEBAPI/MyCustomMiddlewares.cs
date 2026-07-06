namespace MoviesWEBAPI;

public class FirstCustomMiddleware
{
    private readonly RequestDelegate _next;

    public FirstCustomMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine("FirstCustomMiddleware Request received");

        await _next(context);

        Console.WriteLine("FirstCustomMiddleware Response sent");
    }
}


public class SecondCustomMiddleware
{
    private readonly RequestDelegate _next;

    public SecondCustomMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine("SecondCustomMiddleware Request received");

        await _next(context);

        Console.WriteLine("SecondCustomMiddleware Response sent");
    }
}


public class LogRequestMiddleware
{
    private readonly RequestDelegate _next;

    public LogRequestMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine("Request started ");

        await _next(context);

        Console.WriteLine("Request ending ");
    }
}