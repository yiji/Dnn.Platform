<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="NewsArticles.ascx.cs" Inherits="GcDesign.NewsArticles.NewsArticles" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnJsInclude ID="DnnJsInclude1" runat="server" FilePath="~/desktopmodules/GcDesign-NewsArticles/includes/shadowbox/shadowbox.js" />
<dnn:DnnCssInclude ID="DnnCssInclude1" runat="server" FilePath="~/desktopmodules/GcDesign-NewsArticles/includes/shadowbox/shadowbox.css" />
<dnn:DnnJsInclude ID="DnnJsInclude2" runat="server" FilePath="~/desktopmodules/GcDesign-NewsArticles/includes/ad-gallery/jquery.ad-gallery.js" />
<dnn:DnnCssInclude ID="DnnCssInclude2" runat="server" FilePath="~/desktopmodules/GcDesign-NewsArticles/includes/ad-gallery/jquery.ad-gallery.css" />

<div class="NewsArticles">
    <asp:PlaceHolder id="plhControls" runat="Server" />
</div>