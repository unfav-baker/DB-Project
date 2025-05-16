using System.Windows;
using System.Windows.Controls;

namespace Dashboard.CustomControls
{
   
    public class Dashboard : TextBox
    {



        public string PlaceHolder
        {
            get { return (string)GetValue(PlaceHolderProperty); }
            set { SetValue(PlaceHolderProperty, value); }
        }

        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.Register("PlaceHolder", typeof(string), typeof(Dashboard), new PropertyMetadata(default));



        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Dashboard), new PropertyMetadata(default));





        static Dashboard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Dashboard), new FrameworkPropertyMetadata(typeof(Dashboard)));
        }
    }
}
