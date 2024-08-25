using Plugin.Maui.Audio;
using System.Globalization;
using System.Timers;

namespace SoundPlayer
{
    public partial class MainPage : ContentPage
    {
        private readonly IAudioManager _audioManager;
        private System.Timers.Timer _timer;
        private DateTime _currentTime;
        private CultureInfo _culture = new CultureInfo("en-US");

        public MainPage(IAudioManager audioManager)
        {
            InitializeComponent();

            InitializeFontSize();

            _audioManager = audioManager;

            UpdateTime();

            InitializeAndStartTimer();
        }

        private void InitializeAndStartTimer()
        {
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        private void InitializeFontSize()
        {
            // Dynamically adjust the font size based on the screen size
            var fontSize = DeviceDisplay.MainDisplayInfo.Height / 15; // Adjust this factor as needed
            HoursLabel.FontSize = fontSize;
            MinutesLabel.FontSize = fontSize;
            AmPmLabel.FontSize = fontSize / 2;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Update the time on the UI thread
            Dispatcher.Dispatch(() =>
            {
                UpdateTime();
            });
        }

        private void UpdateTime()
        {
            _currentTime = DateTime.Now;
            HoursLabel.Text = _currentTime.ToString("hh", _culture);
            MinutesLabel.Text = _currentTime.ToString("mm", _culture);
            AmPmLabel.Text = _currentTime.ToString("tt", _culture);
        }

        private async void OnTimeButtonClicked(object sender, EventArgs e)
        {
            List<string> fileList = GetCurrentTimeFileList();
            
            foreach (string file in fileList)
            {
                var player = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(file));

                player.Play();
                Thread.Sleep(200);

                while (player.IsPlaying)
                {
                    Thread.Sleep(200);
                }

                player.Dispose();
            }
        }

        private async void OnInfoButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Alert", "You have been alerted", "OK");
        }

        private List<string> GetCurrentTimeFileList()
        {
            
            List<int> hoursAndMinutes = GetHoursAndMinutes();
            List<string> CurrentTimeFileList = new List<string>();

            CurrentTimeFileList.Add("es_ist.m4a");

            switch(hoursAndMinutes[1])
            {
                case 0:
                    CurrentTimeFileList.Add(hoursAndMinutes[0] + ".m4a");
                    CurrentTimeFileList.Add("uhr.m4a");
                    break;
                case 1:
                    CurrentTimeFileList.Add("kurz.m4a");
                    CurrentTimeFileList.Add("nach.m4a");
                    CurrentTimeFileList.Add(hoursAndMinutes[0] + ".m4a");
                    break;
                case 5:
                case 10:
                    CurrentTimeFileList.Add(hoursAndMinutes[1] + ".m4a");
                    CurrentTimeFileList.Add("nach.m4a");
                    CurrentTimeFileList.Add(hoursAndMinutes[0] + ".m4a");
                    break;
                case 15:
                    CurrentTimeFileList.Add("viertel.m4a");
                    CurrentTimeFileList.Add("nach.m4a");
                    CurrentTimeFileList.Add(hoursAndMinutes[0] + ".m4a");
                    break;
                case 20:
                    CurrentTimeFileList.Add("zwanzig.m4a");
                    CurrentTimeFileList.Add("nach.m4a");
                    CurrentTimeFileList.Add(hoursAndMinutes[0] + ".m4a");
                    break;
                case 25:
                    CurrentTimeFileList.Add("5.m4a");
                    CurrentTimeFileList.Add("vor.m4a");
                    CurrentTimeFileList.Add("halb.m4a");
                    CurrentTimeFileList.Add((hoursAndMinutes[0] + 1) + ".m4a");
                    break;
                case 30:
                    CurrentTimeFileList.Add("halb.m4a");
                    CurrentTimeFileList.Add((hoursAndMinutes[0] + 1) + ".m4a");
                    break;
                case 35:
                    CurrentTimeFileList.Add("5.m4a");
                    CurrentTimeFileList.Add("nach.m4a");
                    CurrentTimeFileList.Add("halb.m4a");
                    CurrentTimeFileList.Add((hoursAndMinutes[0] + 1) + ".m4a");
                    break;
                case 40:
                    CurrentTimeFileList.Add("zwanzig.m4a");
                    CurrentTimeFileList.Add("vor.m4a");
                    CurrentTimeFileList.Add((hoursAndMinutes[0] + 1) + ".m4a");
                    break;
                case 45:
                    CurrentTimeFileList.Add("viertel.m4a");
                    CurrentTimeFileList.Add("vor.m4a");
                    CurrentTimeFileList.Add((hoursAndMinutes[0] + 1) + ".m4a");
                    break;
                case 50:
                    CurrentTimeFileList.Add("10.m4a");
                    CurrentTimeFileList.Add("vor.m4a");
                    CurrentTimeFileList.Add((hoursAndMinutes[0] + 1) + ".m4a");
                    break;
                case 55:
                    CurrentTimeFileList.Add("5.m4a");
                    CurrentTimeFileList.Add("vor.m4a");
                    CurrentTimeFileList.Add((hoursAndMinutes[0] + 1) + ".m4a");
                    break;
                default:
                    CurrentTimeFileList = new List<string>();
                    break;

            }
            
            return CurrentTimeFileList;
        }

        private List<int> GetHoursAndMinutes()
        {
            int hours = Int32.Parse(_currentTime.ToString("hh", _culture));
            int minutes = Int32.Parse(_currentTime.ToString("mm", _culture));
            if (minutes > 0 && minutes <= 4)
            {
                minutes = 1;
            }
            else
            {
                minutes /= 5;
                minutes *= 5;
            }
            return new List<int>
            {
                hours,
                minutes
            };
        }
    }
}
