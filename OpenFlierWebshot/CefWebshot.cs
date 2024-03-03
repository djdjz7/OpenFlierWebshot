using CefSharp;
using CefSharp.DevTools.Page;
using CefSharp.OffScreen;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace CloudRoom.MqttModule
{
    public class CefWebshot
    {
        public static async Task<Bitmap> TakeWebshot(string url,int? width, int? height)
        {
            if (string.IsNullOrWhiteSpace(url))
                return new Bitmap(Stream.Null);
            try
            {
                var browser = new ChromiumWebBrowser(url);
                await browser.WaitForInitialLoadAsync();
                var cefbrowserHost = browser.GetBrowserHost();
                if (url.Contains("moegirl.org.cn"))
                {
                    browser.ExecuteScriptAsync(@"window.localStorage.setItem(""moeskin:sitenotice-lastview-content"",""萌娘百科衷心希望新冠感染疫情早日结束！关于萌娘百科站内广告的说明为了拯救萌百，维持运营开发和服务器运转，我们近期在站内添加了一批广告。同时，在引入了“延伸确认用户”用户组后，我们也为组内的用户免除了绝大部分广告。……由于此次涉及到多个广告发布商，且各个供应商提供的广告种类较多且杂，对于广告所展示的内容，还请大家仔细甄别。对于突如其来的弹窗等广告，可选择直接关闭。……感谢大家一直以来对萌娘百科的支持与理解。阅读更多公告关于技术性故障的公告讨论中事项设立分类指引与讨论专案的提案"")");
                    browser.ExecuteScriptAsync(@"window.localStorage.setItem(""moeskin:sitenotice/last-viewed"",""萌娘百科会员服务(试行)公告登录用户即可免费领取会员服务公告关于全站半保护的公告关于萌娘百科近期改革的公告近期人事调整规则及其变动之公告关于萌娘百科政策更改事宜的公告讨论中事项2023年政策修订增补专案关于软硬件收录范围的修正案"")");
                }
                cefbrowserHost.Invalidate(PaintElementType.View);
                var contentSize = await browser.GetContentSizeAsync();
                var viewport = new Viewport();
                if (width == null || height == null)
                {
                    viewport = new Viewport
                    {
                        Height = contentSize.Height,
                        Width = contentSize.Width,
                        Scale = 1.0
                    };
                }
                else
                {
                    viewport = new Viewport
                    {
                        Height = (double)height,
                        Width = (double)width,
                        Scale = 1.0
                    };
                }
                var buffer = await browser.CaptureScreenshotAsync(CaptureScreenshotFormat.Png, 100, viewport);
                var ms = new MemoryStream(buffer);
                Bitmap bitmap = new Bitmap(ms);
                if (!browser.IsDisposed)
                    browser.Dispose();
                return bitmap;
            }
            catch (Exception ex)
            {
                return new Bitmap(Stream.Null);
            }
        }
    }
}
