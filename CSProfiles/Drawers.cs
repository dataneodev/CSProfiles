using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSProfiles
{
    public struct ViewOptions{
        public bool topView;
        public bool frontView;
        public bool sideView;
    }

    interface IDxfDrawer
    {
        void SetData(List<ProfileItem> profileItem, int darwerId);
        String GetDxfBody(ViewOptions viewOptions);
    }

    sealed class Drawers
    {
        /// <summary>
        /// Check is family has dxf drawer
        /// </summary>
        public bool IsFamilyHasDxfDrawer(ProfilesFamily checkedFamily)
        {  
            if(checkedFamily == null)
            {
                #if DEBUG
                Log.Warning(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                   "checkedFamily == null" );
                #endif
                return false;
            }
            bool result = false;
            if (checkedFamily.profileDrawerId > 0)
            {
                result = true;
            }
            #if DEBUG
            Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                "profileDrawerId="+ checkedFamily.profileDrawerId.ToString()+" is "+result.ToString());
            #endif
            return result;            
        }

        /// <summary>
        /// Check is family has dxf drawer
        /// </summary>
        public bool IsFamilyHas3DView(ProfilesFamily checkedFamily)
        {
            return false;
        }

        /// <summary>
        /// Save dxf file by to provided path;
        /// </summary>
        public bool SaveDxfToFile(String filePath, ProfilesFamily selFamily, 
                                  List<ProfileItem> profileItem, ViewOptions viewOption)
        {
            if (!IsFamilyHasDxfDrawer(selFamily))
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "!IsFamilyHasDxfDrawer(selFamily)");
                #endif
                return false;
            }
            if((profileItem == null) || (profileItem.Count == 0))
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "((profileItem == null) || (profileItem.Count == 0))");
                #endif
                return false;
            }

            if(filePath.Length < 2)
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "filePath.Length < 2");
                #endif
                return false;
            }

            String dxfBody = genDxfBody(selFamily.profileDrawerId, viewOption, profileItem);
            if(dxfBody.Length == 0)
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "dxfBody.Length == 0");
                #endif
                return false;
            }

            try
            {
                StreamWriter outputFile = new StreamWriter(filePath);
                outputFile.WriteLine(dxfBody);
                outputFile.Close();
            }
            catch (IOException e)
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "IOException : " + e.Message.ToString());
                #endif
                return false;
            }
            return true;
        }

        private String genDxfBody(int drawerId, ViewOptions viewOptions, List<ProfileItem> profileItem)
        {
            IDxfDrawer dxfBody;

            switch (drawerId)
            {
                case 1:
                    dxfBody = new Sections_H();
                    break;
                default:
                    dxfBody = null;
                    break;
            }

            if (dxfBody == null)
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "(dxfBody == null)" );
                #endif
                return "";
            }

            dxfBody.SetData(profileItem, drawerId);
            return dxfBody.GetDxfBody(viewOptions);
        }
    }
}
