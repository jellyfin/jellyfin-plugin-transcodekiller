<h1 align="center">Jellyfin Transcode Killer Plugin</h1>
<h3 align="center">Part of the <a href="https://jellyfin.org">Jellyfin Project</a></h3>

<p align="center">

<br/>
<a href="https://github.com/jellyfin/jellyfin-plugin-transcodekiller/actions?query=workflow%3A%22Build+Plugin%22">
<img alt="GitHub Workflow Status" src="https://img.shields.io/github/workflow/status/jellyfin/jellyfin-plugin-transcodekiller/Build%20Pluginn.svg">
</a>
<a href="https://github.com/jellyfin/jellyfin-plugin-transcodekiller">
<img alt="GPLv3 License" src="https://img.shields.io/github/license/jellyfin/jellyfin-plugin-transcodekiller.svg"/>
</a>
<a href="https://github.com/jellyfin/jellyfin-plugin-transcodekiller/releases">
<img alt="Current Release" src="https://img.shields.io/github/release/jellyfin/jellyfin-plugin-transcodekiller.svg"/>
</a>
</p>

<p>
  Plugin prevents Jellyfin from transcoding videos when the source resolution exceeds set limits. By configuring max width/height, you can:
  <ul>
    <li>Block transcoding entirely (e.g., 1x1 limit).</li>
    <li>Block heavy transcodes like 4K → 1080p.</li>
  </ul>
</p>
<p>
  Since Jellyfin can’t fully disable transcoding by default, the plugin is useful for low-power servers or CPU-only setups, ensuring users don’t overload the system with demanding transcodes.
</p>
