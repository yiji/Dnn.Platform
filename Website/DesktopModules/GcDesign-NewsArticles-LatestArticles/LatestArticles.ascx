<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="LatestArticles.ascx.cs" Inherits="GcDesign.MultiConditionSearches.LatestArticles" %>
<%@ Register TagPrefix="GcDesign" Namespace="GcDesign.MultiConditionSearches.Components.WebControls" Assembly="GcDesign.MultiConditionSearches" %>
<asp:Repeater ID="rptLatestArticles" Runat="server" EnableViewState="False">
	<HeaderTemplate />
	<ItemTemplate />
	<FooterTemplate />
</asp:Repeater>
<asp:DataList ID="dlLatestArticles" Runat="server" EnableViewState="False" RepeatDirection="Horizontal" ItemStyle-VerticalAlign="Top" >
	<HeaderTemplate />
	<ItemTemplate />
	<FooterTemplate />
</asp:DataList>
<asp:PlaceHolder ID="phNoArticles" runat="Server" />
<asp:Label Runat="server" EnableViewState="False" ID="lblNotConfigured" ResourceKey="NotConfigured" Visible="false" CssClass="Normal" />
<GcDesign:PageControl id="ctlPagingControl" runat="server" Visible="false"></GcDesign:PageControl>