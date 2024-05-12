using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.TranscodeKiller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.MediaEncoding;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Session;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.TranscodeKiller;

/// <summary>
/// Plugin main process.
/// </summary>
public class KillerEntrypoint : IHostedService
{
    private readonly ISessionManager _sessionManager;
    private readonly ITranscodeManager _transcodeManager;
    private readonly ILogger<KillerEntrypoint> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="KillerEntrypoint"/> class.
    /// </summary>
    /// <param name="sessionManager">Instance of the <see cref="ISessionManager"/> interface.</param>
    /// <param name="logger">Instance of the <see cref="ILogger{Entrypoint}"/> interface.</param>
    /// <param name="transcodeManager">Instance of the <see cref="ITranscodeManager"/>.</param>
    public KillerEntrypoint(ISessionManager sessionManager, ILogger<KillerEntrypoint> logger, ITranscodeManager transcodeManager)
    {
        _sessionManager = sessionManager;
        _logger = logger;
        _transcodeManager = transcodeManager;
    }

    private static PluginConfiguration PluginConfiguration => TranscodeKillerPlugin.Instance!.Configuration;

    private async void PlaybackProgressHandler(object? sender, PlaybackProgressEventArgs e)
    {
        /*
         * Only kill processes that are videos with over the configured width or height.
         */
        if (e.Session.PlayState.PlayMethod is not PlayMethod.Transcode
            || e.Item is not Video
            || (e.Item.Width <= PluginConfiguration.MaxWidth
                && e.Item.Height <= PluginConfiguration.MaxHeight))
        {
            return;
        }

        _logger.LogWarning(
            "Killing transcode process for session {SessionId} for user {UserId}",
            e.PlaySessionId,
            e.Session.UserId);

        await _transcodeManager.KillTranscodingJobs(
                e.Session.DeviceId,
                e.PlaySessionId,
                _ => true)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _sessionManager.PlaybackProgress += PlaybackProgressHandler;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _sessionManager.PlaybackProgress -= PlaybackProgressHandler;
        return Task.CompletedTask;
    }
}
