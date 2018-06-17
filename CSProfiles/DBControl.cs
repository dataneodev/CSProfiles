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
        public int profileFamilyId { get; private set; }
        public String profileName { get; private set; }

        public Profiles(int id, int profileFamilyId, String profileName)
        {
            this.id = id;
            this.profileFamilyId = profileFamilyId;
            this.profileName = profileName;
        }
    }

    public class ProfileItem
    {
        public String paramName { get; private set; }
        public String paramNameNor { get; private set; }
        public String paramNameSup { get; private set; }
        public String paramValue { get; private set; }
        public String paramValueNor { get; private set; }
        public String paramValueSup { get; private set; }
        public String paramUnit { get; private set; }
        public String paramUnitNor1 { get; private set; }
        public String paramUnitNor2 { get; private set; }
        public String paramUnitSup { get; private set; }
        public String paramDesc { get; private set; }

        public ProfileItem(String pName, String pValue, String pUnit, String pDesc)
        {
            paramValueSup = pValue.GetItem("<b>", "</b>");
            paramValueNor = paramValueSup.Length > 0 ? pValue.CutBacking("<b>") + pValue.CutFoward("</b>") : pValue;
            paramValue = paramValueNor + paramValueSup;

            paramNameSup = pName.GetItem("<b>", "</b>");
            paramNameNor = paramNameSup.Length > 0 ? pName.CutBacking("<b>") + pName.CutFoward("</b>") : pName;
            paramName = paramNameNor + paramNameSup;

            paramUnitSup = pUnit.GetItem("<t>", "</t>");
            paramUnitNor1 = paramUnitSup.Length > 0 ? pUnit.CutBacking("<t>") : pUnit;
            paramUnitNor2 = paramUnitSup.Length > 0 ? pUnit.CutFoward("</t>") : "";
            paramUnit = paramUnitNor1 + paramUnitSup + paramUnitNor2;
            paramDesc = pDesc.RemoveTags();
        }
    }

    public static class ExtansionClass
    {
        public static string CutFoward(this string ciag, string search)
        {
            if (search.Length == 0) { return ""; }
            int pos = ciag.IndexOf(search);
            if (pos == -1) { return ""; }
            pos += search.Length;
            return ciag.Substring(pos);
        }

        public static string CutBacking(this string ciag, string search)
        {
            if (search.Length == 0) { return ""; }
            int pos = ciag.IndexOf(search);
            if (pos == -1) { return ""; }
            return ciag.Substring(0, pos);
        }

        public static string GetItem(this string ciag, string posStart, string posEnd)
        {
            if (posStart.Length == 0) { return ""; }
            if (posEnd.Length == 0) { return ""; }

            int pos = ciag.IndexOf(posStart);
            if (pos == -1) { return ""; }
            pos += posStart.Length;

            int pos2 = ciag.IndexOf(posEnd);
            if (pos2 == -1 || pos2 < pos) { return ""; }
            return ciag.Substring(pos, pos2 - pos).Trim();
        }

        public static string RemoveTags(this string ciag)
        {
            return ciag.Replace("<t>","").Replace("</t>", "").Replace("<b>", "").Replace("</b>", "");
        }
    }

    public class ProfilesFamily
    {
        public int id { get; private set; }
        public String profileName { get; private set; }
        public int profileNormeId { get; private set; }
        public int profileImageId { get; private set; }
        public int profileDrawerId { get; private set; }
        public String descryption { get; private set; }

        public ProfilesFamily(int id, String profileName, int profileNormeId, int profileImageId, 
            int profileDrawerId, String desc)
        {
            this.id = id;
            this.profileName = profileName;
            this.profileNormeId = profileNormeId;
            this.profileImageId = profileImageId;
            this.profileDrawerId = profileDrawerId;
            this.descryption = desc;
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
                    dataReader["nname"].ToString(), dataReader["code"].ToString() ));
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
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            if (DBConnection == null)
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "DBConnection == null");
                #endif
                return;
            }
            listFamily.Clear();
            if (selectedNorme == null)
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "selectedNorme == null"); 
                #endif
                return;
            }

            SQLiteCommand sqlcmd = DBConnection.CreateCommand();
            sqlcmd.CommandText = $@"SELECT * FROM TFAMILY WHERE normeid={selectedNorme.id.ToString()} OR norme2id={selectedNorme.id.ToString()}";
            try
            {
                SQLiteDataReader dataReader = sqlcmd.ExecuteReader();
                while (dataReader.Read())
                {
                    listFamily.Add(new ProfilesFamily(Convert.ToInt32(dataReader["id"]),
                    Convert.ToString(dataReader["fname"]), Convert.ToInt32(dataReader["normeid"]),
                    Convert.ToInt32(dataReader["imageid"]), Convert.ToInt32(dataReader["drawerid"]),
                    Convert.ToString(dataReader["descryption"])));
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
            if (DBConnection == null)
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "DBConnection == null");
                #endif
                return;
            }

            profilesList.Clear();
            if (selectedFamily == null)
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "selectedFamily == null"); 
                #endif
                return;
            }

            SQLiteCommand sqlcmd = DBConnection.CreateCommand();
            sqlcmd.CommandText = "SELECT * FROM TPROFILES WHERE familyid=" + selectedFamily.id.ToString();
            try
            {
                SQLiteDataReader dataReader = sqlcmd.ExecuteReader();
                while (dataReader.Read())
                {
                    profilesList.Add(new Profiles(Convert.ToInt32(dataReader["id"]), Convert.ToInt32(dataReader["familyid"]),
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
            if (DBConnection == null)
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "DBConnection == null");
                #endif
                return;
            }
            
            listViewData.Clear();
            if (selectedProfile == null)
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "selectedProfile == null"); 
                #endif
                return;
            }

            SQLiteCommand sqlcmd = DBConnection.CreateCommand();
            sqlcmd.CommandText = "SELECT  v.valuef, v.valuet, n.pname, n.punit, n.descryption FROM TVALUES v INNER"
                                 + $" JOIN TNAMES n ON v.nameid = n.id WHERE v.profileid={selectedProfile.id.ToString()};";
            try
            {
                SQLiteDataReader dataReader = sqlcmd.ExecuteReader();
                while (dataReader.Read())
                {
                    String value = "";
                    int idcol = dataReader.GetOrdinal("valuef");
                    if (!dataReader.IsDBNull(idcol))
                    {
                        value = dataReader.GetString(idcol);
                    }

                    idcol = dataReader.GetOrdinal("valuet");
                    if (!dataReader.IsDBNull(idcol))
                    {
                        value = dataReader.GetString(idcol);
                    }

                    listViewData.Add(new ProfileItem(dataReader["pname"].ToString(), value, dataReader["punit"].ToString(), 
                                    dataReader["descryption"].ToString()));
                }
            }
            catch (SQLiteException e)
            {
                #if DEBUG
                    Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                        $"SQL Error: {e.Message.ToString()} \nSQL: {sqlcmd.CommandText}");
                #endif
                listViewData.Clear();
            }
        }

        public void GetprofilesImage(System.Windows.Controls.Image image, ProfilesFamily selectedFamily)
        {
            if (DBConnection == null)
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "DBConnection == null");
                #endif
                return;
            }
            image.Source = null;

            if (selectedFamily == null)
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "selectedProfile == null"); 
                #endif
                return;
            }

            if (selectedFamily.profileImageId == 0)
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "selectedFamily.profileImageId == 0"); 
                #endif
                return;
            }

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
 
        public void CloseConnection()
        {
            DBConnection.Close();
            DBConnection.Dispose();
        }
    }
}
