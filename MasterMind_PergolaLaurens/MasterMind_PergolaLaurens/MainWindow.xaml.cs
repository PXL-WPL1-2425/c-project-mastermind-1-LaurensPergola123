using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Mastermind
{
    public partial class MainWindow : Window
    {
        // Beschikbare kleuren voor het spel
        private readonly string[] colors = { "Red", "Yellow", "Orange", "White", "Green", "Blue" };
        private string[] secretCode; // Geheime code van het spel
        private int attempts; // Aantal pogingen van de speler
        private bool isDebugMode = false; // Debugmodus om geheime code te tonen

        private DispatcherTimer timer; // Timer voor tijdslimiet
        private int timerSeconds; // Huidige tijd in seconden

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeGame();
        }

        /// <summary>
        /// Initialiseert het spel en start de eerste ronde
        /// </summary>
        private void InitializeGame()
        {
            MessageBox.Show("Welkom bij Mastermind!\n\n" +
                "Spelregels:\n" +
                "- Je doel is om de geheime code van 4 kleuren te kraken.\n" +
                "- Kies kleuren uit het dropdown-menu en klik op 'Check code'.\n" +
                "- Rode hints: Kleur en positie correct.\n" +
                "- Witte hints: Kleur correct, maar verkeerde positie.\n" +
                "- Je hebt 10 seconden per beurt. Bij overschrijding verlies je een beurt.\n" +
                "- Je kan maar maximaal 10 beurten proberen.\n" +
                "- Win door de volledige code correct te raden.\n\n" +
                "Veel succes!",
                "Spelregels",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            secretCode = GenerateSecretCode();
            attempts = 1;
            UpdateTitle();
            UpdateDebugTextBox();
            StartCountdown();
            
        }

        /// <summary>
        /// Genereert een willekeurige geheime code bestaande uit 4 kleuren
        /// </summary>
        private string[] GenerateSecretCode()
        {
            Random rand = new Random();
            string[] code = new string[4];
            for (int i = 0; i < code.Length; i++)
            {
                code[i] = colors[rand.Next(colors.Length)];
            }
            return code;
        }

        /// <summary>
        /// Werkt de titel van het venster bij om de huidige poging te tonen
        /// </summary>
        private void UpdateTitle()
        {
            this.Title = $"Mastermind - Poging {attempts}";
        }

        /// <summary>
        /// Controleert of het spel voorbij is
        /// </summary>
        private bool HasMaxAttempts()
        {
            if (attempts > 10)
            {
                StopCountdown();
                MessageBox.Show("Je hebt het maximum aantal pogingen behaald. Probeer opnieuw!", "Game Over", MessageBoxButton.OK, MessageBoxImage.Warning);
                InitializeGame();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Start een nieuwe beurt, reset de timer en start opnieuw
        /// </summary>
        private void StartNewRound()
        {
            attempts++;
            UpdateTitle();
            if (HasMaxAttempts()) return;
            StartCountdown();
        }

        /// <summary>
        /// Eventhandler voor kleurselectie in een ComboBox. Past de achtergrondkleur van het corresponderende label aan.
        /// </summary>
        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedColor = selectedItem.Content.ToString();

                Label targetLabel = null;
                if (comboBox == color1) targetLabel = color1Label;
                else if (comboBox == color2) targetLabel = color2Label;
                else if (comboBox == color3) targetLabel = color3Label;
                else if (comboBox == color4) targetLabel = color4Label;

                if (targetLabel != null)
                {
                    targetLabel.Background = (Brush)new BrushConverter().ConvertFromString(selectedColor);
                }
            }
        }
        /// <summary>
        /// Werkt de debugtekstbox bij om de geheime code te tonen of te verbergen
        /// </summary>
        private void UpdateDebugTextBox()
        {
            if (isDebugMode)
            {
                debugTextBox.Text = $"Geheime code: {string.Join(", ", secretCode)}";
                debugTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                debugTextBox.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Toggled de debugmodus in de app 
        /// Als de debugmodus is ingeschakeld wordt geheime code zichtbaar in de debugTextBox
        /// </summary>
        private void ToggleDebug()
        {
            isDebugMode = !isDebugMode;
            UpdateDebugTextBox();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.F12)
            {
                ToggleDebug();
            }
        }

        /// <summary>
        /// Eventhandler voor de "Check" knop. Controleert de gok van de speler tegen de geheime code
        /// </summary>
        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            
            UpdateTitle();

            if (timerSeconds >= 10)
            {
                StopCountdown();
                MessageBox.Show("Tijd is om! Je beurt is voorbij.", "Verlies van beurt", MessageBoxButton.OK, MessageBoxImage.Warning);
                attempts++;
                if (HasMaxAttempts()) return;
                StartCountdown();
                StartNewRound();
                UpdateTitle();
                return;
            }

            string[] playerGuess = {
                (color1.SelectedItem as ComboBoxItem)?.Content.ToString(),
                (color2.SelectedItem as ComboBoxItem)?.Content.ToString(),
                (color3.SelectedItem as ComboBoxItem)?.Content.ToString(),
                (color4.SelectedItem as ComboBoxItem)?.Content.ToString()
            };

            if (Array.Exists(playerGuess, item => item == null))
            {
                resultTextBox.Text = "Selecteer vier kleuren!";
                return;
            }

            ResetLabelBorders();

            int correctPosition = 0;
            int correctColor = 0;

            string[] tempSecretCode = (string[])secretCode.Clone();
            string[] tempPlayerGuess = (string[])playerGuess.Clone();

            // Controleer op correcte kleur en positie (rood)
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

            // Controleer op correcte kleur maar verkeerde positie (wit)
            for (int i = 0; i < 4; i++)
            {
                if (tempPlayerGuess[i] != null && Array.Exists(tempSecretCode, color => color == tempPlayerGuess[i]))
                {
                    correctColor++;
                    tempSecretCode[Array.IndexOf(tempSecretCode, tempPlayerGuess[i])] = null;

                    Label label = GetLabelByIndex(i);
                    label.BorderBrush = Brushes.Wheat;
                    label.BorderThickness = new Thickness(6);
                }
            }

            resultTextBox.Text = $"Rode hints: {correctPosition}, Witte hints: {correctColor}";

            // Controleer op winst
            if (correctPosition == 4)
            {
                StopCountdown();
                timerTextBox.Text = "CODE GEKRAAKT!";
                MessageBox.Show($"Gefeliciteerd! Je hebt de code gekraakt in {attempts} beurten!", "Gewonnen");
                InitializeGame();
                return;
            }

            attempts++;
            if (HasMaxAttempts()) return;
            StartCountdown();
        }

        /// <summary>
        /// Reset de borders van alle labels
        /// </summary>
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
        /// <summary>
        /// Controleert of het spel voorbij is (maximum aantal pogingen bereikt)
        /// </summary>
        private bool IsGameOver()
        {
            if (attempts > 10)
            {
                StopCountdown();
                MessageBox.Show($"Je hebt het maximale aantal pogingen bereikt.\nDe geheime code was: {string.Join(", ", secretCode)}", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                InitializeGame();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Initialiseert de timer
        /// </summary>
        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// Start/herstart de timer bij elke nieuwe poging en in het begin
        /// </summary>
        private void StartCountdown()
        {
            timer.Stop();
            timerSeconds = 0;
            timerTextBox.Text = $"TIJD: {timerSeconds}";
            timer.Start();
        }

        /// <summary>
        /// Stopt de timer als de tijd is verstreken en reset de tijd
        /// </summary>
        private void StopCountdown()
        {
            timer.Stop();
            timerSeconds = 0;
            timerTextBox.Text = "TIJD: 0";
        }

        /// <summary>
        /// Eventhandler voor de timer. Controleert of de tijdslimiet is bereikt.
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            timerSeconds++;
            timerTextBox.Text = $"TIJD: {timerSeconds}";

            if (timerSeconds >= 10)
            {
                StopCountdown();
                timerTextBox.Text = "TE LAAT!";
                MessageBox.Show("Tijd is om! Je beurt is voorbij.", "Verlies van beurt", MessageBoxButton.OK, MessageBoxImage.Warning);
                attempts++;
                if (HasMaxAttempts()) return;
                StartCountdown();
            }
        }
    }
}
