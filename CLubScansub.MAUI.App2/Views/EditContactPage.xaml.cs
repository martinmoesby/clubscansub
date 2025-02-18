using CLubScansub.MAUI.App2.Models;
using Contact = CLubScansub.MAUI.App2.Models.Contact;

namespace CLubScansub.MAUI.App2.Views;


[QueryProperty(nameof(ContactId),"Id")]
public partial class EditContactPage : ContentPage
{

	private Contact contact;
	public EditContactPage()
	{
		InitializeComponent();
	}

  //  private void cancelButton_Clicked(object sender, EventArgs e)
  //  {
		//Shell.Current.GoToAsync("..",true);
  //  }

	public int ContactId
	{
		set { 
			contact = ContactRepository.GetContactById(value);
			//nameLabel.Text = contact?.Name ?? "No contect selected";
			if (contact != null) { 
				contactCtrl.Name	= contact.Name;
                contactCtrl.Phone	= contact.Phone;
                contactCtrl.Address = contact.Address; ;
                contactCtrl.Email	= contact.Email;
			}

		}
	}

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
		Shell.Current.GoToAsync($"//{nameof(ContactsPage)}", true);
    }

    private void UpdateButton_Clicked(object sender, EventArgs e)
    {

		contact.Name = contactCtrl.Name;
		contact.Phone = contactCtrl.Phone;
		contact.Address = contactCtrl.Address;
		contact.Email = contactCtrl.Email;

        ContactRepository.UpdateContact(contact.ContactId, contact);

        Shell.Current.GoToAsync($"//{nameof(ContactsPage)}", true);
    }

    private void ContactControl_OnError(object sender, string e)
    {
		DisplayAlert("Error", e, "OK");
    }
}