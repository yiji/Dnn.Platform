<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ViewCategory.ascx.cs" Inherits="GcDesign.NewsArticles.ViewCategory" %>
<%@ Register TagPrefix="GcDesign" TagName="Header" Src="ucHeader.ascx" %>
<%@ Register TagPrefix="GcDesign" TagName="Listing" Src="Controls/Listing.ascx"%>
<GcDesign:Header id="ucHeader1" SelectedMenu="Categories" runat="server" MenuPosition="Top" />
<asp:PlaceHolder ID="phCategory" runat="server" EnableViewState="false" />
<GcDesign:Listing id="Listing1" runat="server" />
<GcDesign:Header id="ucHeader2" SelectedMenu="Categories" runat="server" MenuPosition="Bottom" />