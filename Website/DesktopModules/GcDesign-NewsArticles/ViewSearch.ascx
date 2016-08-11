<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ViewSearch.ascx.cs" Inherits="GcDesign.NewsArticles.ViewSearch" %>
<%@ Register TagPrefix="GcDesign" TagName="Header" Src="ucHeader.ascx" %>
<%@ Register TagPrefix="GcDesign" TagName="Listing" Src="Controls/Listing.ascx"%>
<GcDesign:Header id="ucHeader1" SelectedMenu="Search" runat="server" MenuPosition="Top" />
<div align="left" id="articleSearchForm" >
    <h1><asp:Label ID="lblSearch" Runat="server" /></h1>
    <asp:Panel ID="pnlSearch" runat="server" DefaultButton="btnSearch">
        <asp:TextBox ID="txtSearch" runat="server" CssClass="NormalTextBox" />
        <asp:Button ID="btnSearch" runat="server" Text="Search" ResourceKey="Search"   />
    </asp:Panel>
</div>
<GcDesign:Listing id="Listing1" runat="server" />
<GcDesign:Header id="ucHeader2" SelectedMenu="Search" runat="server" MenuPosition="Bottom" />