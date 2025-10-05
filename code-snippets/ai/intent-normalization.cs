// Context: Normalizing free-form user prompt into structured intent for orchestration.
// Problem: Directly binding model output to actions risks fragility & vendor lock-in.
// Solution: Intermediate representation (Intent { Type, Targets, Parameters, Confidence, IsActionable })
// Value: Provider swap agility, explicit safety checks, explainability (provenance + confidence).

public sealed class Intent
{
    public required string Type { get; init; } // e.g. "DeviceCommand", "SceneTrigger", "InfoQuery", "RuleTest"
    public string[] Targets { get; init; } = Array.Empty<string>();
    public Dictionary<string, string> Parameters { get; init; } = new();
    public double Confidence { get; init; }
    public bool IsActionable { get; init; }
    public string? RawModelHint { get; init; } // traceability
}

public interface IIntentNormalizer
{
    Intent Normalize(AiModelResult modelResult);
}

public sealed class PatternIntentNormalizer : IIntentNormalizer
{
    private static readonly Regex DeviceCommandPattern = new(@"(?i)turn (on|off) (?<device>.+)", RegexOptions.Compiled);
    private static readonly Regex SceneTriggerPattern = new(@"(?i)activate scene (?<scene>.+)", RegexOptions.Compiled);
    
    private const double PatternMatchConfidenceBoost = 0.1;
    private const double FallbackConfidenceMultiplier = 0.7;
    private const double MinimumFallbackConfidence = 0.2;

    public Intent Normalize(AiModelResult modelResult)
    {
        ArgumentNullException.ThrowIfNull(modelResult);
        
        var normalizedText = modelResult.Text.Trim();
        
        return TryMatchDeviceCommand(normalizedText, modelResult) 
            ?? TryMatchSceneTrigger(normalizedText, modelResult) 
            ?? CreateFallbackIntent(modelResult);
    }
    
    private Intent? TryMatchDeviceCommand(string text, AiModelResult modelResult)
    {
        var match = DeviceCommandPattern.Match(text);
        if (!match.Success) return null;
        
        var deviceName = match.Groups["device"].Value.Trim();
        var action = match.Groups[1].Value.ToLowerInvariant();
        var enhancedConfidence = modelResult.Confidence * (1 - PatternMatchConfidenceBoost) + PatternMatchConfidenceBoost;
        
        return new Intent
        {
            Type = "DeviceCommand",
            Targets = [deviceName],
            Parameters = new Dictionary<string, string> { ["action"] = action },
            Confidence = enhancedConfidence,
            IsActionable = true,
            RawModelHint = modelResult.ModelName
        };
    }
    
    private Intent? TryMatchSceneTrigger(string text, AiModelResult modelResult)
    {
        var match = SceneTriggerPattern.Match(text);
        if (!match.Success) return null;
        
        var sceneName = match.Groups["scene"].Value.Trim();
        
        return new Intent
        {
            Type = "SceneTrigger",
            Targets = [sceneName],
            Confidence = modelResult.Confidence,
            IsActionable = true,
            RawModelHint = modelResult.ModelName
        };
    }
    
    private Intent CreateFallbackIntent(AiModelResult modelResult)
    {
        var adjustedConfidence = Math.Max(
            MinimumFallbackConfidence, 
            modelResult.Confidence * FallbackConfidenceMultiplier);
            
        return new Intent
        {
            Type = "InfoQuery",
            Confidence = adjustedConfidence,
            IsActionable = false,
            RawModelHint = modelResult.ModelName
        };
    }
}

public sealed class AiModelResult
{
    public required string Text { get; init; }
    public required double Confidence { get; init; }
    public required string ModelName { get; init; }
}
