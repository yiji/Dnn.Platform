<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="PostRating.ascx.cs" Inherits="GcDesign.NewsArticles.Controls.PostRating" %>
<asp:RadioButtonList ID="lstRating" runat="server" CssClass="Normal" RepeatDirection="Horizontal" AutoPostBack="true" RepeatLayout="Flow" CausesValidation="false" >
    <asp:ListItem Value="1">1</asp:ListItem>
    <asp:ListItem Value="2">2</asp:ListItem>
    <asp:ListItem Value="3">3</asp:ListItem>
    <asp:ListItem Value="4">4</asp:ListItem>
    <asp:ListItem Value="5">5</asp:ListItem>
</asp:RadioButtonList>
<asp:Label ID="lblRatingSaved" runat="server" Text="Rating Saved!" CssClass="Normal" EnableViewState="False" Visible="false" />
