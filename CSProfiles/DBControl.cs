using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CSProfiles
{
    // Object definitions
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
        public String paramName { get; set; }
        public String paramNameNor { get; set; }
        public String paramNameSup { get; set; }
        public String paramValue { get; set; }
        public String paramUnit { get; set; }
        public String paramUnitNor1 { get; set; }
        public String paramUnitNor2 { get; set; }
        public String paramUnitSup { get; set; }
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
    // SQLite database class
    sealed public class DBControl {
        public bool IsDatabaseFileExists { get; private set; }
        public String DBPath { get; private set; }
        private SQLiteConnection DBConnection;

        public DBControl(String databaseName)
        {
            DBPath = GetDBPath(databaseName);
            IsDatabaseFileExists = File.Exists(DBPath) ? true : false;
            DBConnection = ConnectDB();
            if (IsDatabaseFileExists) { DBConnection.Open(); }
        }

        private SQLiteConnection ConnectDB()
        {
            return IsDatabaseFileExists ? new SQLiteConnection("Data Source=" + DBPath + ";Version=3;") : null;
        }

        private String GetDBPath(string DBName)
        {
            return GKCommon.GetProgramPath() + DBName;
        }

        public void GetProfilesNorme(ObservableCollection<ProfilesNorme> listNorme)
        {
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            if (listNorme == null)
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "listNorme == null"); 
                #endif
                return;
            }
            listNorme.Clear();
            if (DBConnection == null)
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "DBConnection == null");
                #endif
                return;
            }

            SQLiteCommand sqlcmd = DBConnection.CreateCommand();
            sqlcmd.CommandText = "SELECT * FROM TNORME";
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
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "SQL Error: " + e.Message.ToString());
                #endif
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
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "SQL Error: " + e.Message.ToString());
                #endif
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
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "SQL Error: " + e.Message.ToString());
                #endif

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
                if (dataReader.Read())
                {
                    String profileParam = dataReader.GetString(1);
                    String[] paramLine = profileParam.Split(';');
                    foreach (String line in paramLine)
                    {
                        if(line.Length == 0) { continue; }
                        String[] lineParam = line.Split('=');
                        if (lineParam.Length != 3)
                        {
                            #if DEBUG
                                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                    "lineParam.Length != 3 : " + line);
                            #endif
                            continue;
                        }
                        listViewData.Add(ProfileItemFormat(lineParam[0], lineParam[1], lineParam[2]));
                    }
                }
            }
            catch (SQLiteException e)
            {
                #if DEBUG
                    Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "SQL Error: " + e.Message.ToString());
                #endif
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
            sqlcmd.CommandText = "SELECT * FROM TIMAGE WHERE id=" + selectedFamily.profileImageId.ToString() + " LIMIT 1";
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
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "SQL Error: " + e.Message.ToString());
                #endif
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

        private ProfileItem ProfileItemFormat(String Name, String Value, String Unit)
        {
            String nameNor = Name;
            String nameSup = "";
            if (Name.Length > 0)
            {
                if (Name[0] != '(')
                {
                    nameNor = Name[0].ToString();
                    nameSup = Name.Substring(1);
                } else if (Name.IndexOf(')') != -1)
                {
                    nameNor = Name.Substring(0, Name.IndexOf(')')+1);
                    nameSup = Name.Substring(Name.IndexOf(')')+1);
                }
            }

            String unitNor1 = Unit;
            String unitNor2 = "";
            String unitSup = "";

            if (Unit.Length > 0)
            {
                Match match = Regex.Match(Unit, "m[1-9]");
                if (match.Success)
                {
                    unitNor1 = Unit.Substring(0, match.Index + 1);
                    unitSup = Unit.Substring(match.Index + 1, 1);
                    unitNor2 = Unit.Substring(match.Index + 2);
                }
            }

            return new ProfileItem() { paramName = Name, paramNameNor = nameNor,
                        paramNameSup = nameSup, paramValue = Value, paramUnit = Unit,
                        paramUnitNor1 = unitNor1, paramUnitNor2 = unitNor2, paramUnitSup = unitSup };
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
