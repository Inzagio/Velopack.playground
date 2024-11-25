using Velopack;

namespace Playground.Api.Deployment;

internal static class ApplicationUpdate
{
    public static async Task UpdateMyApp()
    {
        var mgr = new UpdateManager("D:\\VeloUpdates");

        // check for new version
        var newVersion = await mgr.CheckForUpdatesAsync();
        if (newVersion == null)
            return; // no update available

        // download new version
        await mgr.DownloadUpdatesAsync(newVersion);

        // install new version and restart app
        mgr.ApplyUpdatesAndRestart(newVersion);
    }

}