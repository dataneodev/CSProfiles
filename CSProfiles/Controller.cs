using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSProfiles
{
    class Controller
    {
        public const String ProgramName = "CJProfiles";
        public const float ProgramVersion = 1.0F;
        public const String DBName = "Profile.pdb";

        private readonly DBControl DB;
        public ObservableCollection<ProfilesNorme> normeList = new ObservableCollection<ProfilesNorme>();
        public ObservableCollection<ProfilesFamily> familyList = new ObservableCollection<ProfilesFamily>();
        public ObservableCollection<Profiles> profilesList = new ObservableCollection<Profiles>();
        public ObservableCollection<ProfileItem> listViewData = new ObservableCollection<ProfileItem>();

        public Controller()
        {
            DB = new DBControl(DBName);
        }

        /// <summary>
        /// Is database is loaded correct
        /// </summary>
        public bool IsDBActive()
        {
            return DB.IsDatabaseFileExists;
        }

        /// <summary>
        /// Load norme list from DB in to Controller.normeList
        /// </summary>
        public void LoadProfilesNorme()
        {
            profilesList.Clear();
            familyList.Clear();
            DB.GetProfilesNorme(normeList);
        }

        /// <summary>
        /// Load family list from DB in to Controller.familyList
        /// </summary>
        public void LoadProfilesFamily(ProfilesNorme selectedNorme)
        {
            profilesList.Clear();
            DB.GetProfilesFamily(familyList, selectedNorme);
        }

        /// <summary>
        /// Load profiles list from DB in to Controller.profilesList
        /// </summary>
        public void LoadProfilesList(ProfilesFamily selectedFamily)
        {
            DB.GetProfilesList(profilesList, selectedFamily);
        }

        /// <summary>
        /// Load profiles data from DB in to Controller.listViewData
        /// </summary>
        public void LoadProfilesData(Profiles selectedProfile)
        {
            DB.GetProfilesItems(listViewData, selectedProfile);
        }

        public void LoadImage(ProfilesFamily selectedFamily, System.Windows.Controls.Image image)
        {
            DB.GetprofilesImage(image, selectedFamily);
        }
    }
}
