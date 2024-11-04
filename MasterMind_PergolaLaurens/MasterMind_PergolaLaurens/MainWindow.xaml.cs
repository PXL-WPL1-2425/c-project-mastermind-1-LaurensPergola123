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
    }
}

     

            
