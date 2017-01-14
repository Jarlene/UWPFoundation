
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
    public class DropOutAnimation : AnimationBase
    {
        public DropOutAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(700);
        }

        public override IAnimation PlayOn(UIElement target, Action continueWith)
        {
            var transform = (CompositeTransform)AnimationUtils.PrepareTransform(target, typeof(CompositeTransform));
            transform.TranslateY = -AnimationUtils.GetPointInParent(target).Y - target.RenderSize.Height;
            var storyboard = PrepareStoryboard(continueWith);
            AddAnimationToStoryboard(storyboard, transform, CreateAnimation(), "TranslateY");

            storyboard.Begin();

            return this;
        }

        Timeline CreateAnimation()
        {
            return new DoubleAnimation()
            {
                EasingFunction = new BounceEase()
                {
                    Bounces = 3,
                    Bounciness = 3,
                    EasingMode = EasingMode.EaseOut
                },
                Duration = new Duration(Duration),
                To = 0,
            };
        }
    }
}
