using System;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ExampleEffectsLibrary.Effects;

namespace CustomEffectsWithCapture
{
    public sealed partial class MainPage : Page
    {
        MediaCapture _mediaCapture;
        IPropertySet _effectConfiguration;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _mediaCapture = new MediaCapture();
            _effectConfiguration = new PropertySet();
            // This becomes relevant once MSFT:2300978 is fixed
            //effectProperties["Encoding"] = (VideoEncodingProperties)mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
            _effectConfiguration["Saturation"] = 0.0f;
        }

        private async void captureVideo_Click(object sender, RoutedEventArgs e)
        {
            var settings = new MediaCaptureInitializationSettings()
            {
                StreamingCaptureMode = StreamingCaptureMode.Video
            };
            await _mediaCapture.InitializeAsync(settings);
            _mediaCapture.Failed += MediaCapture_Failed;
            captureElement.Source = _mediaCapture;

            var effect = await _mediaCapture.AddVideoEffectAsync(new VideoEffectDefinition(typeof(SaturationVideoEffect).FullName, _effectConfiguration), MediaStreamType.VideoPreview);
            await _mediaCapture.StartPreviewAsync();
        }

        private void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            throw new NotImplementedException();
        }

        private void SaturationSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            _effectConfiguration["Saturation"] = (float)e.NewValue / 100f;
        }
    }
}
