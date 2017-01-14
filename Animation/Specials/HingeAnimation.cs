
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
    public class HingeAnimation : AnimationBase
    {
        private Storyboard fallStoryboard = new Storyboard();

        public HingeAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(1500);
        }

        public override IAnimation PlayOn(UIElement target, Action continueWith)
        {
            var transform = (CompositeTransform)AnimationUtils.PrepareTransform(target, typeof(CompositeTransform));

            fallStoryboard.Completed += (s, e) => { if (continueWith != null) continueWith(); };
            var rotateStoryboard = PrepareStoryboard(fallStoryboard.Begin);

            var opacityAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds / 3, 0);
            AddAnimationToStoryboard(rotateStoryboard, transform, CreateRotateAnimation(), "Rotation");
            AddAnimationToStoryboard(fallStoryboard, target, opacityAnim, "Opacity");
            AddAnimationToStoryboard(fallStoryboard, transform, CreateFallAnimation(), "TranslateY");

            rotateStoryboard.Begin();

            return this;
        }

        public override void Stop()
        {
            fallStoryboard.Stop();
            base.Stop();
        }

        Timeline CreateRotateAnimation()
        {
            return new DoubleAnimation()
            {
                EasingFunction = new ElasticEase()
                {
                    Oscillations = 2,
                    Springiness = 3,
                    EasingMode = EasingMode.EaseOut
                },
                Duration = new Duration(TimeSpan.FromMilliseconds(Duration.TotalMilliseconds / 1.5)),
                To = 45,
            };
        }

        Timeline CreateFallAnimation()
        {
            return new DoubleAnimation()
            {
                EasingFunction = new QuinticEase(),
                Duration = new Duration(TimeSpan.FromMilliseconds(Duration.TotalMilliseconds / 3)),
                To = 700,
            };
        }
    }
}