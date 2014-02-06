<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CitybreakSsoDemo._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Portalen</h1>
        <p class="lead">Välkommen till portalen. Denna sida är inte skyddad.</p>
        <p><a href="/Protected.aspx" class="btn btn-primary btn-large">Den här sidan är däremot skyddad med inloggning.</a></p>
    </div>

</asp:Content>
