using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.TranscodeKiller.Configuration;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Gets or sets the max width allowed to transcode.
    /// </summary>
    public int MaxWidth { get; set; } = 1920;

    /// <summary>
    /// Gets or sets the max height allowed to transcode.
    /// </summary>
    public int MaxHeight { get; set; } = 1080;

    /// <summary>
    /// Gets or sets the message shown to users whenever their session gets killed.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
