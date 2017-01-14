using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Animation
{
    public class StandUpAnimation : AnimationBase
    {
        public StandUpAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(1500);
        }

        public override IAnimation PlayOn(UIElement target, Action continueWith)
        {
            var projection = (PlaneProjection)AnimationUtils.PrepareProjection(target, typeof(PlaneProjection));
            projection.CenterOfRotationY = 1;
            projection.RotationX = -55;

            var storyboard = PrepareStoryboard(continueWith);

            var anim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 30, -15, 15, 0);
            AddAnimationToStoryboard(storyboard, projection, anim, "RotationX ");

            storyboard.Begin();

            return this;
        }

    }
}
