<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BrowserProperties.aspx.cs" Inherits="TSoar.Developer.BrowserProperties" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Browser Properties</title>
</head>
<body>
    <div id="BrowserProps"></div>

    <script>
    var txt = "";
    txt += "<p>Browser CodeName: " + navigator.appCodeName + "</p>";
    txt += "<p>Browser Name: " + navigator.appName + "</p>";
    txt += "<p>Browser Version: " + navigator.appVersion + "</p>";
    txt += "<p>Cookies Enabled: " + navigator.cookieEnabled + "</p>";
    txt += "<p>Browser Language: " + navigator.language + "</p>";
    txt += "<p>Browser Online: " + navigator.onLine + "</p>";
    txt += "<p>Platform: " + navigator.platform + "</p>";
    txt += "<p>User-agent header: " + navigator.userAgent + "</p>";

    document.getElementById("BrowserProps").innerHTML = txt;
    </script>

</body>
</html>
