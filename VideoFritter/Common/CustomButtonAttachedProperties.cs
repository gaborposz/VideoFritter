using System.Windows;

namespace VideoFritter.Common
{
    public static class CustomButtonAttachedProperties
    {
        public static readonly DependencyProperty RadiusValueProperty =
              DependencyProperty.RegisterAttached("RadiusValue", typeof(CornerRadius), typeof(CustomButtonAttachedProperties), new FrameworkPropertyMetadata(new CornerRadius(3)));


        public static void SetRadiusValue(DependencyObject element, CornerRadius value)
        {
            element.SetValue(RadiusValueProperty, value);
        }

        public static CornerRadius GetRadiusValue(DependencyObject element)
        {
            return (CornerRadius)element.GetValue(RadiusValueProperty);
        }
    }
}
