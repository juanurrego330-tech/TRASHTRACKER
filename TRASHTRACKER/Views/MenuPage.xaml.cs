using TrashTracker.ViewModels;

namespace TrashTracker.Views
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as MenuViewModel)?.Refrescar();
        }
    }
}