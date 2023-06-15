using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KPO
{
    /// <summary>
    /// Interaction logic for DialogBoxWithResult.xaml
    /// </summary>
    public partial class DialogBoxWithResult : Window
    {
        public DialogBoxWithResult()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Savior_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Returnal_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
