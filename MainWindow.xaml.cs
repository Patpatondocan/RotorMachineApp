using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;

namespace RotorMachineApp
{
    public partial class MainWindow : Window
    {
        private readonly List<char> originalItems = new();

        public MainWindow()
        {
            InitializeComponent();
            LoadAlphabet();
        }

        private void LoadAlphabet()
        {
            // Load the alphabet into the ListView
            for (char c = 'A'; c <= 'Z'; c++)
            {
                Control.Items.Add(c);
                RingH.Items.Add(c);
                originalItems.Add(c); // Store the original items
            }
        }

        private void TextBoxH_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numeric input
            if (!int.TryParse(e.Text, out int newValue))
            {
                e.Handled = true; // Cancel the input
            }
            else
            {
                // Ensure the input is within the range [0, 25]
                if (newValue < 0 || newValue > 25)
                {
                    e.Handled = true; // Cancel the input
                }
            }
        }

        private void TextBoxH_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RingH != null && int.TryParse(TextBoxH.Text, out int newValue))
            {
                // Ensure the input is within the range [0, 25]
                newValue = Math.Max(0, Math.Min(newValue, 25));

                // Calculate the number of positions to shift
                int shiftAmount = newValue;

                // Shift the items in the ListView counterclockwise
                var items = originalItems.ToList();
                items = items.Skip(shiftAmount).Concat(items.Take(shiftAmount)).ToList();

                // Clear the items in the ListView and add the shifted items
                RingH.Items.Clear();
                foreach (var item in items)
                {
                    RingH.Items.Add(item);
                }
            }
            else if (string.IsNullOrWhiteSpace(TextBoxH.Text))
            {
                // Handle the case when TextBox is cleared or set to 0
                RingH.Items.Clear();
                foreach (var item in originalItems)
                {
                    RingH.Items.Add(item);
                }
            }
        }
    }
}
