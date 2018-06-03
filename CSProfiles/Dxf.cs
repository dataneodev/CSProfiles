using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CSProfiles
{
    // base class with drawing functons
    public class Dxf
    {
        private bool openDxf;
        private bool polyLineStarted;

        private int scaleFac = 1;
        private LineType selectedLine = LineType.lineContinue;
        private List<String> dxfBody = new List<String>();

        private const String LayerContinue = "CJCONTINUE";
        private const String LayerHidden = "CJHIDDEN";
        public enum LineType { lineContinue, lineHidden };

        public Dxf()
        {
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            openDxf = true;
            dxfBody.Clear();
            String[] header = new String[] {
                "0", "SECTION", "2", "HEADER", "999", "dxf created by CJProfiles", "0", "ENDSEC",
				// TABLES
				"0", "SECTION","2","TABLES","0","TABLE","2","LTYPE","70","1",
                    "0","LTYPE","2","CJCONTINUOUS","70","64","3","CJSolid line","72","65","73","0","40","0.000000",
                    "0","LTYPE","2","CJHIDDEN","70","64","3","CJHidden line","72","65","73","2","40","30.0","49","12","49","-18","0","ENDTAB",
                "0", "TABLE","2","LAYER","70","6",
                    "0","LAYER","2",LayerContinue,"70","64","62","3","6","CJCONTINUOUS",
                    "0","LAYER","2",LayerHidden,"70","64","62","50","6","CJHIDDEN","0","ENDTAB",
                "0","TABLE","2","STYLE","70","0","0","ENDTAB",
                "0","TABLE","2","VPORT","70","1","0","VPORT","2","CSVIEV","70","0","12","1000","22","150","40","2000","41","2","0","ENDTAB",
                "0","ENDSEC",
				// end TABLES
				"0", "SECTION", "2", "BLOCKS", "0", "ENDSEC",
                "0", "SECTION", "2", "ENTITIES"
              };
            foreach (String line in header)
            {
                dxfBody.Add(line);
            }
        }

        public void endDxf()
        {
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            if (openDxf == false)
            {
                #if DEBUG
                Log.Warning(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    " openDxf == false");
                #endif
                return;
            }
            if (polyLineStarted == true)
            {
                polyLineStarted = false;
                polyLineEnd();
            }
            // ends of ENTITIES
            dxfBody.Add("0");
            dxfBody.Add("ENDSEC");
            // ends of file
            dxfBody.Add("0");
            dxfBody.Add("EOF");
            openDxf = false;
        }

        public void polyLineStart(LineType lineTyp)
        {
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            if (polyLineStarted)
            {
                #if DEBUG
                Log.Warning(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "polyLine is already open");
                #endif
                return;
            }
            selectedLine = lineTyp;
            dxfBody.Add("0");
            dxfBody.Add("POLYLINE");
            dxfBody.Add("8");
            if (selectedLine == LineType.lineContinue)
            {
                dxfBody.Add(LayerContinue);
            }
            else
            {
                dxfBody.Add(LayerHidden);
            }
            dxfBody.Add("66");
            dxfBody.Add("1");
            dxfBody.Add("40");
            dxfBody.Add("0");
            dxfBody.Add("41");
            dxfBody.Add("0");
            polyLineStarted = true;
        }

        public void polyLineEnd()
        {
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            if (!polyLineStarted)
            {
                #if DEBUG
                    Log.Warning(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "polyLine not started");
                #endif
                return;
            }
            dxfBody.Add("0");
            dxfBody.Add("SEQEND");
            polyLineStarted = false;
        }

        public void polyLineAdd(float x, float y, double Bulge)
        {
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "x=" + x.ToString() + " y=" + y.ToString() + " Bulge=" + Bulge.ToString() );
            #endif
            if (!polyLineStarted)
            {
                polyLineStart(LineType.lineContinue);
            }
            dxfBody.Add("0");
            dxfBody.Add("VERTEX");
            dxfBody.Add("8");
            if (selectedLine == LineType.lineContinue)
            {
                dxfBody.Add(LayerContinue);
            }
            else
            {
                dxfBody.Add(LayerHidden);
            }
            dxfBody.Add("10");
            dxfBody.Add((scaleFac*x).ToString());
            dxfBody.Add("20");
            dxfBody.Add((scaleFac*y).ToString());
            dxfBody.Add("30");
            dxfBody.Add("0.0");
            if (Bulge != 0)
            {
                dxfBody.Add("42");
                dxfBody.Add(Bulge.ToString().Replace(",","."));
            }
        }

        public String getDxfBody()
        {
            #if DEBUG
                Log.Notice(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif
            if (openDxf == true)
            {
                endDxf();
            }
            return String.Join("\n", dxfBody);
        }

    }
}
