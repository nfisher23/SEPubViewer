using SEPubViewer.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SEPubViewer.Views
{
    /// <summary>
    /// Interaction logic for SearchByTicker.xaml
    /// </summary>
    public partial class SearchByTicker : Page
    {
        public SearchByTicker()
        {
            InitializeComponent();
            SearchByTickerViewModel m = (SearchByTickerViewModel)this.DataContext;
            
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
