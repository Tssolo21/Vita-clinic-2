using Avalonia.Controls;
using Avalonia.Interactivity;
using VitaClinic.WebAPI.Models;
using VitaClinic.WebAPI.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace VitaClinic.WebAPI.Views
{
    public partial class AddInventoryDialog : Window
    {
        private Inventory? _inventoryToEdit;

        public AddInventoryDialog()
        {
            InitializeComponent();
            Title = "Add New Inventory Item";
        }

        public AddInventoryDialog(Inventory inventoryToEdit) : this()
        {
            _inventoryToEdit = inventoryToEdit;
            Title = "Edit Inventory Item";

            // Populate fields with existing data
            var itemNameBox = this.FindControl<TextBox>("ItemNameBox");
            if (itemNameBox != null) itemNameBox.Text = inventoryToEdit.ItemName;

            var itemTypeBox = this.FindControl<ComboBox>("ItemTypeBox");
            if (itemTypeBox != null) itemTypeBox.SelectedItem = GetComboBoxItem(inventoryToEdit.ItemType);

            var descriptionBox = this.FindControl<TextBox>("DescriptionBox");
            if (descriptionBox != null) descriptionBox.Text = inventoryToEdit.Description;

            var quantityBox = this.FindControl<TextBox>("QuantityBox");
            if (quantityBox != null) quantityBox.Text = inventoryToEdit.Quantity.ToString();

            var minStockBox = this.FindControl<TextBox>("MinStockBox");
            if (minStockBox != null) minStockBox.Text = inventoryToEdit.MinimumStockLevel.ToString();

            var unitPriceBox = this.FindControl<TextBox>("UnitPriceBox");
            if (unitPriceBox != null) unitPriceBox.Text = inventoryToEdit.UnitPrice.ToString("F2");

            var supplierBox = this.FindControl<TextBox>("SupplierBox");
            if (supplierBox != null) supplierBox.Text = inventoryToEdit.Supplier;
        }

        private ComboBoxItem? GetComboBoxItem(string? itemType)
        {
            if (string.IsNullOrEmpty(itemType)) return null;
            var typeBox = this.FindControl<ComboBox>("ItemTypeBox");
            if (typeBox?.Items == null) return null;
            return typeBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item?.Content?.ToString() == itemType);
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var errorText = this.FindControl<TextBlock>("ErrorText");
            if (errorText != null)
            {
                errorText.Text = "";
            }

            // Get values from controls
            var itemName = this.FindControl<TextBox>("ItemNameBox")?.Text?.Trim();
            var itemType = (this.FindControl<ComboBox>("ItemTypeBox")?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var description = this.FindControl<TextBox>("DescriptionBox")?.Text?.Trim();
            var quantityText = this.FindControl<TextBox>("QuantityBox")?.Text?.Trim();
            var minStockText = this.FindControl<TextBox>("MinStockBox")?.Text?.Trim();
            var unitPriceText = this.FindControl<TextBox>("UnitPriceBox")?.Text?.Trim();
            var supplier = this.FindControl<TextBox>("SupplierBox")?.Text?.Trim();

            // Validation
            if (string.IsNullOrEmpty(itemName))
            {
                if (errorText != null) errorText.Text = "Item name is required.";
                return;
            }

            if (string.IsNullOrEmpty(itemType))
            {
                if (errorText != null) errorText.Text = "Please select an item type.";
                return;
            }

            if (!int.TryParse(quantityText, out int quantity) || quantity < 0)
            {
                if (errorText != null) errorText.Text = "Please enter a valid quantity (0 or greater).";
                return;
            }

            if (!int.TryParse(minStockText, out int minStock) || minStock < 0)
            {
                if (errorText != null) errorText.Text = "Please enter a valid minimum stock level (0 or greater).";
                return;
            }

            if (!decimal.TryParse(unitPriceText, out decimal unitPrice) || unitPrice < 0)
            {
                if (errorText != null) errorText.Text = "Please enter a valid unit price (0.00 or greater).";
                return;
            }

            if (_inventoryToEdit != null)
            {
                // Update existing inventory item
                _inventoryToEdit.ItemName = itemName;
                _inventoryToEdit.ItemType = itemType;
                _inventoryToEdit.Description = description;
                _inventoryToEdit.Quantity = quantity;
                _inventoryToEdit.MinimumStockLevel = minStock;
                _inventoryToEdit.UnitPrice = unitPrice;
                _inventoryToEdit.Supplier = supplier;
                _inventoryToEdit.UpdatedAt = DateTime.UtcNow;

                Close(_inventoryToEdit);
            }
            else
            {
                // Create new inventory item
                var inventory = new Inventory
                {
                    ItemName = itemName,
                    ItemType = itemType,
                    Description = description,
                    Quantity = quantity,
                    MinimumStockLevel = minStock,
                    UnitPrice = unitPrice,
                    Supplier = supplier,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                Close(inventory);
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}