using System;
using System.Linq;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Windows.Forms;

namespace WallpaperHubConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Selected wallpaper resolution for your device is: " + ResolutionSelector() + "\n\nPlease check the wallpaper code (in Url) that you would like to set for your background. Got it? good!\n\nNow please enter the wallpaper code here:");
            string wallpaperCode = Console.ReadLine();
            Console.WriteLine("\nAwesome, Now wait while we do some magic behind the scene!");
            WallpaperSelector(wallpaperCode);
        }

        public static HtmlAgilityPack.HtmlDocument UserWallpaperPage(string wallpaperCode)
        {
            // set the html var to the website link to parse
            var html = @"https://wallpaperhub.app/wallpapers/" + wallpaperCode;

            //gets the website html from the HTTP
            HtmlWeb web = new HtmlWeb();

            //Creates a var called document to load the wesite in it.
            var wallpaperHub = web.Load(html);
            return wallpaperHub;
        }

        public static string WallpaperSelector(string wallpaperCode)
        {
            var wallpaperLink = "";
            var wallpaperHubPage = UserWallpaperPage(wallpaperCode);

            var divWallpaperClass = wallpaperHubPage.DocumentNode.Descendants("div").FirstOrDefault(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "wallpaperSection resolutions");
            if (divWallpaperClass != null)
            {
                var aLinkTag = divWallpaperClass.Descendants('a').FirstOrDefault(x => x.InnerHtml.Contains("h5") && x.InnerHtml.Contains(ResolutionSelector()));
                if (aLinkTag != null)
                {
                    var downloadLink = aLinkTag.Attributes["href"].Value.Split(new string[] { "Url=" }, StringSplitOptions.None);
                    wallpaperLink = downloadLink[1];
                    DownloadWallpaper(wallpaperLink, FileName(wallpaperCode));
                }
                else
                {
                    aLinkTag = divWallpaperClass.Descendants('a').FirstOrDefault(x => x.InnerHtml.Contains("h5") && x.InnerHtml.Contains("1920px x 1080px"));
                    if (aLinkTag != null)
                    {
                        var downloadLink = aLinkTag.Attributes["href"].Value.Split(new string[] { "Url=" }, StringSplitOptions.None);
                        wallpaperLink = downloadLink[1];
                        DownloadWallpaper(wallpaperLink, FileName(wallpaperCode));
                    }
                }
            }

            return wallpaperLink;
        }

        public static string ResolutionSelector()
        {
            int ScreenHeight = Screen.PrimaryScreen.Bounds.Height;
            int ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            string display;

            if (ScreenHeight == 3000 && ScreenWidth == 4500 || ScreenWidth / ScreenHeight == 1.5)
            {
                //Surface Studio 2
                display = "4500px x 3000px";
            }
            else if (ScreenHeight == 2160 && ScreenWidth == 3240 || ScreenHeight == 3240 && ScreenWidth == 2160)
            {
                //Surface Book 2
                display = "3240px x 2160px";
            }
            else if (ScreenHeight == 1920 && ScreenWidth == 2880 || ScreenHeight == 2880 && ScreenWidth == 1920)
            {
                //Surface Pro X
                display = "2880px x 1920px";
            }
            else if (ScreenHeight == 1824 && ScreenWidth == 2736 || ScreenHeight == 2736 && ScreenWidth == 1824)
            {
                //Surface Pro 7
                display = "2736px x 1824px";
            }
            else if (ScreenHeight == 1664 && ScreenWidth == 2496)
            {
                //Surface Laptop 3 15
                display = "2496px x 1664px";
            }
            else if (ScreenHeight == 1504 && ScreenWidth == 2256)
            {
                //Surface Laptop 3 13
                display = "2256px x 1504px";
            }
            else if (ScreenHeight == 1200 && ScreenWidth == 1800 || ScreenHeight == 1800 && ScreenWidth == 1200)
            {
                //Surface Go
                display = "1800px x 1200px";
            }
            else if (ScreenHeight == 2160 && ScreenWidth == 4096 || ScreenHeight == 4096 && ScreenWidth == 2160 || ScreenHeight / ScreenWidth == 16 / 9 || ScreenWidth / ScreenHeight == 16 / 9)
            {
                //4K
                display = "4096px x 2160px";
            }
            else if (ScreenHeight == 1440 && ScreenWidth == 3440)
            {
                //Ultrawide
                display = "3440px x 1440px";
            }
            else if (ScreenHeight == 1080 && ScreenWidth == 1920 || ScreenHeight == 1920 && ScreenWidth == 1080)
            {
                //1080p
                display = "1920px x 1080px";
            }
            else if (ScreenHeight == 1080 && ScreenWidth == 1920 || ScreenHeight == 1920 && ScreenWidth == 1080)
            {
                //Mobile
                display = "1125px x 2436px";
            }
            else if (ScreenHeight == 560 && ScreenWidth == 1218 || ScreenHeight == 1218 && ScreenWidth == 560)
            {
                //Mobile (small)
                display = "560px x 1218px";
            }
            else
            {
                display = "Default";
            }
            return display;
        }

        public static string FileName(string wallpaperCode)
        {
            var WallpaperHubPage = UserWallpaperPage(wallpaperCode);
            var fileName = "Wallpaper.jpg";

            var divWallpaperContainer = WallpaperHubPage.DocumentNode.Descendants("div").FirstOrDefault(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "wallpaperContainer");
            if (divWallpaperContainer != null)
            {
                //Does the same thing as above and looks for the Div with details class and stores it in the title var.
                var divDetailsContainer = divWallpaperContainer.Descendants("div").FirstOrDefault(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "details");
                if (divDetailsContainer != null)
                {
                    //Finds the first h1 tag that is in title var and stores it in titlevalue
                    var titleValue = divDetailsContainer.Descendants("h1").FirstOrDefault();
                    if (titleValue != null)
                    {
                        //A var that is set to the innter text of the the h1 that was found in the titleValue var.
                        fileName = titleValue.InnerText + ".jpg";
                    }

                }
            }

            return fileName;
        }

        public static void DownloadWallpaper(string wallpaperLink, string fileName)
        {
            using (var client = new WebClient())
            {
                var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "WallpaperHub\\");
                System.IO.Directory.CreateDirectory(filePath);
                var file = filePath + fileName;
                client.DownloadFile(wallpaperLink, file);
                SetWallpaper(file, PicturePosition.Fit);
                Console.WriteLine("\nRemember, we have also saved the wallpaper file in: Pictures\\WallpaperHub\\" + fileName);
                Console.ReadKey();
            }
        }

        public enum PicturePosition
        {
            Tile, Center, Stretch, Fit, Fill
        }

        internal sealed class NativeMethods
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            internal static extern int SystemParametersInfo(
                int uAction,
                int uParam,
                String lpvParam,
                int fuWinIni);
        }

        public static void SetWallpaper(string file, PicturePosition style)
        {
            Console.WriteLine("\nSetting wallpaper...");
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            switch (style)
            {
                case PicturePosition.Tile:
                    key.SetValue(@"PicturePosition", "0");
                    key.SetValue(@"TileWallpaper", "1");
                    break;
                case PicturePosition.Center:
                    key.SetValue(@"PicturePosition", "0");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case PicturePosition.Stretch:
                    key.SetValue(@"PicturePosition", "2");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case PicturePosition.Fit:
                    key.SetValue(@"PicturePosition", "6");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case PicturePosition.Fill:
                    key.SetValue(@"PicturePosition", "10");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
            }
            key.Close();

            const int SET_DESKTOP_BACKGROUND = 20;
            const int UPDATE_INI_FILE = 1;
            const int SEND_WINDOWS_INI_CHANGE = 2;
            NativeMethods.SystemParametersInfo(SET_DESKTOP_BACKGROUND, 0, file, UPDATE_INI_FILE | SEND_WINDOWS_INI_CHANGE);
            Console.WriteLine("Wallpaper Set! :)");
        }
    }
}