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
using System.Windows.Forms;

namespace danceoclock
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class NewAction : Window
    {
        MainWindow parent = null;

        public NewAction(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        private void browsePathButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                dirTextBox.Text = dialog.SelectedPath;
            }
            
        }

        private void recordButton_Click(object sender, RoutedEventArgs e)
        {
            KinectWindow kinectWindow = new KinectWindow(parent, dirTextBox.Text + "\\" + fileNameTextBox.Text + ".txt", 1, 2);
            /*CurrentNumFrames = 0;
            Sec = 1;
            Length = 2;
            */
            kinectWindow.Show();
            Close();
        }
    }
}
