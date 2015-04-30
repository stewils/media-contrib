using System;
using Windows.Foundation.Collections;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.UI.Text;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;

namespace ExampleEffectsLibrary.Compositors
{
    public sealed class SimpleCompositor : IVideoCompositor
    {
        private VideoEncodingProperties _backgroundProperties;
        private CanvasDevice _canvasDevice;

        public void CompositeFrame(CompositeVideoFrameContext context)
        {
            IDirect3DSurface outputSurface = context.OutputFrame.Direct3DSurface;
            using (CanvasRenderTarget renderTarget = CanvasRenderTarget.CreateFromDirect3D11Surface(_canvasDevice, outputSurface))
            using (CanvasDrawingSession drawSession = renderTarget.CreateDrawingSession())
            {
                foreach (var overlaySurface in context.SurfacesToOverlay)
                {
                    var overlay = context.GetOverlayForSurface(overlaySurface);

                    var width = (float)overlay.Position.Width;
                    var height = (float)overlay.Position.Height;
                    using (var overlayBitmap = CanvasBitmap.CreateFromDirect3D11Surface(_canvasDevice, overlaySurface))
                    using (var videoBrush = new CanvasImageBrush(_canvasDevice, overlayBitmap))
                    {
                        var scale = width / overlay.Clip.GetVideoEncodingProperties().Width;
                        videoBrush.Transform = Matrix3x2.CreateScale(scale) * Matrix3x2.CreateTranslation((float)overlay.Position.X, (float)overlay.Position.Y);
                        drawSession.FillEllipse(new Vector2((float)overlay.Position.X + width / 2, (float)overlay.Position.Y + height / 2), width / 2, height / 2, videoBrush);
                    }
                }

                drawSession.DrawText("Party\nTime!", new Vector2(_backgroundProperties.Width / 1.5f, 100), Windows.UI.Colors.CornflowerBlue,
                    new CanvasTextFormat()
                    {
                        FontSize = (float)_backgroundProperties.Width / 13,
                        FontWeight = new FontWeight() { Weight = 999 },
                        HorizontalAlignment = CanvasHorizontalAlignment.Center,
                        VerticalAlignment = CanvasVerticalAlignment.Center
                    });
            }
        }

        public void SetEncodingProperties(VideoEncodingProperties backgroundProperties, IDirect3DDevice device)
        {
            _backgroundProperties = backgroundProperties;
            _canvasDevice = CanvasDevice.CreateFromDirect3D11Device(device, CanvasDebugLevel.Error);
        }

        public void Close(MediaEffectClosedReason reason)
        {
            // Clean up device resources
            if (_canvasDevice != null)
                _canvasDevice.Dispose();
        }

        public void SetProperties(IPropertySet configuration)
        {
            // We have no special configuration
        }

        public void DiscardQueuedFrames()
        {
            // We don't cache frames, so we have nothing to clean up
        }

        public bool TimeIndependent { get { return false; } }
    }
}
