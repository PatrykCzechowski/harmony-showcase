// Context: Central authorization middleware enforcing deny-by-default.
// Problem: Risk of accidentally exposing endpoints when attributes are forgotten.
// Solution: Require explicit Resource/Action metadata else return 403 before controller.
// Value: Shrinks attack surface; makes permission matrix derivable by scanning attributes.

public sealed class ResourceAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResourceAuthorizationMiddleware> _log;

    public ResourceAuthorizationMiddleware(RequestDelegate next, ILogger<ResourceAuthorizationMiddleware> log)
    { _next = next; _log = log; }

    public async Task InvokeAsync(HttpContext ctx, IAuthorizationEvaluator evaluator)
    {
        var endpoint = ctx.GetEndpoint();
        if (endpoint == null) { await _next(ctx); return; }
        if (endpoint.Metadata.GetMetadata<IAllowAnonymous>() != null) { await _next(ctx); return; }
        var metas = endpoint.Metadata.GetOrderedMetadata<ResourceAuthorizationAttribute>();
        if (metas.Count == 0)
        {
            _log.LogWarning("Denied: missing metadata {Path}", ctx.Request.Path);
            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            await ctx.Response.WriteAsJsonAsync(new { error = "MissingAuthorizationMetadata" });
            return;
        }
        foreach (var m in metas)
        {
            var decision = await evaluator.EvaluateAsync(new AuthorizationContext(ctx.User, m.Resource, m.Action, ctx));
            if (decision.Result == AuthorizationResult.Allow)
            { await _next(ctx); return; }
        }
        _log.LogInformation("Denied: no allow decision {Path}", ctx.Request.Path);
        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
        await ctx.Response.WriteAsJsonAsync(new { error = "Denied" });
    }
}
