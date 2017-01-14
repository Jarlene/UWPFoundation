﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.Graphics.Effects;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AnyView
{
    public sealed class Shadow : ContentControl
    {
        const int MAX_Z_DEPTH = 5;
        const int MIN_Z_DEPTH = 1;

        CanvasControl _shadowCanvas;
        ContentPresenter _contentPresenter;
        Grid _topGrid;

        public int Z_Depth
        {
            get { return (int)GetValue(Z_DepthProperty); }
            set { SetValue(Z_DepthProperty, value); }
        }
        public static readonly DependencyProperty Z_DepthProperty =
            DependencyProperty.Register("Z_Depth", typeof(int), typeof(Shadow), new PropertyMetadata(2));

        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(double), typeof(Shadow), new PropertyMetadata(0d));

        public bool IsCached { get; set; }


        public Shadow()
        {
            this.DefaultStyleKey = typeof(Shadow);
            SizeChanged += Shadow_SizeChanged;
            Unloaded += Shadow_Unloaded;
        }

        private void Shadow_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!IsCached && _shadowCanvas != null)
            {
                _shadowCanvas.RemoveFromVisualTree();
                _shadowCanvas = null;
            }
        }

        private void Shadow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_shadowCanvas != null && e.NewSize != e.PreviousSize)
            {
                _shadowCanvas.Invalidate();
            }
        }

        private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (Z_Depth < MIN_Z_DEPTH)
                Z_Depth = 1;

            var shadowParams = ShadowConfig.GetShadowParamForZDepth(Math.Min(Z_Depth, MAX_Z_DEPTH));
            DrawShadow(sender, args.DrawingSession, shadowParams);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _shadowCanvas = (CanvasControl)GetTemplateChild("ShadowCanvas");
            _contentPresenter = (ContentPresenter)GetTemplateChild("PART_ContentPresenter");
            _topGrid = (Grid)GetTemplateChild("PART_Grid");

            if (_shadowCanvas != null)
            {
                _shadowCanvas.Draw += OnDraw;
            }
        }

        void DrawShadow(CanvasControl canvasCtrl, CanvasDrawingSession drawSession, List<ShadowParam> shadowParams)
        {
            var canvasCommandList = new CanvasCommandList(canvasCtrl);
            var content = _contentPresenter.Content as FrameworkElement;
            var contentWidth = content.ActualWidth + content.Margin.Left + content.Margin.Right;
            var contentHeight = content.ActualHeight + content.Margin.Top + content.Margin.Bottom;
            var radius = GetActualCornerRadius(contentWidth);
            double maxOffset_Y = shadowParams.Max(param => param.Offset_Y);

            _shadowCanvas.VerticalAlignment = content.VerticalAlignment;
            _shadowCanvas.HorizontalAlignment = content.HorizontalAlignment;

            using (var ds = canvasCommandList.CreateDrawingSession())
            {
                ds.FillRoundedRectangle(new Rect(0, 0, contentWidth, contentHeight), radius, radius, Color.FromArgb(255, 0, 0, 0));
            }

            CompositeEffect compositeEffect = CreateEffects(shadowParams, canvasCommandList);

            var bound = compositeEffect.GetBounds(drawSession);
            double shadowWidth = Math.Abs(bound.X);
            double shadowHeight = Math.Abs(bound.Y);

            UpdateLayout(maxOffset_Y, bound, shadowWidth, shadowHeight);

            DrawEffect(drawSession, compositeEffect, shadowWidth, shadowHeight);
        }

        private void DrawEffect(CanvasDrawingSession drawSession, CompositeEffect compositeEffect, double shadowWidth, double shadowHeight)
        {
            drawSession.DrawImage(compositeEffect, (float)shadowWidth, (float)shadowHeight);
        }

        private CompositeEffect CreateEffects(List<ShadowParam> shadowParams, CanvasCommandList canvasCommandList)
        {
            CompositeEffect compositeEffect = new CompositeEffect();

            shadowParams.ForEach(param =>
            {
                var shadowEffect = CreateShadowEffect(canvasCommandList, param);
                compositeEffect.Sources.Add(shadowEffect);
            });
            return compositeEffect;
        }

        private void UpdateLayout(double maxOffset_Y, Rect bound, double shadowWidth, double shadowHeight)
        {
            _contentPresenter.Margin = new Thickness(shadowWidth, shadowHeight - maxOffset_Y, shadowWidth, shadowHeight + maxOffset_Y);
            _shadowCanvas.Height = bound.Height;
            _shadowCanvas.Width = bound.Width;
        }

        float GetActualCornerRadius(double length)
        {
            if (CornerRadius >= 1)
                return (float)CornerRadius;

            return (float)(length * CornerRadius);
        }

        Transform2DEffect CreateShadowEffect(IGraphicsEffectSource source, ShadowParam param)
        {
            return new Transform2DEffect
            {
                Source = new Transform2DEffect
                {
                    Source = new ShadowEffect
                    {
                        BlurAmount = param.Blur,
                        ShadowColor = Color.FromArgb(param.Alpha, 0, 0, 0),
                        Source = source
                    },
                },
            };
        }
    }
}
