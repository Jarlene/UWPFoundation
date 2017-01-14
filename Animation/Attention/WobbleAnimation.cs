using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Animation
{
    public class WobbleAnimation : AnimationBase
    {
        public WobbleAnimation()
        {
            Duration = TimeSpan.FromMilliseconds(1000);
        }

        public override IAnimation PlayOn(UIElement target, Action continueWith)
        {
            var transform = (CompositeTransform)AnimationUtils.PrepareTransform(target, typeof(CompositeTransform));
            transform.CenterX = AnimationUtils.GetCenterX(target);
            transform.CenterY = AnimationUtils.GetCenterY(target);

            var storyboard = PrepareStoryboard(continueWith);
            float WidthPerUnit = (float)(target.DesiredSize.Width / 100.0);

            var translationAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 0, -25 * WidthPerUnit, 20 * WidthPerUnit, -15 * WidthPerUnit, 10 * WidthPerUnit, -5 * WidthPerUnit, 0, 0);
            AddAnimationToStoryboard(storyboard, transform, translationAnim, "TranslateX");

            var rotateAnim = AnimationUtils.CreateAnimationWithValues(Duration.TotalMilliseconds, 0, -5, 3, -3, 2, -1, 0);
            AddAnimationToStoryboard(storyboard, transform, rotateAnim, "Rotation");

            storyboard.Begin();

            return this;
        }
    }
}
