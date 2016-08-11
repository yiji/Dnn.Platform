<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="GcNewsSearchOptions.ascx.cs" Inherits="GcDesign.NewsArticles.GcNewsSearchOptions" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="0" cellpadding="2" summary="Edit Links Design Table" border="0">
	<tr valign="top">
		<td class="SubHead" width="150"><asp:label id="plModuleID" runat="server" Text="关联搜索模块" resourcekey="Module" suffix=":" controlname="drpModuleID" /></td>
		<td align="left" width="325">
			<asp:dropdownlist id="drpModuleID" Runat="server" Width="325" datavaluefield="ModuleID" datatextfield="ModuleTitle" 
				CssClass="NormalTextBox" />
        </td>
	</tr>
    <tr valign="top">
		<td class="SubHead" width="150"><asp:label id="Label1" runat="server" resourcekey="Search" suffix=":" Text="搜索界面" /></td>
		<td align="left" width="325">
            <asp:dropdownlist id="drpSearchWindow" Runat="server" Width="325" CssClass="NormalTextBox" />
        </td>
	</tr>
</table>