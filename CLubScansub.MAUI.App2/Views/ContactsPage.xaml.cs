using CLubScansub.MAUI.App2.Models;
using System.Collections.ObjectModel;
using Contact = CLubScansub.MAUI.App2.Models.Contact;

namespace CLubScansub.MAUI.App2.Views;

public partial class ContactsPage : ContentPage
{

    private List<Contact> contacts;
    private ObservableCollection<Contact> filteredContacts;
	public ContactsPage()
	{
		InitializeComponent();

	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        contacts = ContactRepository.GetAll();
        filteredContacts=new ObservableCollection<Contact>(contacts);
        contactsList.ItemsSource = filteredContacts;
    }

    private void addContactButton_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(AddContactPage),true);
    }

    private void editContactButton_Clicked(object sender, EventArgs e)
    {

        Shell.Current.GoToAsync(nameof(EditContactPage),true);
    }

    private async void contactsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (contactsList.SelectedItem != null) {

            var item = (Contact)e.SelectedItem;
            //DisplayAlert("Item Selected", item.Name, "OK");
            await Shell.Current.GoToAsync($"{nameof(EditContactPage)}?Id={item.ContactId}",true);
        }

    }

    private void contactsList_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        contactsList.SelectedItem = null;
    }

    private void deleteButton_Clicked(object sender, EventArgs e)
    {
        var buttonCLicked = sender as MenuItem;
        var contact = buttonCLicked.CommandParameter as Contact;

        if (contact != null) {
            ContactRepository.DeleteContact(contact.ContactId);
            contacts.Remove(contact);
            filteredContacts.Remove(contact);
        }

    }

    private void loadContacts()
    {

    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filterText = ((SearchBar)sender).Text;
        if (filterText == string.Empty)
        {
            filteredContacts = new ObservableCollection<Contact>(contacts);
        } else
        {
            filteredContacts = new ObservableCollection<Contact>(contacts.Where(x => x.searchstring.Contains(filterText, StringComparison.OrdinalIgnoreCase)));
        }
        contactsList.ItemsSource = filteredContacts;

    }

    private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
    {
        //var filterText = ((SearchBar)sender).Text;
        //if (filterText == string.Empty)
        //{
        //    loadContacts();
        //    return;
        //}
        //contacts = new ObservableCollection<Contact>(contacts.Where(x=>x.searchstring.Contains(filterText, StringComparison.OrdinalIgnoreCase)));

    }
}