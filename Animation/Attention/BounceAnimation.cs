using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Animation
{
    public class BounceAnimation : AnimationBase
    {
        public BounceAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(800);
        }

        public override IAnimation PlayOn(UIElement target, Action continueWith)
        {
            var transform = AnimationUtils.PrepareTransform(target, typeof(CompositeTransform));

            var storyboard = PrepareStoryboard(continueWith);

            AddAnimationToStoryboard(storyboard, transform, CreateAnimation(), "TranslateY");

            storyboard.Begin();

            return this;
        }

        Timeline CreateAnimation()
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
                Value = -8,
            });
            frames.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                EasingFunction = new BounceEase()
                {
                    Bounces = 2,
                    Bounciness = 1.3,
                    EasingMode = EasingMode.EaseOut
                },
                KeyTime = KeyTime.FromTimeSpan(Duration),
                Value = 0,
            });

            return frames;
        }
    }
}
