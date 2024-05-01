using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;
using OpenFlier.Plugin;
using PuppeteerSharp;
using System.IO;
using System.Text;

namespace OpenFlierWebshot
{
    public class WebshotPlugin : ICommandInputPlugin
    {
        const string MSEDGE_64_PATH = "C:\\Program Files\\Microsoft\\Edge\\Application\\msedge.exe";
        const string MSEDGE_PATH = "C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe";
        const string CHROME_64_PATH = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
        const string CHROME_PATH = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
        public CommandInputPluginInfo GetPluginInfo()
            => new CommandInputPluginInfo()
            {
                InvokeCommands = new[] { "aws", "ws", "bs" },
                PluginAuthor = "djdjz7",
                PluginDescription = "Takes a screenshot of a webpage.",
                PluginIdentifier = "openflier.djdjz7.webshot",
                PluginName = "OpenFlier Webshot",
                PluginNeedsAdminPrivilege = false,
                PluginNeedsConfigEntry = true,
                PluginVersion = "2.0.0",
            };

        public async Task PluginMain(CommandInputPluginArgs args)
        {
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
            new ConfigWindow().ShowDialog();
        }

        public async Task TakeWebshot(string filename, string url, int? width = null, int? height = null)
        {
            var path = GetBrowserPath();
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = path,
            });
            var page = await browser.NewPageAsync();
            await page.GoToAsync(url, 0, new WaitUntilNavigation[] { WaitUntilNavigation.Networkidle2, WaitUntilNavigation.Load });
            if (width != null && height != null)
            {
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = (int)width,
                    Height = (int)height,
                });
                await page.ScreenshotAsync(filename);
            }
            else
            {
                await page.ScreenshotAsync(filename, new ScreenshotOptions { FullPage = true });
            }
            await browser.CloseAsync();
        }

        public async Task BeforeExit()
        {
            return;
        }

        public string GetBrowserPath()
        {
            if (File.Exists("Plugins\\openflier.djdjz7.webshot\\browserExecutablePath"))
            {
                string path = File.ReadAllText("Plugins\\openflier.djdjz7.webshot\\browserExecutablePath");
                if (!File.Exists(path))
                    throw new FileNotFoundException("browserExecutablePath 文件指向的路径不存在。");
                return path;
            }
            else if (File.Exists(CHROME_64_PATH))
                return CHROME_64_PATH;
            else if (File.Exists(CHROME_PATH))
                return CHROME_PATH;
            else if (File.Exists(MSEDGE_64_PATH))
                return MSEDGE_64_PATH;
            else if (File.Exists(MSEDGE_PATH))
                return MSEDGE_PATH;
            else
                throw new FileNotFoundException("未找到浏览器路径。请在插件目录下创建 browserExecutablePath 文件并写入浏览器路径。");
                
        }
    }
}
