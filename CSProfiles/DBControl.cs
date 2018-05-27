using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSProfiles
{

    public class ProfilesProperties : Profiles
    {
        private String profileCharacteristic;
        public List<ProfileItem> profileCharacteristicList
        {
            get
            {
                List<ProfileItem> result = new List<ProfileItem>();
                String[] profileLine = profileCharacteristic.Split(';');
                if(profileLine.Length == 0)
                {
                    return result;
                }

                foreach(string line in profileLine)
                {
                    String[] para = line.Split('=');
                    if(para.Length != 3)
                    {
                        // log
                        continue;
                    }
                    result.Add(new ProfileItem(para[0], para[1], para[2]));
                }
                return result;
            }
        }
    }

    public class Profiles
    {
        private int id;
        private int profileNormeId;
        private int profileFamilyId;
        private String profileName;
    }

    public class ProfileItem
    {
        public String paramName { get; set; }
        public String paramValue { get; set; }
        public String paramUnit { get; set; }

        public ProfileItem(string paramName, string paramValue, string paramUnit)
        {
            this.paramName = paramName;
            this.paramValue = paramValue;
            this.paramUnit = paramUnit;
        }
    }

    public class ProfilesFamily
    {
        private int id;
        private String profileName;
        private int profileNormeId;
        private int profileImageId;
        private int profileDrawerId;
    }

    public class ProfilesImage
    {
        private int id;
        private byte[] profilesImage;
    }

    public class ProfilesNorme
    {
        public int id { get; set; }
        public String normeName { get; set; }
        public String code { get; set; }

        public ProfilesNorme(int id, String normeName, String code)
        {
            this.id = id;
            this.normeName = normeName;
            this.code = code;
        }
    }

    class DBControl
    {
    }
}
