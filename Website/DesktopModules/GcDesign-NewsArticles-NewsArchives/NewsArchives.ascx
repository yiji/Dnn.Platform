<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="NewsArchives.ascx.cs" Inherits="GcDesign.MultiConditionSearches.NewsArchives" %>
<asp:Repeater ID="rptNewsArchives" Runat="server" EnableViewState="False" >
	<HeaderTemplate />
	<ItemTemplate />
	<FooterTemplate />
</asp:Repeater>
<asp:DataList ID="dlNewsArchives" Runat="server" EnableViewState="False" RepeatDirection="Horizontal" ItemStyle-VerticalAlign="Top" >
	<HeaderTemplate />
	<ItemTemplate />
	<FooterTemplate />
</asp:DataList>
<div runat="server" id="divNotConfigured" class="dnnFormMessage dnnFormWarning" Visible="False" EnableViewState="False">
    <%= LocalizeString("NotConfigured") %>
</div>