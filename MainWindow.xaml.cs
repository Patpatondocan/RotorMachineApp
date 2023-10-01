using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Text;

namespace RotorMachineApp
{
    public partial class MainWindow : Window
    {
        private readonly ControlRing controlRing = new();
        private readonly ShuffledRing ringH = new();
        private readonly ShuffledRing ringM = new();
        private readonly ShuffledRing ringS = new();

        private readonly List<char> originalControlItems = new(); // Store the original items for ControlRing
        private readonly List<char> originalRingHItems = new(); // Store the original items for RingH
        private readonly List<char> originalRingMItems = new(); // Store the original items for RingM
        private readonly List<char> originalRingSItems = new(); // Store the original items for RingS

        public MainWindow()
        {
            InitializeComponent();
            LoadAlphabet();
            SubscribeTextChangedEvents();
            SubscribePreviewTextInputEvents();
        }

        private void LoadAlphabet()
        {
            // Load the ControlRing items into the Control ListView
            foreach (char c in controlRing.Items)
            {
                Control.Items.Add(c);
                originalControlItems.Add(c); // Store the original items
            }

            // Load the ShuffledRing items into the RingH, RingM, and RingS ListViews
            foreach (char c in ringH.Items)
            {
                RingH.Items.Add(c);
                originalRingHItems.Add(c); // Store the original items
            }

            foreach (char c in ringM.Items)
            {
                RingM.Items.Add(c);
                originalRingMItems.Add(c); // Store the original items
            }

            foreach (char c in ringS.Items)
            {
                RingS.Items.Add(c);
                originalRingSItems.Add(c); // Store the original items
            }
        }

        private void SubscribeTextChangedEvents()
        {
            TextBoxH.TextChanged += TextBoxH_TextChanged;
            TextBoxM.TextChanged += TextBoxM_TextChanged;
            TextBoxS.TextChanged += TextBoxS_TextChanged;
        }

        private void SubscribePreviewTextInputEvents()
        {
            TextBoxH.PreviewTextInput += ValidateRingInput;
            TextBoxM.PreviewTextInput += ValidateRingInput;
            TextBoxS.PreviewTextInput += ValidateRingInput;
        }

        private void TextBoxH_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateRingPosition(TextBoxH, RingH, originalRingHItems);
        }

        private void TextBoxM_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateRingPosition(TextBoxM, RingM, originalRingMItems);
        }

        private void TextBoxS_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateRingPosition(TextBoxS, RingS, originalRingSItems);
        }

        private static void UpdateRingPosition(TextBox textBox, ListView ring, List<char> originalItems)
        {
            if (int.TryParse(textBox.Text, out int newValue))
            {
                // Check if the value is 0 or null and reset the ring to its original state
                if (newValue == 0 || string.IsNullOrWhiteSpace(textBox.Text))
                {
                    // Clear the items in the ring and add the original items back
                    ring.Items.Clear();
                    foreach (var item in originalItems)
                    {
                        ring.Items.Add(item);
                    }
                }
                else
                {
                    // Ensure the input is within the range [0, 25]
                    newValue = Math.Max(0, Math.Min(newValue, 25));

                    // Calculate the number of positions to shift
                    int shiftAmount = newValue;

                    // Shift the items in the ring counterclockwise
                    var items = ring.Items.Cast<char>().ToList();
                    items = items.Skip(shiftAmount).Concat(items.Take(shiftAmount)).ToList();

                    // Clear the items in the ring and add the shifted items
                    ring.Items.Clear();
                    foreach (var item in items)
                    {
                        ring.Items.Add(item);
                    }
                }
            }
        }

        private void ValidateRingInput(object sender, TextCompositionEventArgs e)
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

        private string EncryptMessage(string inputMessage)
        {
            // Initialize the encrypted message as an empty string
            StringBuilder encryptedMessage = new();

            // Process each character in the input message
            foreach (char inputChar in inputMessage)
            {
                // Find the position of the inputChar in the Control ring
                int controlIndex = Control.Items.IndexOf(inputChar);

                if (controlIndex != -1)
                {
                    // Get the character at the same position in RingH
                    char ringHChar = (char)RingH.Items[controlIndex];

                    // Find the position of ringHChar in the Control ring
                    int controlIndex2 = Control.Items.IndexOf(ringHChar);

                    if (controlIndex2 != -1)
                    {
                        // Get the character at the same position in RingM
                        char ringMChar = (char)RingM.Items[controlIndex2];

                        // Find the position of ringMChar in the Control ring
                        int controlIndex3 = Control.Items.IndexOf(ringMChar);

                        if (controlIndex3 != -1)
                        {
                            // Get the character at the same position in RingS
                            char ringSChar = (char)RingS.Items[controlIndex3];

                            // Append the encrypted character to the result
                            encryptedMessage.Append(ringSChar);
                        }
                    }
                }
            }

            return encryptedMessage.ToString();
        }

        private string MirrorEncryptMessage(string inputMessage)
        {
            // Initialize the mirrored encrypted message as an empty string
            StringBuilder mirroredEncryptedMessage = new StringBuilder();

            // Process each character in the input message
            foreach (char inputChar in inputMessage)
            {
                // Find the position of the inputChar in RingS
                int ringSIndex = RingS.Items.IndexOf(inputChar);

                if (ringSIndex != -1)
                {
                    // Get the character at the same position in RingM
                    char ringMChar = (char)RingM.Items[ringSIndex];

                    // Find the position of ringMChar in RingM
                    int ringMIndex = RingM.Items.IndexOf(ringMChar);

                    if (ringMIndex != -1)
                    {
                        // Get the character at the same position in RingH
                        char ringHChar = (char)RingH.Items[ringMIndex];

                        // Append the mirrored encrypted character to the result
                        mirroredEncryptedMessage.Append(ringHChar);
                    }
                }
            }

            return mirroredEncryptedMessage.ToString();
        }

        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Encrypt the message
            string encryptedMessage = EncryptMessage(InputBox.Text);

            // Display the encrypted message in reverse order
            EncryptedBlock.Text = new string(encryptedMessage.Reverse().ToArray());

            // Mirror the encrypted message
            string mirroredEncryptedMessage = MirrorEncryptMessage(InputBox.Text);

            // Display the mirrored encrypted message
            MirrorEncryptedBlock.Text = new string(mirroredEncryptedMessage.Reverse().ToArray());
        }

        public class ControlRing
                {
                    public List<char> Items { get; } = new List<char>();

                    public ControlRing()
                    {
                        // Load the fixed alphabet "ABCDEFGHIJKLMNOPQRSTUVWXYZ" into the ControlRing
                        for (char c = 'A'; c <= 'Z'; c++)
                        {
                            Items.Add(c);
                        }
                    }
                }

        public class ShuffledRing
        {
            public List<char> Items { get; } = new List<char>();

            public ShuffledRing()
            {
                // Create a list of characters representing the alphabet
                List<char> alphabet = new List<char>("ABCDEFGHIJKLMNOPQRSTUVWXYZ");

                // Shuffle the alphabet randomly
                Random rng = new Random();
                int n = alphabet.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    char value = alphabet[k];
                    alphabet[k] = alphabet[n];
                    alphabet[n] = value;
                }

                // Load the shuffled alphabet into the ShuffledRing
                foreach (char c in alphabet)
                {
                    Items.Add(c);
                }
            }
        }
    }
}
