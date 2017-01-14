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
    public class RubberBandAnimation : AnimationBase
    {
        public RubberBandAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(800);
        }

        public override IAnimation PlayOn(UIElement target, Action continueWith)
        {
            var transform = (CompositeTransform)AnimationUtils.PrepareTransform(target, typeof(CompositeTransform));
            AnimationUtils.SetCenterForCompositeTransform(target, transform);

            var storyboard = PrepareStoryboard(continueWith);

            AddAnimationToStoryboard(storyboard, transform, CreateAnimation(1.25), "ScaleX");

            AddAnimationToStoryboard(storyboard, transform, CreateAnimation(0.75), "ScaleY");

            storyboard.Begin();

            return this;
        }

        Timeline CreateAnimation(double startValue)
        {
            DoubleAnimationUsingKeyFrames frames = new DoubleAnimationUsingKeyFrames();
            var firstTimeSpan = TimeSpan.FromMilliseconds(Duration.TotalMilliseconds / 8);

            frames.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                EasingFunction = new SineEase()
                {
                    EasingMode = EasingMode.EaseIn
                },
                KeyTime = KeyTime.FromTimeSpan(firstTimeSpan),
                Value = startValue,
            });
            frames.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                EasingFunction = new ElasticEase()
                {
                    Oscillations = 2,
                    Springiness = 1,
                    EasingMode = EasingMode.EaseOut
                },
                KeyTime = KeyTime.FromTimeSpan(Duration),
                Value = 1,
            });

            return frames;
        }
    }
}
