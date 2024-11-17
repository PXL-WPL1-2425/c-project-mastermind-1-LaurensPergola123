using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mastermind
{
    public partial class MainWindow : Window
    {
        private List<string> colors = new List<string> { "Red", "Yellow", "Orange", "White", "Green", "Blue" };
        private List<string> secretCode;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            secretCode = GenerateSecretCode();
            this.Title = "Mastermind - Oplossing: " + string.Join(", ", secretCode);
        }

        private List<string> GenerateSecretCode()
        {
            Random rand = new Random();
            List<string> code = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                code.Add(colors[rand.Next(colors.Count)]);
            }
            return code;
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedColor = selectedItem.Content.ToString();

                Label targetLabel = null;
                if (comboBox == Color1) targetLabel = color1Label;
                else if (comboBox == Color2) targetLabel = color2Label;
                else if (comboBox == Color3) targetLabel = color3Label;
                else if (comboBox == Color4) targetLabel = color4Label;

                if (targetLabel != null)
                {
                    targetLabel.Background = (Brush)new BrushConverter().ConvertFromString(selectedColor);
                }
            }
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> playerGuess = new List<string>
            {
                (Color1.SelectedItem as ComboBoxItem)?.Content.ToString(),
                (Color2.SelectedItem as ComboBoxItem)?.Content.ToString(),
                (Color3.SelectedItem as ComboBoxItem)?.Content.ToString(),
                (Color4.SelectedItem as ComboBoxItem)?.Content.ToString()
            };

            if (playerGuess.Contains(null))
            {
                resultTextBlock.Text = "Selecteer vier kleuren!";
                return;
            }
        }
    }
}
