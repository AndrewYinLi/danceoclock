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

namespace danceoclock {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// 

    public partial class NewAlarmWindow : Window {

        private MainWindow parent;

        public NewAlarmWindow(MainWindow parent) {
            InitializeComponent();
            musicPathTextBox.TextWrapping = TextWrapping.NoWrap;
            this.parent = parent;
            
        }

        private void browseMusicButton_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true) {
                musicPathTextBox.Text = dlg.FileName;
            }
        }

        private void browseActionTextBox_Click(object sender, RoutedEventArgs e) {

        }

        private void createAlarmButton_Click(object sender, RoutedEventArgs e) {
            parent.createNewAlarm(musicPathTextBox.Text, );
        }
    }
}
