﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Animation
{
    public class ZoomInRightAnimation : AnimationBase
    {
        public ZoomInRightAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(1000);
        }

        public override IAnimation PlayOn(UIElement target, Action continueWith)
        {
            var transform = (CompositeTransform)AnimationUtils.PrepareTransform(target, typeof(CompositeTransform));
            target.Opacity = 0;
            transform.TranslateX = AnimationUtils.GetParentSize(target).Width - AnimationUtils.GetPointInParent(target).X;
            transform.ScaleX = transform.ScaleY = 0;
            transform.CenterX = AnimationUtils.GetCenterX(target);
            transform.CenterY = AnimationUtils.GetCenterY(target);
            var storyboard = PrepareStoryboard(continueWith);

            var opacityAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 1);
            var scaleXAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 1);
            var scaleYAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 1);
            var transformXAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, -30, 0);
            AddAnimationToStoryboard(storyboard, target, opacityAnim, "Opacity");
            AddAnimationToStoryboard(storyboard, transform, scaleXAnim, "ScaleX");
            AddAnimationToStoryboard(storyboard, transform, scaleYAnim, "ScaleY");
            AddAnimationToStoryboard(storyboard, transform, transformXAnim, "TranslateX");

            storyboard.Begin();

            return this;
        }
    }
}