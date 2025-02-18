using CLubScansub.MAUI.App2.Views;

namespace CLubScansub.MAUI.App2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ContactsPage),typeof(ContactsPage));
            Routing.RegisterRoute(nameof(EditContactPage), typeof(EditContactPage));
            Routing.RegisterRoute(nameof(AddContactPage), typeof(AddContactPage));
        }
    }
}
