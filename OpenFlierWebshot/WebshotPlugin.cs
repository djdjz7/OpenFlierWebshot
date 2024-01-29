using OpenFlier.Plugin;
using Microsoft.Playwright;

namespace OpenFlierWebshot
{
    public class WebshotPlugin : ICommandInputPlugin
    {
        public static IPlaywright? PlaywrightEnv { get; set; }

        public CommandInputPluginInfo GetPluginInfo()
            => new CommandInputPluginInfo()
            {
                InvokeCommands = new[] { "aws", "ws" },
                PluginAuthor = "djdjz7",
                PluginDescription = "Takes a screenshot of a webpage.",
                PluginIdentifier = "openflier.djdjz7.webshot",
                PluginName = "OpenFlier Webshot",
                PluginNeedsAdminPrivilege = false,
                PluginNeedsConfigEntry = false,
                PluginVersion = "1.0.0",
            };

        public async Task PluginMain(CommandInputPluginArgs args)
        {
            if (PlaywrightEnv is null)
                PlaywrightEnv = await Playwright.CreateAsync();
        }

        public void PluginOpenConfig()
        {
            throw new NotImplementedException();
        }
    }
}
