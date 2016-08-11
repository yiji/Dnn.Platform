<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ViewCurrent.ascx.cs" Inherits="GcDesign.NewsArticles.ViewCurrent" %>
<%@ Register TagPrefix="GcDesign" TagName="Header" Src="ucHeader.ascx" %>
<%@ Register TagPrefix="GcDesign" TagName="Listing" Src="Controls/Listing.ascx"%>
<GcDesign:Header id="ucHeader1" SelectedMenu="CurrentArticles" runat="server" MenuPosition="Top" />
<GcDesign:Listing id="ucListing1" runat="server" />
<GcDesign:Header id="ucHeader2" SelectedMenu="CurrentArticles" runat="server" MenuPosition="Bottom" />