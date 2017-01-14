namespace AnyView
{
    public delegate void ToggleEvent(object sender, ToggleEventArgs args);

    public class ToggleEventArgs
    {
        public ToggleEventArgs(bool isChecked) { IsChecked = isChecked; }

        public bool IsChecked { get; set; }

        public bool IsCancel { get; set; }
    }
}
