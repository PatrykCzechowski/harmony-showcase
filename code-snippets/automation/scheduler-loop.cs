// Context: Deterministic automation engine main loop.
// Problem: Inconsistent timing & duplicate executions when using simple timers.
// Solution: Align loop to minute boundary + debounce per-rule minute slot.
// Value: Predictable evaluation, simpler reasoning & auditing.

public sealed class ScheduledAutomationHostedService : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<ScheduledAutomationHostedService> _log;

    public ScheduledAutomationHostedService(IServiceProvider sp, ILogger<ScheduledAutomationHostedService> log)
    { _sp = sp; _log = log; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DelayUntilNextMinute(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await ProcessAsync(stoppingToken); }
            catch (Exception ex) { _log.LogWarning(ex, "automation iteration failed"); }
            await DelayUntilNextMinute(stoppingToken);
        }
    }

    private static async Task DelayUntilNextMinute(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var ms = 60000 - (now.Second * 1000 + now.Millisecond);
        if (ms < 0) ms = 0;
        await Task.Delay(ms, ct);
    }

    private async Task ProcessAsync(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IAutomationRuleQuery>();
        var evaluators = scope.ServiceProvider.GetServices<IAutomationTriggerEvaluator>().ToList();
        var executors = scope.ServiceProvider.GetServices<IAutomationActionExecutor>().ToList();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var now = DateTime.UtcNow;
        
        var rules = await repo.GetEnabledBatchAsync(0, 200, ct);
        foreach (var rule in rules)
        {
            var evaluator = evaluators.FirstOrDefault(e => e.TriggerType == rule.TriggerType);
            if (evaluator is null || !evaluator.IsMatch(rule, now))
                continue;
                
            if (IsAlreadyExecutedThisMinute(rule, now))
                continue;
                
            await ExecuteRuleActions(rule, executors, ct);
            
            rule.LastExecutedAt = now;
            await uow.SaveChangesAsync(ct);
        }
    }
    
    private bool IsAlreadyExecutedThisMinute(IAutomationRule rule, DateTime now)
    {
        return rule.LastExecutedAt.HasValue && SameMinute(rule.LastExecutedAt.Value, now);
    }
    
    private static async Task ExecuteRuleActions(IAutomationRule rule, IReadOnlyList<IAutomationActionExecutor> executors, CancellationToken ct)
    {
        var activeActions = rule.Actions.Where(a => a.IsActive);
        foreach (var action in activeActions)
        {
            var executor = executors.FirstOrDefault(x => x.CanHandle(action.CommandType));
            if (executor != null)
                await executor.ExecuteAsync(action, ct);
        }
    }

    private static bool SameMinute(DateTime a, DateTime b) => a.Year == b.Year && a.Month == b.Month && a.Day == b.Day && a.Hour == b.Hour && a.Minute == b.Minute;
}
