using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Views
{
    public partial class InventoryView : UserControl
    {
        private MainWindow? _mainWindow;
        private ObservableCollection<Inventory> _inventoryItems = new ObservableCollection<Inventory>();

        public ObservableCollection<Inventory> InventoryItems => _inventoryItems;

        public InventoryView()
        {
            InitializeComponent();
        }

        public InventoryView(MainWindow mainWindow) : this()
        {
            _mainWindow = mainWindow;
            this.DataContext = this;
            this.AttachedToVisualTree += OnAttachedToVisualTree;
        }

        private async void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            InitializeInventoryList();
            await LoadInventoryAsync();
        }

        private void InitializeInventoryList()
        {
            var list = this.FindControl<ItemsControl>("InventoryList");
            if (list != null)
            {
                list.ItemsSource = _inventoryItems;
                Console.WriteLine("✓ InventoryList ItemsSource set successfully");
            }
            else
            {
                Console.WriteLine("✗ ERROR: InventoryList not found!");
            }
        }

        private async void LoadInventory(object sender, RoutedEventArgs e)
        {
            await LoadInventoryAsync();
        }

        private async Task LoadInventoryAsync()
        {
            try
            {
                Console.WriteLine("Loading inventory items...");

                using var context = DatabaseHelper.CreateContext();
                var items = await context.Inventory.ToListAsync();

                Console.WriteLine($"Found {items.Count} inventory items in database");

                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _inventoryItems.Clear();
                    foreach (var item in items)
                    {
                        _inventoryItems.Add(item);
                        Console.WriteLine($"Added to UI collection - ID: {item.Id}, Name: {item.ItemName}, Quantity: {item.Quantity}");
                    }
                    Console.WriteLine($"Loaded {_inventoryItems.Count} inventory items to UI ObservableCollection");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading inventory: {ex.Message}");
            }
        }

        private async void AddInventoryItem(object sender, RoutedEventArgs e)
        {
            try
            {
                Console.WriteLine("=== AddInventoryItem START ===");

                var dialog = new AddInventoryDialog();
                var owner = Window.GetTopLevel(this) as Window;
                Console.WriteLine($"Owner window: {owner?.GetType().Name ?? "null"}");

                var result = owner != null ? await dialog.ShowDialog<Inventory?>(owner) : null;
                Console.WriteLine($"Dialog result: {(result != null ? $"Inventory item received: {result.ItemName}" : "null/cancelled")}");

                if (result != null)
                {
                    Console.WriteLine($"Adding new inventory item: {result.ItemName} ({result.ItemType})");
                    Console.WriteLine($"Database path: {DatabaseHelper.GetDatabasePath()}");

                    using var context = DatabaseHelper.CreateContext();
                    result.CreatedAt = DateTime.UtcNow;
                    result.UpdatedAt = DateTime.UtcNow;

                    context.Inventory.Add(result);
                    Console.WriteLine("Inventory item added to context");

                    Console.WriteLine("Calling SaveChangesAsync for inventory item creation...");
                    var changes = await context.SaveChangesAsync();
                    Console.WriteLine($"SaveChangesAsync returned: {changes} changes");

                    Console.WriteLine($"Inventory item saved with ID: {result.Id}");

                    // Reload data
                    Console.WriteLine("Reloading inventory data...");
                    await LoadInventoryAsync();
                    Console.WriteLine("=== AddInventoryItem COMPLETE ===");
                }
                else
                {
                    Console.WriteLine("=== AddInventoryItem CANCELLED ===");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"!!! ERROR in AddInventoryItem !!!");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private async void EditInventoryItem(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string itemId)
            {
                try
                {
                    Console.WriteLine($"=== EditInventoryItem START - ID: {itemId} ===");

                    // Find the item to edit
                    var itemToEdit = _inventoryItems.FirstOrDefault(i => i.Id == itemId);
                    if (itemToEdit == null)
                    {
                        Console.WriteLine($"ERROR: Inventory item with ID {itemId} not found in current list");
                        return;
                    }

                    var dialog = new EditInventoryDialog(itemToEdit);
                    var owner = Window.GetTopLevel(this) as Window;

                    var result = owner != null ? await dialog.ShowDialog<Inventory?>(owner) : null;
                    Console.WriteLine($"Dialog result: {(result != null ? $"Inventory item updated: {result.ItemName}" : "null/cancelled")}");

                    if (result != null)
                    {
                        Console.WriteLine($"Updating inventory item: {result.ItemName}");

                        using var context = DatabaseHelper.CreateContext();
                        var existingItem = await context.Inventory.FindAsync(itemId);
                        if (existingItem != null)
                        {
                            existingItem.ItemName = result.ItemName;
                            existingItem.ItemType = result.ItemType;
                            existingItem.Description = result.Description;
                            existingItem.Quantity = result.Quantity;
                            existingItem.MinimumStockLevel = result.MinimumStockLevel;
                            existingItem.UnitPrice = result.UnitPrice;
                            existingItem.Supplier = result.Supplier;
                            existingItem.UpdatedAt = DateTime.UtcNow;

                            await context.SaveChangesAsync();
                            Console.WriteLine($"Inventory item {itemId} updated successfully");

                            // Reload data
                            await LoadInventoryAsync();
                        }
                    }

                    Console.WriteLine("=== EditInventoryItem COMPLETE ===");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"!!! ERROR in EditInventoryItem !!!");
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private async void DeleteInventoryItem(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string itemId)
            {
                try
                {
                    Console.WriteLine($"=== DeleteInventoryItem START - ID: {itemId} ===");

                    // Find the item to display info in confirmation
                    var itemToDelete = _inventoryItems.FirstOrDefault(i => i.Id == itemId);

                    // Create confirmation dialog
                    var deleteButton = new Button
                    {
                        Content = "Delete",
                        Padding = new Avalonia.Thickness(20, 10),
                        Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(239, 68, 68)),
                        Foreground = Avalonia.Media.Brushes.White
                    };

                    var cancelButton = new Button
                    {
                        Content = "Cancel",
                        Padding = new Avalonia.Thickness(20, 10),
                        Margin = new Avalonia.Thickness(0, 0, 10, 0),
                        Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(148, 163, 184)),
                        Foreground = Avalonia.Media.Brushes.White
                    };

                    var confirmDialog = new Window
                    {
                        Title = "Confirm Deletion",
                        Content = new StackPanel
                        {
                            Margin = new Avalonia.Thickness(20),
                            Children =
                            {
                                new TextBlock
                                {
                                    Text = itemToDelete != null
                                        ? $"Are you sure you want to delete inventory item '{itemToDelete.ItemName}'?\n\nThis action cannot be undone."
                                        : $"Are you sure you want to delete inventory item #{itemId}?\n\nThis action cannot be undone.",
                                    Margin = new Avalonia.Thickness(0, 0, 0, 20),
                                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                                },
                                new StackPanel
                                {
                                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                                    Children = { cancelButton, deleteButton }
                                }
                            }
                        },
                        SizeToContent = Avalonia.Controls.SizeToContent.WidthAndHeight,
                        WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
                    };

                    var owner = Window.GetTopLevel(this) as Window;

                    bool? confirmResult = null;
                    cancelButton.Click += (s, args) => { confirmResult = false; confirmDialog.Close(); };
                    deleteButton.Click += (s, args) => { confirmResult = true; confirmDialog.Close(); };

                    if (owner != null)
                    {
                        await confirmDialog.ShowDialog(owner);
                    }
                    else
                    {
                        confirmDialog.Show();
                    }

                    if (confirmResult == true)
                    {
                        Console.WriteLine($"Deleting inventory item {itemId}");

                        using var context = DatabaseHelper.CreateContext();
                        var itemToRemove = await context.Inventory.FindAsync(itemId);

                        if (itemToRemove != null)
                        {
                            context.Inventory.Remove(itemToRemove);
                            var deleteChanges = await context.SaveChangesAsync();
                            Console.WriteLine($"SaveChangesAsync returned {deleteChanges} changes for deletion");

                            if (deleteChanges > 0)
                            {
                                Console.WriteLine($"Inventory item {itemId} deleted successfully");

                                // Reload data
                                await LoadInventoryAsync();
                            }
                        }
                        else
                        {
                            Console.WriteLine($"ERROR: Inventory item {itemId} not found in database");
                        }
                    }

                    Console.WriteLine("=== DeleteInventoryItem COMPLETE ===");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"!!! ERROR in DeleteInventoryItem !!!");
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            _mainWindow?.ShowDashboard(sender, e);
        }
    }
}