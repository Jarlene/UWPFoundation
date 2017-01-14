using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace AnyView
{
    public abstract class BasePage: Page
    {
        protected void IsEnableTitleGoback(bool enable = false)
        {
            if (enable)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            } else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }

        protected bool IsEnableXExit
        {
            get;
            set;
        }

        protected bool IsEnableYExit
        {
            get;
            set;
        }

        protected double XExitDistance
        {
            get;
            set;
        } = 20;

        protected double YExitDistance
        {
            get;
            set;
        } = 40;

        protected double CurrentX = 0;
        protected double CurrentY = 0;

        protected virtual void SwipeManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            CurrentX += e.Delta.Translation.X;
            CurrentY += e.Delta.Translation.Y;
        }

        protected virtual void SwipeManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (IsEnableXExit && CurrentX > XExitDistance)
            {
                GoBack();
            } else if(IsEnableYExit && CurrentY > YExitDistance)
            {
                GoBack();
            }
            CurrentX = 0;
            CurrentY = 0;
        }

        protected virtual void GoBack()
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    base.OnNavigatedTo(e);
        //    SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
        //    systemNavigationManager.BackRequested += OnBackRequested;
        //    systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        //}


        //protected override void OnNavigatedFrom(NavigationEventArgs e)
        //{
        //    base.OnNavigatedFrom(e);
        //    SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
        //    systemNavigationManager.BackRequested -= OnBackRequested;
        //    systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        //}

        //private void OnBackRequested(object sender, BackRequestedEventArgs e)
        //{
        //    e.Handled = true;
        //    Frame.GoBack(new DrillInNavigationTransitionInfo());
        //}
    }

  


}
