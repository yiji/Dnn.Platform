<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="NewsSearch.ascx.cs" Inherits="GcDesign.NewsArticles.NewsSearch" %>
<asp:Label Runat="server" EnableViewState="False" ID="lblNotConfigured" ResourceKey="NotConfigured"
	Visible="False" CssClass="Normal" />
<asp:PlaceHolder ID="phSearchForm" runat="server">
<div align="center" id="articleSearchFormSmall" >
    <asp:Panel ID="pnlSearch" runat="server" DefaultButton="btnSearch">
        <asp:TextBox ID="txtSearch" runat="server" CssClass="NormalTextBox" />
        <asp:Button ID="btnSearch" runat="server" Text="Search" ResourceKey="Search"  />
    </asp:Panel>
</div>
</asp:PlaceHolder>