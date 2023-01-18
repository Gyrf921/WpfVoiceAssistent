using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;

namespace WpfVoiceAssistent.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Menubutton.xaml
    /// </summary>
    public partial class Menubutton : UserControl
    {
        public Menubutton()
        {
            InitializeComponent();
        }

        public PackIconMaterialKind Icon
        {
            get { return (PackIconMaterialKind)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(PackIconMaterialKind), typeof(Menubutton));
       
        
        public PackIconMaterialKind IsActive
        {
            get { return (PackIconMaterialKind)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(PackIconMaterialKind), typeof(Menubutton));
    
    }
}
