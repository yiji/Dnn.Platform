<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ViewTag.ascx.cs" Inherits="GcDesign.NewsArticles.ViewTag" %>
<%@ Register TagPrefix="GcDesign" TagName="Header" Src="ucHeader.ascx" %>
<%@ Register TagPrefix="GcDesign" TagName="Listing" Src="Controls/Listing.ascx"%>
<GcDesign:Header id="ucHeader1" SelectedMenu="Categories" runat="server" MenuPosition="Top" />
<div align="left">
    <h1><asp:Label ID="lblTag" Runat="server" /></h1>
</div>
<GcDesign:Listing id="Listing1" runat="server" />
<GcDesign:Header id="ucHeader2" SelectedMenu="Categories" runat="server" MenuPosition="Bottom" />