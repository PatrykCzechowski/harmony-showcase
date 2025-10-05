// Context: Starting managed FFmpeg process for low-latency HLS.
// Problem: Ad-hoc processes leave stale segments & race UI initial playlist load.
// Solution: Wrapper ensures directory prepared, stale artifacts cleaned, waits until playlist appears.
// Value: Reliable startup, reduced latency, controlled disk footprint.

public sealed class StreamingService
{
    private readonly string _root;
    private readonly ILogger<StreamingService> _log;
    private readonly ConcurrentDictionary<string, Process> _procs = new();

    public StreamingService(IWebHostEnvironment env, ILogger<StreamingService> log)
    { _root = Path.Combine(env.WebRootPath, "streams"); _log = log; }

    public async Task<string> StartAsync(string cameraId, string rtspUrl, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cameraId);
        ArgumentException.ThrowIfNullOrWhiteSpace(rtspUrl);
        
        var streamDirectory = Path.Combine(_root, cameraId);
        Directory.CreateDirectory(streamDirectory);
        CleanupStaleFiles(streamDirectory);
        
        var process = StartFfmpegProcess(rtspUrl, streamDirectory);
        _procs[cameraId] = process;
        
        _ = Task.Run(() => MonitorFfmpegLogs(cameraId, process), ct);
        await WaitForPlaylistReady(streamDirectory, ct);
        
        return $"/stream/files/{cameraId}/index.m3u8";
    }
    
    private static Process StartFfmpegProcess(string rtspUrl, string outputDirectory)
    {
        var args = BuildFfmpegArguments(rtspUrl, outputDirectory);
        var startInfo = new ProcessStartInfo("ffmpeg", args)
        {
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        var process = Process.Start(startInfo);
        return process ?? throw new InvalidOperationException("Failed to start FFmpeg process");
    }
    
    private static string BuildFfmpegArguments(string rtspUrl, string outputDirectory)
    {
        const int segmentDuration = 2;
        const int playlistSize = 5;
        
        return $"-rtsp_transport tcp -i \"{rtspUrl}\" " +
               $"-codec copy -an -f hls " +
               $"-hls_time {segmentDuration} -hls_list_size {playlistSize} " +
               $"-hls_flags delete_segments+append_list " +
               $"-hls_segment_filename \"{outputDirectory}/seg_%03d.ts\" " +
               $"\"{outputDirectory}/index.m3u8\"";
    }

    private static void CleanupStaleFiles(string directory)
    {
        var staleSegments = Directory.GetFiles(directory, "*.ts");
        var stalePlaylists = Directory.GetFiles(directory, "*.m3u8");
        
        foreach (var file in staleSegments.Concat(stalePlaylists))
        {
            try { File.Delete(file); }
            catch (IOException) { /* File might be in use, ignore */ }
        }
    }

    private async Task WaitForPlaylistReady(string directory, CancellationToken ct)
    {
        const int timeoutSeconds = 10;
        const int pollIntervalMs = 250;
        
        var playlistPath = Path.Combine(directory, "index.m3u8");
        var start = DateTime.UtcNow;
        
        while (!File.Exists(playlistPath))
        {
            ct.ThrowIfCancellationRequested();
            
            if ((DateTime.UtcNow - start).TotalSeconds > timeoutSeconds)
                throw new TimeoutException($"Playlist not ready after {timeoutSeconds}s");
                
            await Task.Delay(pollIntervalMs, ct);
        }
    }

    private async Task MonitorFfmpegLogs(string cameraId, Process process)
    {
        try
        {
            while (!process.HasExited)
            {
                var line = await process.StandardError.ReadLineAsync();
                if (string.IsNullOrEmpty(line))
                    break;
                    
                if (line.Contains("error", StringComparison.OrdinalIgnoreCase))
                    _log.LogWarning("FFmpeg[{CameraId}]: {ErrorLine}", cameraId, line);
            }
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error monitoring FFmpeg logs for camera {CameraId}", cameraId);
        }
    }
}
