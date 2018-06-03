using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

        private readonly DBControl DB = new DBControl(DBName);
        private readonly Drawers D3D = new Drawers();

        public ObservableCollection<ProfilesNorme> normeList = new ObservableCollection<ProfilesNorme>();
        public ObservableCollection<ProfilesFamily> familyList = new ObservableCollection<ProfilesFamily>();
        public ObservableCollection<Profiles> profilesList = new ObservableCollection<Profiles>();
        public ObservableCollection<ProfileItem> listViewData = new ObservableCollection<ProfileItem>();

        public Controller()
        {
          
        }

        /// <summary>
        /// Is database is loaded correct
        /// </summary>
        public bool IsDBActive()
        {
            return DB.IsDatabaseFileExists;
        }

        /// <summary>
        /// Open DXF File
        /// </summary>
        public void OpenDXFFile(Profiles selectedProfile, bool frontView, 
            bool topView, bool sideView)
        {
            #if DEBUG
            Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif

            if(selectedProfile == null)
            {
                #if DEBUG
                Log.Warning(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "selectedProfile == null");
                #endif
                return;
            }

            ProfilesFamily profileFamily = null;
            foreach(ProfilesFamily family in familyList)
            {
                if(family.id == selectedProfile.profileFamilyId)
                {
                    profileFamily = family;
                }
            }

            if(profileFamily == null)
            {
                #if DEBUG
                Log.Warning(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "profileFamily == null");
                #endif
                return;
            }

            if (!D3D.IsFamilyHasDxfDrawer(profileFamily))
            {
                //MessageBox.Show("DXF for this profile is not possible",
                //MVC.GetProgramName(),MessageBoxButton.OK, MessageBoxImage.Warning);
                #if DEBUG
                Log.Warning(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "!D3D.IsFamilyHasDxfDrawer(profileFamily)");
                #endif
                return;
            }
        }

        /// <summary>
        /// Save DXF File
        /// </summary>
        public void SaveDXFFile(Profiles selectedProfile, bool frontView,
            bool topView, bool sideView)
        {
            #if DEBUG
            Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif


        }

        /// <summary>
        /// Is Profiles family has DXF Draawer
        /// </summary>
        public bool HasDXFDrawer(ProfilesFamily checkedFamily)
        {
             return D3D.IsFamilyHasDxfDrawer(checkedFamily);
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

        /// <summary>
        /// Load profiles image from DB in to image
        /// </summary>
        public void LoadImage(ProfilesFamily selectedFamily, System.Windows.Controls.Image image)
        {
            DB.GetprofilesImage(image, selectedFamily);
        }

        /// <summary>
        ///Get program full name with versiob eg. CSProfile 1.0
        /// </summary>
        public String GetProgramName()
        {
            return ProgramName + " " + ProgramVersion.ToString("0.0").Replace(",", ".");
        }
    }

#if DEBUG
    // loggin class
    public static class Log
    {
        private static List<string> logger = new List<string>();
        
        public static void Notice(String path, String msg)
        { AddMsg("NOTICE " + path + (msg.Length > 0 ? " : " + msg : "") );  }
        public static void Notice(String path)
        { Notice(path, ""); }

        public static void Warning(String path, String msg)
        { AddMsg("WARNING " + path + (msg.Length > 0 ? " : " + msg : "")); }
        public static void warning(String path)
        { Warning(path, ""); }

        public static void Error(String path, String msg)
        { AddMsg("ERROR " + path + (msg.Length > 0 ? " : " + msg : "")); }
        public static void Error(String path)
        { Error(path,""); }

        private static void AddMsg(String msg)
        { logger.Add(DateTime.Now.ToString() + " " + msg); }

        public static void SaveLog()
        {
            String logPath = GKCommon.GetProgramPath() + Controller.ProgramName + "_" + DateTime.Now.ToString().Replace(":","-") + ".txt";
            try
            {
                StreamWriter outputFile = new StreamWriter(logPath);
                for(int i=0; i < logger.Count; i++)
                {
                    outputFile.WriteLine(logger[i].ToString());
                }
                outputFile.Close();
            }
            catch(IOException)
            {
                // nothing
            }
            finally
            {
                logger.Clear();
            }
        }
    }
#endif
    // MY common functions
    public static class GKCommon
    {
        /// <summary>
        /// Gets directory path to running exe eg. C:\MyProgram\
        /// </summary>
        public static String GetProgramPath()
        {
            String path = Environment.GetCommandLineArgs()[0];
            String dir = Path.GetDirectoryName(path);

            if (dir.Contains(Path.DirectorySeparatorChar))
                if (!dir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    dir += Path.DirectorySeparatorChar;

            if (dir.Contains(Path.AltDirectorySeparatorChar))
                if (!dir.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                    dir += Path.AltDirectorySeparatorChar;
            return dir;
        }
    }
}
