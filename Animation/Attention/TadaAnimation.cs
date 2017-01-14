using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Animation
{
    public class TadaAnimation : AnimationBase
    {
        public TadaAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(1000);
        }

        public override IAnimation PlayOn(UIElement target, Action continueWith)
        {
            var transform = (CompositeTransform)AnimationUtils.PrepareTransform(target, typeof(CompositeTransform));
            transform.CenterX = AnimationUtils.GetCenterX(target);
            transform.CenterY = AnimationUtils.GetCenterY(target);

            var storyboard = PrepareStoryboard(continueWith);

            var scaleXAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 1, 0.9f, 0.9f, 1.1f, 1.1f, 1.1f, 1.1f, 1.1f, 1.1f, 1);
            AddAnimationToStoryboard(storyboard, transform, scaleXAnim, "ScaleX");

            var scaleYAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 1, 0.9f, 0.9f, 1.1f, 1.1f, 1.1f, 1.1f, 1.1f, 1.1f, 1);
            AddAnimationToStoryboard(storyboard, transform, scaleYAnim, "ScaleY");

            var rotateAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 0, -3, -3, 3, -3, 3, -3, 3, -3, 0);
            AddAnimationToStoryboard(storyboard, transform, rotateAnim, "Rotation");

            storyboard.Begin();

            return this;
        }

    }
}
