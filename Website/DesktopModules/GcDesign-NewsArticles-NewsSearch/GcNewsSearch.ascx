<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="GcNewsSearch.ascx.cs" Inherits="GcDesign.MultiConditionSearches.GcNewsSearch" %>
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