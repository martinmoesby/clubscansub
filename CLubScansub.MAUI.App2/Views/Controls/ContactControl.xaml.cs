using System.Runtime.CompilerServices;

namespace CLubScansub.MAUI.App2.Views.Controls;

public partial class ContactControl : ContentView
{

    public event EventHandler<string>? OnError;
    public event EventHandler<EventArgs>? OnSave;
    public event EventHandler<EventArgs>? OnCancel;
    public ContactControl()
	{
		InitializeComponent();
	}

    public string Name { get => entryName.Text; set => entryName.Text = value; }
    public string Email { get => entryEmail.Text; set => entryEmail.Text = value; }
    public string Phone { get => entryPhone.Text; set => entryPhone.Text = value; }
    public string Address { get => entryAddress.Text; set => entryAddress.Text = value; }

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        OnCancel?.Invoke(sender, e);
    }

    private void SaveButton_Clicked(object sender, EventArgs e)
    {
        if (nameValidator.IsNotValid)
        {
            OnError?.Invoke(sender, "Name is required");
            return;
        }

        if (emailValidator.IsNotValid)
        {
            var errorMessage = "";
            foreach (var item in emailValidator.Errors!)
            {
                errorMessage += item!.ToString() + "\n";
            }
            OnError?.Invoke(sender, errorMessage);
            return;
        }

        OnSave?.Invoke(sender, e);
    }
}