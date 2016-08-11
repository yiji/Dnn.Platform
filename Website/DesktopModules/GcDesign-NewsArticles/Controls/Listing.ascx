<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Listing.ascx.cs" Inherits="GcDesign.NewsArticles.Controls.Listing" %>
<asp:Repeater ID="rptListing" runat="server" >
    <HeaderTemplate></HeaderTemplate>
    <ItemTemplate></ItemTemplate>
    <FooterTemplate></FooterTemplate>
</asp:Repeater>
<asp:PlaceHolder ID="phNoArticles" runat="server" EnableViewState="False"></asp:PlaceHolder>


