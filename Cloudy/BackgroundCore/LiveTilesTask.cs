
 using Newtonsoft.Json;
using NotificationsExtensions.Tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;

namespace BackgroundCore
{
    public sealed class LiveTilesTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var defferal = taskInstance.GetDeferral();

             UpdateLiveTile(null);

            defferal.Complete();
        }

        public static async void UpdateLiveTile(string cityName)
        {
            ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

            if (cityName == null && !LocalSettings.Values.ContainsKey("fav_city")) { return; }
            if (cityName == null) { cityName = LocalSettings.Values["fav_city"].ToString(); }

            //Get Weather data:

            HttpClient wClient = new HttpClient();
            string response = await wClient.GetStringAsync($"http://api.openweathermap.org/data/2.5/weather?q={cityName}&appid=YOUR_APP_ID&units=metric");

            WeatherData dat = JsonConvert.DeserializeObject<WeatherData>(response);
 

            string isItCloudy, condition, img;

            if (dat.clouds.all == 0)
            {
                isItCloudy = "No";
                img = "ms-appx:///Assets/heart-no.png";
                //No
            }
            else
            {
                //Yes
                isItCloudy = "Yes!";
                img = "ms-appx:///Assets/heart-yes.png";

            }

            condition = $"It's {dat.weather.FirstOrDefault().description} w/ {dat.main.temp}° in {cityName} City!";


            //Build the tile:

            var TileLarge = new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {
                    TextStacking = TileTextStacking.Center,
                    Children =
        {
            new TileGroup()
            {
                Children =
                {
                    new TileSubgroup() { Weight = 1 },
                    new TileSubgroup()
                    {
                        Weight = 2,
                        Children =
                        {
                            new TileImage()
                            {
                                Source = new TileImageSource(img),
                                Crop = TileImageCrop.Circle
                            }
                        }
                    },
                    new TileSubgroup() { Weight = 1 }
                }
            },
            new TileText()
            {
                Text = isItCloudy,
                Style = TileTextStyle.Title,
                Align = TileTextAlign.Center
            },
            new TileText()
            {
                Text = condition,
                Style = TileTextStyle.SubtitleSubtle,
                Align = TileTextAlign.Center
            }
        }
                }
            };

            var TileWide = new TileBinding()
            {
                Branding = TileBranding.NameAndLogo,
                Content = new TileBindingContentAdaptive()
                {
                    Children =
        {
            new TileGroup()
            {
                Children =
                {
                    // Image column
                    new TileSubgroup()
                    {
                        Weight = 33,
                        Children =
                        {
                            new TileImage()
                            {
                                Source = new TileImageSource(img),
                                Crop = TileImageCrop.Circle
                            }
                        }
                    },
                    // Text column
                    new TileSubgroup()
                    {
                        // Vertical align its contents
                        TextStacking = TileTextStacking.Center,
                        Children =
                        {
                            new TileText()
                            {
                                Text = isItCloudy,
                                Style = TileTextStyle.Subheader
                            },
                            new TileText()
                            {
                                Text = condition,
                                Style = TileTextStyle.CaptionSubtle
                            }
                        }
                    }
                }
            }
        }
                }
            };

            var TileMedium = new TileBinding()
            {
                Branding = TileBranding.Name,
                Content = new TileBindingContentAdaptive()
                {
                    PeekImage = new TilePeekImage()
                    {
                        Source = new TileImageSource(img)
                    },
                    Children =
        {
            new TileText()
            {
                Text = isItCloudy
            },
            new TileText()
            {
                Text = condition,
                Style = TileTextStyle.CaptionSubtle,
                Wrap = true
            }
        }
                }
            };


            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileLarge = TileLarge,
                    TileWide = TileWide,
                    TileMedium = TileMedium
                }
            };




            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            TileNotification t = new TileNotification(content.GetXml());
            updater.Update(t);
        }

    }
}
