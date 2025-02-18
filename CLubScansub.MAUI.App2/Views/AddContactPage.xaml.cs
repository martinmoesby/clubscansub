using CLubScansub.MAUI.App2.Models;
using Contact = CLubScansub.MAUI.App2.Models.Contact;

namespace CLubScansub.MAUI.App2.Views;

public partial class AddContactPage : ContentPage
{
	public AddContactPage()
	{
		InitializeComponent();
	}

    private void contactCtrl_OnCancel(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync($"//{nameof(ContactsPage)}", true);
    }

    private void contactCtrl_OnSave(object sender, EventArgs e)
    {
        var contact = new Contact()
        {
            Name = contactCtrl.Name,
            Address = contactCtrl.Address,
            Phone = contactCtrl.Phone,
            Email = contactCtrl.Email
        };

        ContactRepository.AddContact(contact);

        Shell.Current.GoToAsync($"//{nameof(ContactsPage)}",true);
    }

    private void contactCtrl_OnError(object sender, string e)
    {
        DisplayAlert("Error", e, "OK");
    }
}