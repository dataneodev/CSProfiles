using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace CSProfiles
{
    public static class Updater
    {
        const String pageUrl = "https://sites.google.com/site/dataneosoftware/csprofiles";
        const String updateServerUrl = "https://script.google.com/macros/s/AKfycbwxX5HtjmwrCveYj8u9t2Hk2yA-sv4c_HeGnYYlXT4mVOLmTnk/exec";
        private static float versionCurrent;
        public static float versionServer { get; private set; }
        public static String versionUpdateUrl { get; private set; }

        public static void OpenHomePage()
        {
            #if DEBUG
                Log.Notice(System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "OpenHomePage: " + pageUrl);
            #endif
            try
            {
                if (IsNewVersion())
                {
                    System.Diagnostics.Process.Start(versionUpdateUrl);
                }
                else
                {
                    System.Diagnostics.Process.Start(pageUrl);
                } 
            }
            catch(System.ComponentModel.Win32Exception noBrowser)
            {
                #if DEBUG
                    Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "SQL Error: " + noBrowser.Message.ToString());
                #endif
            }
            catch (System.Exception other)
            {
                #if DEBUG
                    Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "SQL Error: " + other.Message.ToString());
                #endif
            }
        }

        public static async void CheckUpdate(float version, TextBlock label)
        {
            #if DEBUG
                Log.Notice(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "CheckUpdate(): " + updateServerUrl);
            #endif
            versionCurrent = version;

            WebRequest request = null;
            try {
                request = WebRequest.Create(updateServerUrl);
            }
            catch(System.Exception e)
            {
                #if DEBUG
                    Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "WebRequest Exception: " + e.Message.ToString());
                #endif
                return;
            }

            WebResponse response = null;
            String responseString;
            try
            {
                Task<WebResponse> webResponseTask = request.GetResponseAsync();

                await Task.WhenAll(webResponseTask);

                if (webResponseTask.IsFaulted)
                {
                    #if DEBUG
                    Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "webResponseTask.IsFaulted");
                    #endif
                    return;
                }

                response = webResponseTask.Result;
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    responseString = sr.ReadToEnd();
                }

            }
            catch (System.Exception e)
            {
                #if DEBUG
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "WebResponse Exception: " + e.Message.ToString());
                #endif
                return;
            }

            if(responseString.Length == 0)
            {
                #if DEBUG
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "data.Length == 0");
                #endif
                return;
            }

            XmlDocument doc = new XmlDocument();
            XmlNodeList nodes;
            try
            {
                doc.LoadXml(responseString);
                nodes = doc.DocumentElement.SelectNodes("/VersionInfo");
            }
            catch (System.Exception e)
            {
                #if DEBUG
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "XML load Exception: " + e.Message.ToString());
                #endif
                return;
            }

            if(nodes.Count == 0)
            {
                #if DEBUG
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "nodes.Count == 0");
                #endif
                return;
            }

            try
            {
                versionUpdateUrl = nodes[0].SelectSingleNode("UpdatePage").InnerText;
            }
            catch (System.Xml.XPath.XPathException e)
            {
                #if DEBUG
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "System.Xml.XPath.XPathException: " + e.Message.ToString());
                #endif
                versionUpdateUrl = "";
                return;
            }

            try
            {
                float outVersion;
                if(!float.TryParse(nodes[0].SelectSingleNode("Version").InnerText.Replace(".",","), out outVersion))
                {
                    #if DEBUG
                    Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name,
                        "!float.TryParse(nodes[0].SelectSingleNode(\"Version\")");
                    #endif
                    versionServer = 0;
                    return;
                }
                versionServer = outVersion;
            }
            catch (System.Xml.XPath.XPathException e)
            {
                #if DEBUG
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name,
                    "System.Xml.XPath.XPathException: " + e.Message.ToString());
                #endif
                versionServer = 0;
                return;
            }

            if (IsNewVersion())
            {
                label.Text = "New version avilable !";
                label.FontWeight = FontWeights.Bold;
                label.Foreground =System.Windows.Media.Brushes.Red;
            }
        }

        public static bool IsNewVersion()
        {
            if((versionServer > versionCurrent) && (versionUpdateUrl.Length > 0))
            {
                return true;
            }
            return false;
        }
    }
}
