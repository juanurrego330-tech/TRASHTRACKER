using TrashTracker.Views;
namespace TRASHTRACKER
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(CreateReportPage), typeof(CreateReportPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        }
    }
}
