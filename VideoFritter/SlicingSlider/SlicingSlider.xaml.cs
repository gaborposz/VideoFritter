using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace VideoFritter.SlicingSlider
{
    /// <summary>
    /// Interaction logic for SlicingSlider.xaml
    /// </summary>
    public partial class SlicingSlider : UserControl
    {
        public SlicingSlider()
        {
            InitializeComponent();
        }


        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(SlicingSlider), new PropertyMetadata(0.0));


        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(SlicingSlider), new PropertyMetadata(1.0));


        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(SlicingSlider), new PropertyMetadata(0.0));


        public bool IsSelectionRangeEnabled
        {
            get { return (bool)GetValue(IsSelectionRangeEnabledProperty); }
            set { SetValue(IsSelectionRangeEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsSelectionRangeEnabledProperty =
            DependencyProperty.Register("IsSelectionRangeEnabled", typeof(bool), typeof(SlicingSlider), new PropertyMetadata(true));


        public TickPlacement TickPlacement
        {
            get { return (TickPlacement)GetValue(TickPlacementProperty); }
            set { SetValue(TickPlacementProperty, value); }
        }

        public static readonly DependencyProperty TickPlacementProperty =
            DependencyProperty.Register("TickPlacement", typeof(TickPlacement), typeof(SlicingSlider), new PropertyMetadata(TickPlacement.BottomRight));


        public double TickFrequency
        {
            get { return (double)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }

        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register("TickFrequency", typeof(double), typeof(SlicingSlider), new PropertyMetadata(0.1));


        public double SelectionStart
        {
            get { return (double)GetValue(SelectionStartProperty); }
            set { SetValue(SelectionStartProperty, value); }
        }

        public static readonly DependencyProperty SelectionStartProperty =
            DependencyProperty.Register("SelectionStart", typeof(double), typeof(SlicingSlider), new PropertyMetadata(0.0));


        public double SelectionEnd
        {
            get { return (double)GetValue(SelectionEndProperty); }
            set { SetValue(SelectionEndProperty, value); }
        }

        public static readonly DependencyProperty SelectionEndProperty =
            DependencyProperty.Register("SelectionEnd", typeof(double), typeof(SlicingSlider), new PropertyMetadata(1.0));
    }
}
