using Avalonia.Controls;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Views
{
    public partial class EditInventoryDialog : Window
    {
        public EditInventoryDialog(Inventory inventoryToEdit)
        {
            InitializeComponent();
            Title = "Edit Inventory Item";

            // Create and host the AddInventoryDialog for editing
            var addDialog = new AddInventoryDialog(inventoryToEdit);
            var contentControl = this.FindControl<ContentControl>("DialogContent");
            if (contentControl != null)
            {
                contentControl.Content = addDialog.Content;

                // Handle the dialog result
                addDialog.Closed += (s, e) =>
                {
                    var result = addDialog.DataContext as Inventory;
                    this.Close(result);
                };
            }
        }
    }
}