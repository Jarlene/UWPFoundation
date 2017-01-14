using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Animation
{
    public class WaveAnimation : AnimationBase
    {
        public WaveAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(1000);
        }

        public override IAnimation PlayOn(UIElement target, Action continueWith)
        {
            var transform = (CompositeTransform)AnimationUtils.PrepareTransform(target, typeof(CompositeTransform));
            transform.CenterX = AnimationUtils.GetCenterX(target);
            transform.CenterY = AnimationUtils.GetBottomY(target);

            var storyboard = PrepareStoryboard(continueWith);

            var anim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 12, -12, 3, -3, 0);
            AddAnimationToStoryboard(storyboard, transform, anim, "Rotation");

            storyboard.Begin();

            return this;
        }
    }
}
