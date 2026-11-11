namespace Modsen.Portal.TimeTrack.UseCases.Options;

public class ReportProcessingOptions
{
    public string? QueueName { get; set; }
    public int FileAccessibilityInSeconds { get; set; }
    public int ReportGenerationTimeoutInSeconds { get; set;}
}
