
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
    public class FlipInXAnimation : AnimationBase
    {
        public FlipInXAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(1200);
        }

        public override IAnimation PlayOn(UIElement target, Action continueWith)
        {
            var projection = (PlaneProjection)AnimationUtils.PrepareProjection(target, typeof(PlaneProjection));
            projection.CenterOfRotationY = 0.5;
            projection.RotationX = -90;
            target.Opacity = 0.2;

            var storyboard = PrepareStoryboard(continueWith);

            var opacityAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 1);
            AddAnimationToStoryboard(storyboard, target, opacityAnim, "Opacity");
            var rotateAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 30, 0);
            AddAnimationToStoryboard(storyboard, projection, rotateAnim, "RotationX");

            storyboard.Begin();

            return this;
        }

    }
}
