using System.Windows;
using System.Windows.Controls;

namespace CSProfiles
{
    public partial class MainWindow : Window
    {
        Controller MVC;
        // main start
        public MainWindow()
        {
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            MVC = new Controller();
            if (!MVC.IsDBActive())
            {
                MessageBox.Show("Database file doesn't exists.",
                    MVC.GetProgramName(),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
            }

            InitializeComponent();
            this.Title = MVC.GetProgramName();
            BindingObj();
            
            // start load data 
            MVC.LoadProfilesNorme();
            if (normeCB.Items.Count>0) { normeCB.SelectedIndex = 0;}
            UpdateChecks();
        }
        // binding controls with controlers ObservableCollection
        private void BindingObj()
        {
            normeCB.ItemsSource = MVC.normeList;
            familyCB.ItemsSource = MVC.familyList;
            profilesCB.ItemsSource = MVC.profilesList;
            paramProfilesLV.ItemsSource = MVC.listViewData;
        }

        // event on controls change
        private void NormeCBChange(object sender, SelectionChangedEventArgs e)
        {
            #if DEBUG
                Log.Notice(this.GetType().Name+"."+System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            ComboBox cmb = sender as ComboBox;
            MVC.LoadProfilesFamily(cmb.SelectedItem as ProfilesNorme);
            if (familyCB.Items.Count>0) { familyCB.SelectedIndex = 0; }
        }

        private void FamilyCBChange(object sender, SelectionChangedEventArgs e)
        {
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            ComboBox cmb = sender as ComboBox;
            ProfilesFamily ActiveFamily = cmb?.SelectedItem as ProfilesFamily;
            MVC.LoadProfilesList(ActiveFamily);
            if (profilesCB.Items.Count > 0) { profilesCB.SelectedIndex = 0; }
            MVC.LoadImage(ActiveFamily, imageSection);
            DxfGB.IsEnabled = MVC.HasDXFDrawer(cmb.SelectedItem as ProfilesFamily);
            familiDescTB.Text = ActiveFamily?.descryption ?? "";
        }

        private void ProfilesCBChange(object sender, SelectionChangedEventArgs e)
        {
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            ComboBox cmb = sender as ComboBox;
            MVC.LoadProfilesData(cmb.SelectedItem as Profiles);
        }

        private void OpenDxfBtClick(object sender, RoutedEventArgs e)
        {
            #if DEBUG
            Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            MVC.OpenDXFFile(profilesCB.SelectedItem as Profiles, frontViewChB.IsChecked.Value, 
                topViewChB.IsChecked.Value, sideViewChB.IsChecked.Value);
        }

        private void SaveDxfBtClick(object sender, RoutedEventArgs e)
        {
            #if DEBUG
            Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            MVC.SaveDXFFile(profilesCB.SelectedItem as Profiles, frontViewChB.IsChecked.Value,
                topViewChB.IsChecked.Value, sideViewChB.IsChecked.Value);
        }

        private void PopCopyValueClick(object sender, RoutedEventArgs e)
        {
            int selId = paramProfilesLV?.SelectedIndex ?? -1;
            ProfileItem selItem = selId != -1 && MVC.listViewData.Count > selId ? MVC.listViewData[selId] : null;
            #if DEBUG
            Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, 
                selItem != null ? selItem.paramValue : "selItem == null");
            #endif
            if(selItem != null)
            {
                Clipboard.SetText(selItem.paramValue);
            }
        }

        private void PopCopyItemClick(object sender, RoutedEventArgs e)
        {
            int selId = paramProfilesLV?.SelectedIndex ?? -1;
            ProfileItem selItem = selId != -1 && MVC.listViewData.Count > selId ? MVC.listViewData[selId] : null;
            #if DEBUG
            Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                selItem != null ? selItem.paramName + "=" + selItem.paramValue + selItem.paramUnit : "selItem == null");
            #endif
            if(selItem != null)
            {
                Clipboard.SetText(selItem.paramName+"="+selItem.paramValue+selItem.paramUnit);
            }
        }

        private void PopCopyAllClick(object sender, RoutedEventArgs e)
        {
            string clip="";
            foreach(ProfileItem item in MVC.listViewData)
            {
                clip += item.paramName + "=" + item.paramValue + item.paramUnit + System.Environment.NewLine;
            }
            Clipboard.SetText(clip);
            #if DEBUG
            Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, clip);
            #endif
        }

        private void ShowingPopMenu(object sender, ContextMenuEventArgs e)
        {
            #if DEBUG
            Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            int selId = paramProfilesLV?.SelectedIndex ?? -1;
            ProfileItem selItem = selId != -1 && MVC.listViewData.Count > selId ? MVC.listViewData[selId] : null;
            if(selItem != null)
            {
                popCopyValue.Header = "Copy value to clipboard ["+selItem.paramValue+"]";
                popCopyItem.Header = "Copy row to clipboard ["+selItem.paramName+"="+selItem.paramValue+selItem.paramUnit+"]";
            }
        }

        private void AppClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            #if DEBUG
            Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Closing the app");
            Log.SaveLog(); ;
            #endif
            MVC.Endcontroller();
        }

        private void HomePageClick(object sender, RoutedEventArgs e)
        {
            Updater.OpenHomePage();
        }

        private void UpdateChecks()
        {
            #if DEBUG
            Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            Updater.CheckUpdate(Controller.ProgramVersion, homepageL);
        }
    }    
}
