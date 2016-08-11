<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="NewsArticlesOptions.ascx.cs" Inherits="GcDesign.NewsArticles.NewsArticlesOptions" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<table id="tblLatestArticlesDetail" cellspacing="2" cellpadding="2" summary="Appearance Design Table"
    border="0" runat="server">
    <tr valign="top">
        <td class="SubHead" width="150">
            <dnn:label id="plModuleID" runat="server" resourcekey="Module" suffix=":" controlname="drpModuleID"></dnn:label>
        </td>
        <td align="left" width="325">
            <asp:DropDownList ID="drpModuleID" runat="server" Width="325" DataValueField="ModuleID" DataTextField="ModuleTitle"
                CssClass="NormalTextBox" AutoPostBack="True">
            </asp:DropDownList></td>
    </tr>
</table>
