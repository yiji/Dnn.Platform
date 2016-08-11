<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="LatestComments.ascx.cs" Inherits="GcDesign.MultiConditionSearches.LatestComments" %>
<asp:Repeater ID="rptLatestComments" Runat="server" EnableViewState="False" >
	<HeaderTemplate />
	<ItemTemplate />
	<FooterTemplate />
</asp:Repeater>
<asp:PlaceHolder ID="phNoComments" runat="Server" />
<asp:Label Runat="server" EnableViewState="False" ID="lblNotConfigured" ResourceKey="NotConfigured" Visible="False" CssClass="Normal" />
