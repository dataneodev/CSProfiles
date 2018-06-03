using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSProfiles
{
    public class Sections_H : IDxfDrawer
    {
	    private ObservableCollection<ProfileItem> profileData;
        private int drawerId;

        public void SetData(ObservableCollection<ProfileItem> profileItem, int darwerId)
        {
            this.profileData = profileItem;
            this.drawerId = darwerId;
        }

        public String GetDxfBody(ViewOptions viewOptions)
        {
     
            IHSectionCordinate Cor = null;
		    switch(drawerId) {
			    case 1:
				    Cor = new HSectionCordinateEC();
                break;
            }
		
		    if(Cor == null) {
                #if DEBUG
                    Log.Warning(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "cordinate == null");
                #endif
                return "";
                //throw new IllegalArgumentException("getDxfBody: cordinate == null");
            }

            Cor.setProfilesProperties(profileData); // IllegalArgumentException

            Dxf dxfFile = new Dxf();
            float B = Cor.getB();
            float H = Cor.getH();
            float tf = Cor.getTf();
            float tw = Cor.getTw();
            float r = Cor.getR();

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
			    dxfFile.polyLineAdd(tw/2, tf+r+Cor.getH1(), buckleA);
			    dxfFile.polyLineAdd(tw/2+r, H-tf, 0);
			    dxfFile.polyLineAdd(B/2, H-tf, 0);
			    dxfFile.polyLineAdd(B/2, H, 0);

			    // right side
			    dxfFile.polyLineAdd(-1* B/2, H, 0);
			    dxfFile.polyLineAdd(-1* B/2, H-tf, 0);
			    dxfFile.polyLineAdd(-1* (tw/2+r), H-tf, buckleA);
			    dxfFile.polyLineAdd(-1* tw/2, tf+r+Cor.getH1(), 0);
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

    public class HSectionCordinateEC : HSectionCordinateSource, IHSectionCordinate
    {
        public HSectionCordinateEC(){
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            //  logger.fine("HSectionCordinateEC. Map definition");
            ParametrsMap = new List<HSectionMap>();
            ParametrsMap.Add(new HSectionMap(HSectionDim.h, "h", "mm"));
            ParametrsMap.Add(new HSectionMap(HSectionDim.b, "b", "mm"));
            ParametrsMap.Add(new HSectionMap(HSectionDim.tf, "tg", "mm"));
            ParametrsMap.Add(new HSectionMap(HSectionDim.tw, "ts", "mm"));
            ParametrsMap.Add(new HSectionMap(HSectionDim.r, "r", "mm"));
        }
    }

    interface IHSectionCordinate
    {
        void setProfilesProperties(ObservableCollection<ProfileItem> profile);
        float getH();
        float getB();
        float getTf();
        float getTw();
        float getR();
        float getH1();
    }

    public enum HSectionDim { h, b, tf, tw, r }

    public class HSectionMap
    {
        public HSectionDim dim;
        public String dimNameFamily;
        public String dimUnitFamily;
        public bool dimExists;
        public float dimValue;

        public HSectionMap(HSectionDim dim, String dimNameFamily, String dimUnitFamily)
        {
            this.dim = dim;
            this.dimNameFamily = dimNameFamily;
            this.dimUnitFamily = dimUnitFamily;
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

        public void setProfilesProperties(ObservableCollection<ProfileItem> profile)
        {
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            procesProfileData(profile);
        }

        private void procesProfileData(ObservableCollection<ProfileItem> profileData)
        {
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif

            if (ParametrsMap == null) {
            //  throw new IllegalArgumentException("ParametrsMap == null");
            }
		
		    if(ParametrsMap.Count == 0) {
            //throw new IllegalArgumentException("ParametrsMap.size() == 0");
            }

            if (profileData == null)
            {
                // throw new IllegalArgumentException("profileData == null");
            }

            // List<ProfileItem> profileParams = profileData.getProfileCharacteristicList();
            /*
                if(profileParams.Count == 0) {
                //throw new IllegalArgumentException("profileParams.size() == 0");
                }

                for(int a=0; a < ParametrsMap.Count; a++) {
                    for (int b = 0; b < profileParams.Count; b++)
                    {
                        if (
                            (ParametrsMap[a].dimNameFamily.equals(profileParams[b].getParamName())) &&
                            (ParametrsMap[a].dimUnitFamily.equals(profileParams[b].getParamUnit()))
                            )
                        {
                            ParametrsMap[a].setExists();
                             try
                             {
                                ParametrsMap[a].dimValue = Float.parseFloat(profileParams[b].getParamValue());
                             }
                            catch (NumberFormatException e)
                            {
                                //throw new IllegalArgumentException("procesProfileData: getMapValue: " + profileParams.get(b).getParamValue() + " is not a float");
                            }
                            break;
                        }
                    } 
                }*/

            for (int a=0; a < ParametrsMap.Count; a++) {
            if (ParametrsMap[a].dimExists == false)
            {
                //throw new IllegalArgumentException("procesProfileData: map check failed");
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

        public float getH()
        {
            return getMapValue(HSectionDim.h);
        }
        public float getB()
        {
            return getMapValue(HSectionDim.b);
        }
        public float getTf()
        {
            return getMapValue(HSectionDim.tf);
        }
        public float getTw()
        {      
            return getMapValue(HSectionDim.tw);
        }
        public float getR()
        {
            return getMapValue(HSectionDim.r);
        }
        public float getH1()
        {
            return (getH() - 2 * getTf() - 2 * getR());
        }
    }

}
