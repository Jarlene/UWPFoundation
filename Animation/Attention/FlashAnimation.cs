using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Animation
{
    public class FlashAnimation : AnimationBase
    {
        public FlashAnimation()
        {
            RepeatBehavior = new RepeatBehavior(2);
            Duration = TimeSpan.FromMilliseconds(400);
        }

        public override IAnimation PlayOn(UIElement target, Action continueWith)
        {
            var storyboard = PrepareStoryboard(continueWith);

            AddAnimationToStoryboard(storyboard, target, CreateAnimation(), "Opacity");

            storyboard.Begin();

            return this;
        }

        Timeline CreateAnimation()
        {
            DoubleAnimationUsingKeyFrames frames = new DoubleAnimationUsingKeyFrames();

            frames.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                EasingFunction = new SineEase()
                {
                    EasingMode = EasingMode.EaseIn
                },
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(Duration.TotalMilliseconds / 2)),
                Value = 0,
            });
            frames.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                EasingFunction = new SineEase()
                {
                    EasingMode = EasingMode.EaseIn
                },
                KeyTime = KeyTime.FromTimeSpan(Duration),
                Value = 1,
            });

            return frames;
        }
    }
}
