using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SharpList
{
    /// <summary>
    /// Interaction logic for DialogBox.xaml
    /// </summary>
    public partial class DialogBox : Window
    {
        public bool validated;

        public DialogBox()
        {
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            validated = false;
            Close();
        }

        private void validateButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameTextBox.Text.Length > 0 && nameTextBox.Text.Length < 31)
            {
                validated = true;
                Close();
            }
            else
            {
                errorMessage.Content = "Length must be between 1 and 30 !";
            }
        }
    }
}
