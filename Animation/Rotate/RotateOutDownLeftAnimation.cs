
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
    public class RotateOutDownLeftAnimation : AnimationBase
    {
        public RotateOutDownLeftAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(800);
        }

        public override IAnimation PlayOn(UIElement target, Action continueWith)
        {
            var transform = (CompositeTransform)AnimationUtils.PrepareTransform(target, typeof(CompositeTransform));
            transform.Rotation = 0;
            transform.CenterY = AnimationUtils.GetBottomY(target);
            var storyboard = PrepareStoryboard(continueWith);

            var opacityAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 0);
            var rotateAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 90);
            AddAnimationToStoryboard(storyboard, target, opacityAnim, "Opacity");
            AddAnimationToStoryboard(storyboard, transform, rotateAnim, "Rotation");

            storyboard.Begin();

            return this;
        }
    }
}
