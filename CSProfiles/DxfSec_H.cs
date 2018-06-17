using System;
using System.Collections.Generic;

namespace CSProfiles
{
    public class Sections_H : IDxfDrawer
    {
	    private List<ProfileItem> profileData;
        private int drawerId;

        public void SetData(List<ProfileItem> profileItem, int darwerId)
        {
            this.profileData = new List<ProfileItem>(profileItem);
            this.drawerId = darwerId;
        }

        public String GetDxfBody(ViewOptions viewOptions)
        {
            IHSectionCordinate Cor = null;
            // cordinate
		    switch(drawerId) {
			    case 1: // Eurocode
				    Cor = new HSectionCordinateEC();
                break;
            }
		
		    if(Cor == null) {
                #if DEBUG
                    Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "cordinate == null");
                #endif
                return "";
            }

            try
            {
                Cor.SetProfilesProperties(profileData); 
            }
            catch(ArgumentNullException e)
            {
                #if DEBUG
                Log.Error(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "SetProfilesProperties: " + e.Message.ToString());
                #endif
                return "";
            }

            Dxf dxfFile = new Dxf();
            float B = Cor.B;
            float H = Cor.H;
            float tf = Cor.Tf;
            float tw = Cor.Tw;
            float r = Cor.R;

		     // section generate
		    //top view
		    if(viewOptions.frontView) {
			    double buckleA = -1 * Math.Tan((90 / 4)* (Math.PI / 180));
                // left side
                dxfFile.polyLineStart(Dxf.LineType.lineContinue);
			    dxfFile.polyLineAdd(-1* B/2, 0, 0);
			    dxfFile.polyLineAdd(B/2, 0, 0);
			    dxfFile.polyLineAdd(B/2, tf, 0);
			    dxfFile.polyLineAdd(tw/2+r, tf, buckleA);
			    dxfFile.polyLineAdd(tw/2, tf+r, 0);
			    dxfFile.polyLineAdd(tw/2, tf+r+Cor.H1, buckleA);
			    dxfFile.polyLineAdd(tw/2+r, H-tf, 0);
			    dxfFile.polyLineAdd(B/2, H-tf, 0);
			    dxfFile.polyLineAdd(B/2, H, 0);

			    // right side
			    dxfFile.polyLineAdd(-1* B/2, H, 0);
			    dxfFile.polyLineAdd(-1* B/2, H-tf, 0);
			    dxfFile.polyLineAdd(-1* (tw/2+r), H-tf, buckleA);
			    dxfFile.polyLineAdd(-1* tw/2, tf+r+Cor.H1, 0);
			    dxfFile.polyLineAdd(-1* tw/2, tf+r, buckleA);
			    dxfFile.polyLineAdd(-1* (tw/2+r), tf, 0);
			    dxfFile.polyLineAdd(-1* B/2, tf, 0);
			    dxfFile.polyLineAdd(-1* B/2, 0, 0);
			    dxfFile.polyLineEnd();
		    }
		
		    if(viewOptions.topView) {
		        float startX = 0;
                float startY = H + B;
                dxfFile.polyLineStart(Dxf.LineType.lineContinue);
			    dxfFile.polyLineAdd(-1* B/2+startX, 0 + startY, 0);
			    dxfFile.polyLineAdd(B/2+startX, 0 + startY, 0);
			    dxfFile.polyLineAdd(B/2+startX, 2* B + startY, 0);
			    dxfFile.polyLineAdd(-1* B/2+startX, 2* B + startY, 0);
			    dxfFile.polyLineAdd(-1* B/2+startX, 0 + startY, 0);
			    dxfFile.polyLineEnd();
			    // web
			    dxfFile.polyLineStart(Dxf.LineType.lineHidden);
			    dxfFile.polyLineAdd(-1* tw/2+startX, 0 + startY, 0);
			    dxfFile.polyLineAdd(-1* tw/2+startX, 2* B + startY, 0);
			    dxfFile.polyLineEnd();
			    dxfFile.polyLineStart(Dxf.LineType.lineHidden);
			    dxfFile.polyLineAdd(tw/2+startX, 0 + startY, 0);
			    dxfFile.polyLineAdd(tw/2+startX, 2* B + startY, 0);
			    dxfFile.polyLineEnd();			
		    }
		
