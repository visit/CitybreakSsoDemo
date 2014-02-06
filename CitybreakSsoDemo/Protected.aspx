<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Protected.aspx.cs" Inherits="CitybreakSsoDemo.Protected" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label runat="server" ID="Message">Not Logged in</asp:Label>
        <asp:Label runat="server" Visible="False" ID="UserDataLabel">User data from Citybreak:</asp:Label>
        <dl runat="server" id="tokenPayload"></dl>
        <asp:Button runat="server" ID="LogoutButton" Text="Log out" OnClick="LogoutClick" Visible="false"/>
    </div>
    </form>
</body>
</html>
