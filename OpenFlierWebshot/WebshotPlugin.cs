using OpenFlier.Plugin;
using CloudRoom.MqttModule;
using System.Drawing.Imaging;
using System.Drawing;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using CefSharp;
using CefSharp.OffScreen;

namespace OpenFlierWebshot
{
    public class WebshotPlugin : ICommandInputPlugin
    {
        public CommandInputPluginInfo GetPluginInfo()
            => new CommandInputPluginInfo()
            {
                InvokeCommands = new[] { "aws", "ws", "bs" },
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
            if (!Cef.IsInitialized)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Cef.Initialize(new CefSettings() { BackgroundColor = 0x00ffffff, Locale = "zh-CN" });
                    ;
                });
            }
            var filename = Guid.NewGuid().ToString("N") + (args.UsePng ? ".png" : ".jpeg");
            var fullcmd = args.FullCommand;
            switch (args.InvokeCommand.ToLower())
            {
                case "ws":
                    if (fullcmd.Split(' ').Length == 1)
                        await TakeWebshot($"Screenshots\\{filename}", "https://bilibili.com");
                    else
                        await TakeWebshot($"Screenshots\\{filename}", fullcmd.Split(' ')[1]);
                    break;

                case "aws":
                    if (fullcmd.Split(' ').Length <= 3)
                        throw new Exception("缺少分辨率");
                    else
                    {
                        string[] cmdlist = fullcmd.Split(' ');
                        int width = int.Parse(cmdlist[1]);
                        int height = int.Parse(cmdlist[2]);
                        string url = string.Join(' ', cmdlist, 3, cmdlist.Length - 3);
                        await TakeWebshot($"Screenshots\\{filename}", url, width, height);
                    }
                    break;

                case "bs":
                    if (fullcmd.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length <= 1)
                        throw new Exception("必须输入搜素内容");
                    string searchContent = string.Join(' ', fullcmd.Split(' ', StringSplitOptions.RemoveEmptyEntries), 1, fullcmd.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length - 1);
                    await TakeWebshot($"Screenshots\\{filename}", $"https://bing.com/search?q={Uri.EscapeDataString(searchContent)}", 1920, 2400);
                    break;
            }

            string payload = JsonConvert.SerializeObject(
                new
                {
                    type = 20005L,
                    data = new
                    {
                        name = filename,
                        deviceCode = args.MachineIdentifier,
                        versionCode = args.Version,
                    }
                }
            );
            await args.MqttServer.PublishAsync(
                new MqttApplicationMessage
                {
                    Topic = args.ClientID + "/REQUEST_SCREEN_CAPTURE",
                    Payload = Encoding.Default.GetBytes(payload),
                    QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce
                }
            );
        }

        public void PluginOpenConfig()
        {
            throw new NotImplementedException();
        }

        public async static Task TakeWebshot(string filename, string url, int? width = null, int? height = null)
        {

            Bitmap WebScreenshotBitmap = await CefWebshot.TakeWebshot(url, width, height);
            if (filename.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                WebScreenshotBitmap.Save(filename, ImageFormat.Png);
            else
                WebScreenshotBitmap.Save(filename, ImageFormat.Jpeg);
        }

        public async Task BeforeExit()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Cef.Shutdown();
            });
        }
    }
}
