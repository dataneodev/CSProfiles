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

namespace CSProfiles
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Controller MVC;

        public MainWindow()
        {
            MVC = new Controller();
            if (!MVC.IsDBActive())
            {
                MessageBox.Show("Database file doesn't exists.",
                    Controller.ProgramName + " " + Controller.ProgramVersion.ToString("0.0"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
            }

            InitializeComponent();
            BindingObj();

            MVC.LoadProfilesNorme();
            // autostart 
            if (normeCB.Items.Count>0) { normeCB.SelectedIndex = 0;}
        }

        private void BindingObj()
        {
            normeCB.ItemsSource = MVC.normeList;
            familyCB.ItemsSource = MVC.familyList;
            profilesCB.ItemsSource = MVC.profilesList;
            paramProfilesLV.ItemsSource = MVC.listViewData;
        }

        private void NormeCBChange(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            MVC.LoadProfilesFamily(cmb.SelectedItem as ProfilesNorme);
            if (familyCB.Items.Count>0) { familyCB.SelectedIndex = 0; }
        }

        private void FamilyCBChange(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            MVC.LoadProfilesList(cmb.SelectedItem as ProfilesFamily);
            if (profilesCB.Items.Count > 0) { profilesCB.SelectedIndex = 0; }
            MVC.LoadImage(cmb.SelectedItem as ProfilesFamily, imageSection);
        }

        private void ProfilesCBChange(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            MVC.LoadProfilesData(cmb.SelectedItem as Profiles);
        }
    }    
}
