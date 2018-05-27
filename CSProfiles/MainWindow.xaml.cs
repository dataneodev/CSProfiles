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
        public List<ProfilesNorme> normeList;
        public List<ProfilesFamily> familyList;
        public List<Profiles> profilesList;
        public List<ProfileItem> ListViewData;
        
        public MainWindow()
        {
            InitializeComponent();

            // test 
            ListViewData = new List<ProfileItem>();
            ListViewData.Add(new ProfileItem("A", "123", "mm"));
            normeList = new List<ProfilesNorme>() { new ProfilesNorme(1, "Eurocode", "EC") };

            // binding ui controls to list
            normeCB.ItemsSource = normeList;
            familyCB.ItemsSource = familyList;
            profilesCB.ItemsSource = profilesList;
            paramProfilesLV.ItemsSource = ListViewData;
        }

    }
}
