using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Editing;
using Windows.Media.Effects;
using Windows.Storage;
using Windows.Storage.Pickers;
using ExampleEffectsLibrary.Compositors;

namespace CustomVideoCompositor
{
    public sealed partial class MainPage : Page
    {
        private List<Point> _overlayLocations;
        private int _currentOverlay;
        private MediaComposition _composition;
        private MediaOverlayLayer _overlayLayer;

        public MainPage()
        {
            this.InitializeComponent();

            _currentOverlay = 0;
            _overlayLocations = new List<Point>
            {
                new Point(100, 150),
                new Point(30, 350),
                new Point(350, 350)
            };
            VideoDisplay.TransportControls.IsCompact = true;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            _composition = new MediaComposition();

            _overlayLayer = new MediaOverlayLayer(new VideoCompositorDefinition(typeof(SimpleCompositor).FullName));
            _composition.OverlayLayers.Add(_overlayLayer);
        }

        private async void BaseVideo_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".mp4");
            picker.FileTypeFilter.Add(".mov");
            var file = await picker.PickSingleFileAsync();

            var clip = await MediaClip.CreateFromFileAsync(file);
            _composition.Clips.Add(clip);

            SetupMediaStreamSource();
        }

        private async void InitialVideo_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".mp4");
            picker.FileTypeFilter.Add(".mov");
            var file = await picker.PickSingleFileAsync();

            var clip = await MediaClip.CreateFromFileAsync(file);
            var clipProperties = clip.GetVideoEncodingProperties();
            _overlayLayer.Overlays.Add(new MediaOverlay(clip, new Rect(10, 10, clipProperties.Width / 4, clipProperties.Height / 4), 0.7));

            SetupMediaStreamSource();
        }

        private async void OverlayVideo_Click(object sender, RoutedEventArgs e)
        {
            if (_currentOverlay < _overlayLocations.Count)
            {
                var picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".mp4");
                var file = await picker.PickSingleFileAsync();

                var clip = await MediaClip.CreateFromFileAsync(file);
                var clipProperties = clip.GetVideoEncodingProperties();
                _overlayLayer.Overlays.Add(new MediaOverlay(clip, new Rect(_overlayLocations[_currentOverlay], new Size(clipProperties.Width / 2, clipProperties.Height / 2)), 0.7));

                SetupMediaStreamSource();
                _currentOverlay++;
            }
        }

        private void SetupMediaStreamSource()
        {
            VideoDisplay.SetMediaStreamSource(_composition.GeneratePreviewMediaStreamSource((int)VideoDisplay.ActualWidth, (int)VideoDisplay.ActualHeight));
        }
    }
}
