using System;
using System.Threading.Tasks;
using Jellyfin.Api.Helpers;
using Jellyfin.Plugin.TranscodeKiller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Session;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.TranscodeKiller;

/// <summary>
/// Plugin main process.
/// </summary>
public class KillerEntrypoint : IServerEntryPoint
{
    private readonly ISessionManager _sessionManager;
    private readonly TranscodingJobHelper _transcodingJobHelper;
    private readonly ILogger<KillerEntrypoint> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="KillerEntrypoint"/> class.
    /// </summary>
    /// <param name="sessionManager">Instance of the <see cref="ISessionManager"/> interface.</param>
    /// <param name="logger">Instance of the <see cref="ILogger{Entrypoint}"/> interface.</param>
    /// <param name="transcodingJobHelper">Instance of the <see cref="TranscodingJobHelper"/>.</param>
    public KillerEntrypoint(ISessionManager sessionManager, ILogger<KillerEntrypoint> logger, TranscodingJobHelper transcodingJobHelper)
    {
        _sessionManager = sessionManager;
        _logger = logger;
        _transcodingJobHelper = transcodingJobHelper;
    }

    private static PluginConfiguration PluginConfiguration => TranscodeKillerPlugin.Instance!.Configuration;

    /// <inheritdoc />
    public Task RunAsync()
    {
        _sessionManager.PlaybackProgress += PlaybackProgressHandler;

        return Task.CompletedTask;
    }

    private async void PlaybackProgressHandler(object? sender, PlaybackProgressEventArgs e)
    {
        /*
         * Only kill processes that are videos with over the configured width or height.
         */
        if (e.Session.PlayState.PlayMethod is not PlayMethod.Transcode
            || e.Item is not Video
            || e.Item.Width <= PluginConfiguration.MaxWidth
            || e.Item.Height <= PluginConfiguration.MaxHeight)
        {
            return;
        }

        _logger.LogWarning(
            "Killing transcode process for session {SessionId} for user {UserId}",
            e.PlaySessionId,
            e.Session.UserId);

        await _transcodingJobHelper.KillTranscodingJobs(
                e.Session.DeviceId,
                e.PlaySessionId,
                _ => true)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose managed objects.
    /// </summary>
    /// <param name="disposing">Whether to dispose.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sessionManager.PlaybackProgress -= PlaybackProgressHandler;
        }
    }
}
