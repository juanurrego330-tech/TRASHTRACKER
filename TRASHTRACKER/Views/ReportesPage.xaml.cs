using TrashTracker.ViewModels;

namespace TrashTracker.Views
{
    public partial class ReportesPage : ContentPage
    {
        public ReportesPage()
        {
            InitializeComponent();
        }

        // Recarga cada vez que se abre la página (así aparece el reporte recién creado)
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is ReportesViewModel vm)
                await vm.CargarAsync();
        }
    }
}