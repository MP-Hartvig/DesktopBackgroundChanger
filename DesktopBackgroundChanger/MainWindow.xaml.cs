using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopBackgroundChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ThreadInitializer();
            images = GetExistingImages();
        }

        int backgroundCounter = 0;
        int newImageCounter = 0;
        string defaultImageName = "test";
        string mainPath = "C:\\Users\\mpp\\Desktop\\Test pics\\";
        bool looper = true;
        List<Image> images = new List<Image>();

        private void Changer_Click(object sender, RoutedEventArgs e)
        {
            BackgroundChanger();
        }


        private void BackgroundChanger()
        {
            const int setBackground = 20;
            const int sendIniChange = 2;
            const int updateIni = 1;


            backgroundCounter++;

            if (backgroundCounter >= images.Count)
            {
                backgroundCounter = 0;
            }

            WindowsFunctions.SystemParametersInfo(setBackground, 0, images[backgroundCounter].Path, updateIni | sendIniChange);

            Dispatcher?.Invoke(() => { TextChanger(); });
        }

        internal sealed class WindowsFunctions
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            internal static extern int SystemParametersInfo(
                int uAction,
                int uParam,
                string lpvParam,
                int winIni);
        }

        private void Uploader_Click(object sender, RoutedEventArgs e)
        {
            SetNewestImage(mainPath);
            UploadImage();
        }

        private bool UploadImage()
        {
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == true)
            {
                string filePath = mainPath + defaultImageName + newImageCounter + ".png";
                BitmapImage image = new BitmapImage(new Uri(fd.FileName));
                Image imageObj = new Image(filePath, image);

                Dispatcher?.Invoke(() => { SaveImage(imageObj, filePath); });
                images.Add(imageObj);
                return true;
            }

            return false;
        }

        private void SaveImage(Image image, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image.BitMapImage));

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        private List<Image> GetExistingImages()
        {
            string[] imagePaths = GetImagesPaths(mainPath);
            List<Image> images = new List<Image>();
            foreach (string imagePath in imagePaths)
            {
                Image image = new Image(imagePath, new BitmapImage(new Uri(imagePath)));
                images.Add(image);
            }
            return images;
        }

        private string[] GetImagesPaths(string filePath)
        {
            return Directory.GetFiles(filePath);
        }

        private void SetNewestImage(string filePath)
        {
            int newestImage = 0;
            foreach (Image image in images)
            {
                int imageNumber = image.GetFileNumber(mainPath, defaultImageName);
                if (newestImage < imageNumber)
                {
                    newestImage = imageNumber;
                }
            }
            newImageCounter = newestImage + 1;
        }

        private void ThreadInitializer()
        {
            Thread backgroundTimer = new Thread(ThreadStopwatch);
            backgroundTimer.Start();
        }

        private void ThreadStopwatch()
        {
            Stopwatch sw = Stopwatch.StartNew();

            while (looper)
            {
                if (sw.Elapsed.TotalSeconds > 10)
                {
                    BackgroundChanger();
                    sw.Restart();
                }
                Dispatcher.Invoke(() => { StopwatchChanger(10 - sw.Elapsed.Seconds); });
                Thread.Sleep(100 / 15);
            }
            sw.Stop();
        }

        private void TextChanger()
        {
            CurrentImage.Text = "Current background image: " + defaultImageName + backgroundCounter;
        }

        private void StopwatchChanger(int count)
        {
            StopwatchCounter.Text = "Time left till next desktoip image: " + count.ToString();
        }

        private void Quitter_Click(object sender, RoutedEventArgs e)
        {
            looper = false;

            Thread.Sleep(500);
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
