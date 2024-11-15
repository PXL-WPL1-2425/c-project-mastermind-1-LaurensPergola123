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

            ResetLabelBorders();

            int correctPosition = 0;
            int correctColor = 0;

            List<string> tempSecretCode = new List<string>(secretCode);
            List<string> tempPlayerGuess = new List<string>(playerGuess);

            for (int i = 0; i < 4; i++)
            {
                if (tempPlayerGuess[i] == tempSecretCode[i])
                {
                    correctPosition++;
                    tempSecretCode[i] = null;
                    tempPlayerGuess[i] = null;

                    Label label = GetLabelByIndex(i);
                    label.BorderBrush = Brushes.DarkRed;
                    label.BorderThickness = new Thickness(6);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (tempPlayerGuess[i] != null && tempSecretCode.Contains(tempPlayerGuess[i]))
                {
                    correctColor++;
                    tempSecretCode[tempSecretCode.IndexOf(tempPlayerGuess[i])] = null;

                    Label label = GetLabelByIndex(i);
                    label.BorderBrush = Brushes.Wheat;
                    label.BorderThickness = new Thickness(6);
                }
            }

            resultTextBlock.Text = $"Rode hints: {correctPosition * 2}, Witte hints: {correctColor}";

            if (correctPosition == 4)
            {
                MessageBox.Show("Gefeliciteerd! Je hebt de code gekraakt!", "Gewonnen");
                InitializeGame();
            }
        }

        private void ResetLabelBorders()
        {
            color1Label.BorderBrush = Brushes.Transparent;
            color2Label.BorderBrush = Brushes.Transparent;
            color3Label.BorderBrush = Brushes.Transparent;
            color4Label.BorderBrush = Brushes.Transparent;

            color1Label.BorderThickness = new Thickness(0);
            color2Label.BorderThickness = new Thickness(0);
            color3Label.BorderThickness = new Thickness(0);
            color4Label.BorderThickness = new Thickness(0);
        }

        private Label GetLabelByIndex(int index)
        {
            return index switch
            {
                0 => color1Label,
                1 => color2Label,
                2 => color3Label,
                3 => color4Label,
                _ => null
            };
        }
    }
}