		    if(viewOptions.sideView) {
			    float startX = B + 0.5F * B;
                float startY = 0;
                dxfFile.polyLineStart(Dxf.LineType.lineContinue);
			    dxfFile.polyLineAdd(0 + startX, 0 + startY, 0);
			    dxfFile.polyLineAdd(2* B + startX, 0 + startY, 0);
			    dxfFile.polyLineAdd(2* B + startX, H + startY, 0);
			    dxfFile.polyLineAdd(0 + startX, H + startY, 0);
			    dxfFile.polyLineAdd(0 + startX, 0 + startY, 0);
			    dxfFile.polyLineEnd();
			
			    dxfFile.polyLineStart(Dxf.LineType.lineContinue);
			    dxfFile.polyLineAdd(0 + startX, tf + startY, 0);
			    dxfFile.polyLineAdd(2* B + startX, tf + startY, 0);
			    dxfFile.polyLineEnd();
			
			    dxfFile.polyLineStart(Dxf.LineType.lineContinue);
			    dxfFile.polyLineAdd(0 + startX, H - tf + startY, 0);
			    dxfFile.polyLineAdd(2* B + startX, H - tf + startY, 0);
			    dxfFile.polyLineEnd();
		    }
		    dxfFile.endDxf();
		    return dxfFile.getDxfBody();
	    }
    }

    // Eurocode H Section cordinate
    public class HSectionCordinateEC : HSectionCordinateSource, IHSectionCordinate
    {
        public HSectionCordinateEC(){
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            ParametrsMap = new List<HSectionMap>
            {
                new HSectionMap(HSectionDim.h, "h", "mm"),
                new HSectionMap(HSectionDim.b, "b", "mm"),
                new HSectionMap(HSectionDim.tf, "tg", "mm"),
                new HSectionMap(HSectionDim.tw, "ts", "mm"),
                new HSectionMap(HSectionDim.r, "r", "mm")
            };
        }
    }

    // base
    interface IHSectionCordinate
    {
        void SetProfilesProperties(List<ProfileItem> profile);
        float H { get; }
        float B { get; }
        float Tf { get; }
        float Tw { get;}
        float R { get; }
        float H1 { get; }
    }

    public enum HSectionDim { h, b, tf, tw, r }

    public class HSectionMap
    {
        public HSectionDim dim;
        public String dimName;
        public String dimUnit;
        public float dimValue;
        public bool dimExists;

        public HSectionMap(HSectionDim dim, String dimName, String dimUnit)
        {
            this.dim = dim;
            this.dimName = dimName;
            this.dimUnit = dimUnit;
            this.dimExists = false;
        }
        public void setExists()
        {
            this.dimExists = true;
        }
    }

    public abstract class HSectionCordinateSource : IHSectionCordinate
    {
	    protected List<HSectionMap> ParametrsMap;
  
        public float H { get { return getMapValue(HSectionDim.h); } }
        public float B { get { return getMapValue(HSectionDim.b); } }
        public float Tf { get { return getMapValue(HSectionDim.tf); } }
        public float Tw { get { return getMapValue(HSectionDim.tw); } }
        public float R { get { return getMapValue(HSectionDim.r); } }
        public float H1 { get { return H - 2 * Tf - 2 * R; } }

        public void SetProfilesProperties(List<ProfileItem> profile)
        {
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            ProcesProfileData(profile);
        }

        private void ProcesProfileData(List<ProfileItem> profileData)
        {
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif

            if (ParametrsMap == null) {
              throw new ArgumentNullException("ParametrsMap == null");
            }
		
		    if(ParametrsMap.Count == 0) {
                throw new ArgumentNullException("ParametrsMap.size() == 0");
            }

            if (profileData == null)
            {
                 throw new ArgumentNullException("profileData == null");
            }
           
            if(profileData.Count == 0) {
                throw new ArgumentNullException("profileData.Count == 0");
            }

            for(int a=0; a < ParametrsMap.Count; a++) {
                for (int b = 0; b < profileData.Count; b++)
                {
                    if (
                        (ParametrsMap[a].dimName.Equals(profileData[b].paramName)) &&
                        (ParametrsMap[a].dimUnit.Equals(profileData[b].paramUnit))
                       )
                    {
                        ParametrsMap[a].setExists();
                        try
                        {
                            ParametrsMap[a].dimValue = Convert.ToSingle(profileData[b].paramValue.Replace(".",","));
                        }
                        catch (FormatException)
                        { 
                            throw new ArgumentNullException("procesProfileData: getMapValue: " + 
                                        profileData[b].paramValue + " is not a float");
                        }
                    break;
                    }
                } 
            }

            for (int a=0; a < ParametrsMap.Count; a++) {
            if (ParametrsMap[a].dimExists == false)
                {
                throw new ArgumentNullException("map check failed");
                }
            }
        }

        private float getMapValue(HSectionDim dim)
        {
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            for (int a = 0; a < ParametrsMap.Count; a++)
                if (dim == ParametrsMap[a].dim)
                {
                    return ParametrsMap[a].dimValue;
                }
            return 0;
        }
    }

}
