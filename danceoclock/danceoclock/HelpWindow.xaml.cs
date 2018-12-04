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

namespace danceoclock
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public bool isOpen;

        public HelpWindow()
        {
            InitializeComponent();
            isOpen = true;

            this.Closed += new EventHandler(NewAlarmWindow_Closed);
        }

        void NewAlarmWindow_Closed(object sender, EventArgs e)
        {
            isOpen = false;
        }
    }
}
