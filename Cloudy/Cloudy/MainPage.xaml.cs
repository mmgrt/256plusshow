using BackgroundCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Cloudy
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

        }
        private string taskName = "Cloudy.Core.LiveTiles";
        private string fullStart = "";
        private string outlinedStart = "";

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            StorageFile vcdFile = await Windows.Storage.StorageFile.
                GetFileFromApplicationUriAsync(new Uri("ms-appx:///vcd.xml"));

            await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcdFile);

            RegisterBackgroundTask();

            if (e.Parameter is string && !string.IsNullOrWhiteSpace(e.Parameter.ToString()))
            {
                string requestedCityName = e.Parameter as string;
                txtCity.Text = requestedCityName;
                check_Click(txtCity, null);
            }

        }

        private async void check_Click(object sender, RoutedEventArgs e)
        {
            HttpClient wClient = new HttpClient();
            string response = await wClient.GetStringAsync($"http://api.openweathermap.org/data/2.5/weather?q={txtCity.Text}&appid=YOUR_APP_ID&units=metric");

            WeatherData dat = JsonConvert.DeserializeObject<WeatherData>(response);

            bool isCloudy = true;

            if (dat.clouds.all == 0)
            {
                isCloudy = false;
            }
            else
            {
                isCloudy = true;
            }

            if (!isCloudy)
            {
                NOContainer.Visibility = Visibility.Visible;
                YESContainer.Visibility = Visibility.Collapsed;
            }
            else
            {
                YESContainer.Visibility = Visibility.Visible;
                NOContainer.Visibility = Visibility.Collapsed;
            }
            startIcon.Text = outlinedStart;

            addToLiveTilesLabel.Text = "Add to live tiles";
        }

        private async void RegisterBackgroundTask()
        {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.Denied) { return; }

            if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == taskName)
                    {
                        task.Value.Unregister(true);
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.IsNetworkRequested = true;
                taskBuilder.TaskEntryPoint = typeof(LiveTilesTask).FullName;
                taskBuilder.SetTrigger(new TimeTrigger(15, false));
                var registration = taskBuilder.Register();
                Debug.WriteLine(registration.Name);
            }
        }


        private void addToLiveTile_Click(object sender, RoutedEventArgs e)
        {

            startIcon.Text = startIcon.Text == outlinedStart ? fullStart : outlinedStart;

            addToLiveTilesLabel.Text = addToLiveTilesLabel.Text == "Add to live tiles" ? "Added!!" : "Add to live tiles";

            ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

            if (LocalSettings.Values.ContainsKey("fav_city")) { LocalSettings.Values["fav_city"] = txtCity.Text; }
            else { LocalSettings.Values.Add("fav_city", txtCity.Text); }

            LiveTilesTask.UpdateLiveTile(txtCity.Text);
        }
    }
}

