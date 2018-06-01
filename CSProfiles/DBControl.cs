using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CSProfiles
{

    public class Profiles
    {
        public int id { get; private set; }
        public int profileNormeId { get; private set; }
        public int profileFamilyId { get; private set; }
        public String profileName { get; private set; }

        public Profiles(int id, int profileNormeId, int profileFamilyId, String profileName)
        {
            this.id = id;
            this.profileNormeId = profileNormeId;
            this.profileFamilyId = profileFamilyId;
            this.profileName = profileName;
        }
    }

    public class ProfileItem
    {
        public String paramName { get; private set; }
        public String paramValue { get; private set; }
        public String paramUnit { get; private set; }

        public ProfileItem(string paramName, string paramValue, string paramUnit)
        {
            this.paramName = paramName;
            this.paramValue = paramValue;
            this.paramUnit = paramUnit;
        }
    }

    public class ProfilesFamily
    {
        public int id { get; private set; }
        public String profileName { get; private set; }
        public int profileNormeId { get; private set; }
        public int profileImageId { get; private set; }
        public int profileDrawerId { get; private set; }

        public ProfilesFamily(int id, String profileName, int profileNormeId, int profileImageId, int profileDrawerId)
        {
            this.id = id;
            this.profileName = profileName;
            this.profileNormeId = profileNormeId;
            this.profileImageId = profileImageId;
            this.profileDrawerId = profileDrawerId;
        }
    }

    public class ProfilesImage
    {
        public int id { get; private set; }
        public byte[] profilesImage { get; private set; }
    }

    public class ProfilesNorme
    {
        public int id { get; private set; }
        public String normeName { get; private set; }
        public String code { get; private set; }

        public ProfilesNorme(int id, String normeName, String code)
        {
            this.id = id;
            this.normeName = normeName;
            this.code = code;
        }
    }

    sealed public class DBControl {
        public bool IsDatabaseFileExists { get; private set; }
        public String DBPath { get; private set; }
        private SQLiteConnection DBConnection;

        public DBControl(String databaseName)
        {
            DBPath = GetDBPath(databaseName);
            IsDatabaseFileExists = File.Exists(DBPath) ? true : false;
            DBConnection = ConnectDB();
            DBConnection.Open();
        }

        private SQLiteConnection ConnectDB()
        {
            return IsDatabaseFileExists ? new SQLiteConnection("Data Source=" + DBPath + ";Version=3;") : null;
        }

        private String GetDBPath(string DBName)
        {
            String path = Environment.GetCommandLineArgs()[0];
            String dir = Path.GetDirectoryName(path);

            if (dir.Contains(Path.DirectorySeparatorChar))
                if (!dir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    dir += Path.DirectorySeparatorChar;

            if (dir.Contains(Path.AltDirectorySeparatorChar))
                if (!dir.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                    dir += Path.AltDirectorySeparatorChar;
            return dir + DBName;
        }

        public void GetProfilesNorme(ObservableCollection<ProfilesNorme> listNorme)
        {
            if (DBConnection == null) throw new NullReferenceException();
            
            SQLiteCommand sqlcmd = DBConnection.CreateCommand();
            sqlcmd.CommandText = "SELECT * FROM TNORME";

            listNorme.Clear();
            try
            {
                SQLiteDataReader dataReader = sqlcmd.ExecuteReader();
                while (dataReader.Read())
                {
                    listNorme.Add(new ProfilesNorme(Convert.ToInt32(dataReader["id"]),
                    Convert.ToString(dataReader["nname"]), Convert.ToString(dataReader["code"])));
                }
            }
            catch (SQLiteException e)
            {
                // log
                listNorme.Clear();
            }
        }

        public void GetProfilesFamily(ObservableCollection<ProfilesFamily> listFamily, ProfilesNorme selectedNorme)
        {
            if (DBConnection == null) throw new NullReferenceException();
            listFamily.Clear();
            if (selectedNorme == null) return;

            SQLiteCommand sqlcmd = DBConnection.CreateCommand();
            sqlcmd.CommandText = "SELECT * FROM TFAMILY WHERE normeid=" + selectedNorme.id.ToString();
            try
            {
                SQLiteDataReader dataReader = sqlcmd.ExecuteReader();
                while (dataReader.Read())
                {
                    listFamily.Add(new ProfilesFamily(Convert.ToInt32(dataReader["id"]),
                    Convert.ToString(dataReader["fname"]), Convert.ToInt32(dataReader["normeid"]),
                    Convert.ToInt32(dataReader["imageid"]), Convert.ToInt32(dataReader["drawerid"])));
                }

            }
            catch (SQLiteException e)
            {
                // log
                listFamily.Clear();
            }
        }

        public void GetProfilesList(ObservableCollection<Profiles> profilesList, ProfilesFamily selectedFamily)
        {
            if (DBConnection == null) throw new NullReferenceException();

            profilesList.Clear();
            if (selectedFamily == null) return;

            SQLiteCommand sqlcmd = DBConnection.CreateCommand();
            sqlcmd.CommandText = "SELECT id, normeid, familyid, profilename FROM TPROFILES WHERE familyid=" + selectedFamily.id.ToString();
            try
            {
                SQLiteDataReader dataReader = sqlcmd.ExecuteReader();
                while (dataReader.Read())
                {
                    profilesList.Add(new Profiles(Convert.ToInt32(dataReader["id"]),
                    Convert.ToInt32(dataReader["normeid"]), Convert.ToInt32(dataReader["familyid"]),
                    Convert.ToString(dataReader["profilename"])));
                }
            }
            catch (SQLiteException e)
            {
                // log
               profilesList.Clear();
            }
        }

        public void GetProfilesItems(ObservableCollection<ProfileItem> listViewData, Profiles selectedProfile)
        {
            if (DBConnection == null) throw new NullReferenceException();

            listViewData.Clear();
            if (selectedProfile == null) return;

            SQLiteCommand sqlcmd = DBConnection.CreateCommand();
            sqlcmd.CommandText = "SELECT id, characteristic FROM TPROFILES WHERE id=" + selectedProfile.id.ToString();
            try
            {
                SQLiteDataReader dataReader = sqlcmd.ExecuteReader();
                dataReader.Read();
                String profileParam = dataReader.GetString(1);
                String[] paramLine = profileParam.Split(';');
                foreach (String line in paramLine)
                {
                    String[] lineParam = line.Split('=');
                    if (lineParam.Length != 3)
                    {
                        // log
                        continue;
                    }
                    String unit = lineParam[2].Replace("m3", "m\x00B3").Replace("m2", "m\x00B2");
                    listViewData.Add(new ProfileItem(lineParam[0], lineParam[1], unit));
                }
            }
            catch (SQLiteException e)
            {
                // log
                listViewData.Clear();
            }
        }

        public void GetprofilesImage(System.Windows.Controls.Image image, ProfilesFamily selectedFamily)
        {
            if (DBConnection == null) throw new NullReferenceException();

            image.Source = null;

            if (selectedFamily == null) return;
            if (selectedFamily.profileImageId == 0) return; 

            SQLiteCommand sqlcmd = DBConnection.CreateCommand();
            sqlcmd.CommandText = "SELECT * FROM TIMAGE WHERE id=" + selectedFamily.profileImageId.ToString();
            try
            {
                SQLiteDataReader dataReader = sqlcmd.ExecuteReader();
                if (dataReader.Read())
                {
                    image.Source = LoadImage((byte[])dataReader["imagedata"]);
                }
            }
            catch (SQLiteException e)
            {
                // log
               
            }
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }




        /*
        private class InnerSQL<ListType>
        {
            InnerSQL(SQLiteConnection Con, String SQLQuery, ListType list)
            {
                if (Con == null)
                {
                    throw new NullReferenceException();
                }

                Con.Open();
                SQLiteCommand sqlcmd = Con.CreateCommand();
                sqlcmd.CommandText = SQLQuery;

                List(list.Clear();
                try
                {
                    SQLiteDataReader dataReader = sqlcmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        listNorme.Add(new ProfilesNorme(Convert.ToInt32(dataReader["id"]),
                        Convert.ToString(dataReader["nname"]), Convert.ToString(dataReader["code"])));
                    }
                }
                catch (SQLiteException e)
                {
                    // log
                    listNorme.Clear();
                }

            } 
        }*/






    }
}
