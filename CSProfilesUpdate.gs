// @2018 dataneo 
// set script properties
// PageUrl : new version page
// Version : version eg 1.0
function doGet(e) {
  var scriptProperties = PropertiesService.getScriptProperties();
  var version= scriptProperties.getProperty("Version");
  if(version == null){
    Logger.log("Brak version.");
  }
  
  var pageurl= scriptProperties.getProperty("PageUrl");
  if(pageurl == null){
    Logger.log("Brak PageUrl.");
  }
  
  var ResponseBody = "<?xml version=\"1.0\"?>\n";
  ResponseBody += "<VersionInfo>\n";
  ResponseBody += " <Version>"+ version +"</Version>\n";
  ResponseBody += " <UpdatePage>"+ pageurl +"</UpdatePage>\n";
  ResponseBody += "</VersionInfo>";
  
  return ResT(ResponseBody);
}

function ResT(MSG){
  return ContentService.createTextOutput(MSG);
}