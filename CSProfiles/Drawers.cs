using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        void SetData(ObservableCollection<ProfileItem> profileItem, int darwerId);
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
        /// Open dxf file by default program associated by the file
        /// </summary>
        public void OpenDxf(String tempPath, ProfilesFamily selFamily, 
            ObservableCollection<ProfileItem> profileItem, ViewOptions viewOption)
        {

        }

        /// <summary>
        /// Save dxf file by to provided path
        /// </summary>
        public void SaveDxf(String filePath, ProfilesFamily selFamily, 
            ObservableCollection<ProfileItem> profileItem, ViewOptions viewOption)
        {

        }

        private String genDxfBody(int drawerId, ViewOptions viewOptions, 
            ObservableCollection<ProfileItem> profileItem)
        {
            return "";
        }


    }
}
