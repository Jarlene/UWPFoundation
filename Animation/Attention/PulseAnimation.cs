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
    public class PulseAnimation : AnimationBase
    {
        public PulseAnimation()
        {
            RepeatBehavior = new RepeatBehavior(2);
            Duration = TimeSpan.FromMilliseconds(400);
        }

        public override IAnimation PlayOn(UIElement target, Action continueWith)
        {
            var transform = (CompositeTransform)AnimationUtils.PrepareTransform(target, typeof(CompositeTransform));
            AnimationUtils.SetCenterForCompositeTransform(target, transform);

            var storyboard = PrepareStoryboard(continueWith);

            AddAnimationToStoryboard(storyboard, transform, CreateAnimation(), "ScaleX");

            AddAnimationToStoryboard(storyboard, transform, CreateAnimation(), "ScaleY");

            storyboard.Begin();

            return this;
        }

        Timeline CreateAnimation()
        {
            return new DoubleAnimation()
            {
                Duration = new Duration(Duration),
                To = 1.1,
                EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn },
                AutoReverse = true,
            };
        }
    }
}
