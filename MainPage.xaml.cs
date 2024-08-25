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

            var player = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("time1030.m4a"));

            player.Play();
            Thread.Sleep(200);

            while (player.IsPlaying)
            {
                Thread.Sleep(200);
            }

            player.Dispose();
        }

        private async void OnInfoButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Alert", "You have been alerted", "OK");
        }

        private List<string> GetCurrentTimeFileList()
        {
            return GetHourAndMinuteFiles();
        }

        private List<string> GetHourAndMinuteFiles()
        {
            string hour = _currentTime.ToString("hh", _culture) + ".m4a";
            string minute = "";
            int minuteNumber = Int32.Parse(_currentTime.ToString("mm", _culture));
            if (minuteNumber == 0)
            {
                minute += minuteNumber.ToString() + ".m4a";
            }
            else if (minuteNumber <= 4)
            {
                minute += "1.m4a";
            }
            else
            {
                int tmp = minuteNumber / 5;
                tmp *= 5;
                minute += tmp + ".m4a";
            }
            return new List<string> { hour, minute };
        }
    }
}
