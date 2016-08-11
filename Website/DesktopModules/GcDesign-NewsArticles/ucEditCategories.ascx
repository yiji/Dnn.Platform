<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ucEditCategories.ascx.cs" Inherits="GcDesign.NewsArticles.ucEditCategories" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="HelpButton" Src="~/controls/HelpButtonControl.ascx" %>
<%@ Register TagPrefix="article" TagName="Header" Src="ucHeader.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web.Deprecated" Namespace="DotNetNuke.Web.UI.WebControls" %>


<%@ Register Src="~/controls/TextEditor.ascx" TagName="texteditor" TagPrefix="dnn" %>
<%@ Register Src="~/controls/SectionHeadControl.ascx" TagName="sectionhead" TagPrefix="dnn" %>
<%@ Register Src="~/controls/URLControl.ascx" TagName="url" TagPrefix="dnn" %>


<style type="text/css">
    .auto-style1 {
        height: 49px;
    }
</style>

<article:Header ID="ucHeader1" SelectedMenu="AdminOptions" runat="server" MenuPosition="Top"></article:Header>

<div id="dnnUsers" class="dnnForm dnnUsers dnnClear">
    <dnn:DnnScriptBlock ID="RadScriptBlock1" runat="server">
        <script type="text/javascript">
            function onContextClicking(sender, eventArgs) {
                var id = '<%=ctlContext.ClientID%>';
                var item = eventArgs.get_menuItem();
                var cmd = item.get_value();
                var attributes = item.get_attributes();
                if (cmd == 'delete' && !attributes.getAttribute("confirm")) {
                    item.get_menu().hide();
                    $("<a href='#' />").dnnConfirm({
                        text: '<%=GetConfirmString()%>',
                        callbackTrue: function () {
                            attributes.setAttribute("confirm", true);
                            item.click();
                        }
                    }).click();
                    eventArgs.set_cancel(true);
                }
                /*get current node to set hash*/
                var nodeValue = eventArgs.get_node().get_value();
                location.hash = "#" + $("input[type=radio][name$=rblMode]:checked").val() + "&" + nodeValue;
            }
            function onContextShowing(sender, eventArgs) {
                var node = eventArgs.get_node();
                var menu = eventArgs.get_menu();
                if (node) {
                    var a = node.get_attributes();

                    menu.findItemByValue('view').set_visible(a.getAttribute("CanView") == 'True');
                    menu.findItemByValue('edit').set_visible(a.getAttribute("CanEdit") == 'True');
                    menu.findItemByValue('add').set_visible(a.getAttribute("CanAdd") == 'True');
                    menu.findItemByValue('delete').set_visible(a.getAttribute("CanDelete") == 'True');

                }
            }
            function OnClientNodeClicked(sender, eventArgs) {
                var nodeValue = eventArgs.get_node().get_value();
                location.hash = "#" + $("input[type=radio][name$=rblMode]:checked").val() + "&" + nodeValue;
            }
        </script>
    </dnn:DnnScriptBlock>
    <div class="clearfix">
        <div class="left_trees">
            <dnn:DnnTreeView ID="ctlCategory" runat="server" AllowNodeEditing="true" CssClass="dnnTreePages" EnableDragAndDropBetweenNodes="true" OnClientContextMenuItemClicking="onContextClicking" OnClientContextMenuShowing="onContextShowing" OnClientNodeClicked="OnClientNodeClicked">
                <ContextMenus>
                    <dnn:DnnTreeViewContextMenu ID="ctlContext" runat="server">
                        <Items>
                            <dnn:DnnMenuItem Text="View" Value="view" ImageUrl="~/DesktopModules/GcDesign-NewsArticles/Images/Icon_View.png" />
                            <dnn:DnnMenuItem Text="Add" Value="add" ImageUrl="~/DesktopModules/GcDesign-NewsArticles/Images/Icon_Add.png" />
                            <dnn:DnnMenuItem Text="Edit" Value="edit" ImageUrl="~/DesktopModules/GcDesign-NewsArticles/Images/Icon_Edit.png" />
                            <dnn:DnnMenuItem Text="Delete" Value="delete" ImageUrl="~/DesktopModules/GcDesign-NewsArticles/Images/Icon_Delete.png" />
                        </Items>
                    </dnn:DnnTreeViewContextMenu>
                </ContextMenus>
            </dnn:DnnTreeView>
            <asp:Button ID="btnAdd" runat="server" Text="Add Category" Visible="false" />
        </div>
        

        <div class="right_users">
            <div class="tmTabContainer" runat="server" visible="true" id="pnlDetails">
                <asp:Panel ID="pnlSettings" runat="server" CssClass="WorkPanel" Visible="True">
                    <div id="categoryStatus" runat="server">Category Details</div>
                    <dnn:sectionhead ID="dshCategory" runat="server" CssClass="Head" IncludeRule="True" ResourceKey="CategorySettings" Section="tblCategory" Text="Category Settings" />
                    <table id="tblCategory" runat="server" cellspacing="0" cellpadding="2" width="525" summary="Category Design Table" border="0">
                        <tr>
                            <td colspan="3">
                                <asp:Label ID="lblCategoryHelp" resourcekey="CategorySettingsDescription" CssClass="Normal" runat="server" EnableViewState="False"></asp:Label></td>
                        </tr>
                        <tr valign="top">
                            <td width="25"></td>
                            <td class="SubHead" width="150">
                                <dnn:Label ID="plParent" ResourceKey="Parent" runat="server" ControlName="drpParent" Suffix=":"></dnn:Label>
                            </td>
                            <td align="left" width="325">
                                <asp:DropDownList ID="drpParentCategory" runat="server" CssClass="NormalTextBox" DataTextField="NameIndented" DataValueField="CategoryID" />
                                <asp:CustomValidator ID="valInvalidParentCategory" runat="server" ErrorMessage="<br>Invalid Parent Category. Possible Loop Detected."
                                    ResourceKey="valInvalidParentCategory" ControlToValidate="drpParentCategory" CssClass="NormalRed" Display="Dynamic"></asp:CustomValidator>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td width="25"></td>
                            <td class="SubHead" width="150">
                                <dnn:Label ID="plName" ResourceKey="Name" runat="server" ControlName="txtName" Suffix=":"></dnn:Label>
                            </td>
                            <td align="left" width="325">
                                <asp:TextBox ID="txtName" CssClass="NormalTextBox" Width="325" Columns="30" MaxLength="255" runat="server" />
                                <asp:RequiredFieldValidator ID="valName" resourcekey="valName" Display="Dynamic" runat="server" ErrorMessage="<br>You Must Enter a Valid Category Name" ControlToValidate="txtName" CssClass="NormalRed" EnableClientScript="False" />
                            </td>
                        </tr>
                        <tr valign="top">
                            <td width="25"></td>
                            <td class="SubHead" width="150">
                                <dnn:Label ID="plDescription" ResourceKey="Description" runat="server" ControlName="txtDescription" Suffix=":"></dnn:Label>
                            </td>
                            <td align="left" width="325"></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td colspan="2">
                                <dnn:texteditor id="txtDescription" width="100%" runat="server" height="400"></dnn:texteditor>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td width="25"></td>
                            <td class="SubHead" width="150" valign="top">
                                <dnn:Label ID="plCategoryImage" ResourceKey="CategoryImage" runat="server" ControlName="ctlImageLink" Suffix=":"></dnn:Label>
                            </td>
                            <td>
                                <dnn:url ID="ctlIcon" runat="server" Width="300" ShowLog="False" ShowTabs="false" ShowUrls="false" ShowTrack="false" FileFilter="jpg,jpeg,jpe,gif,bmp,png" Required="false"></dnn:url>
                            </td>
                        </tr>
                        <tr>
                            <td width="25"></td>
                            <td colspan="2">
                                <br />
                                <dnn:sectionhead ID="dshSecurity" CssClass="Head" runat="server" Text="Security Settings" Section="tblSecurity"
                                    ResourceKey="Security" IncludeRule="True" IsExpanded="True" />
                                <table id="tblSecurity" cellspacing="2" cellpadding="2" summary="Security Design Table" border="0" runat="server">
                                    <tr>
                                        <td class="SubHead" width="150">
                                            <dnn:Label ID="plInheritSecurity" ResourceKey="InheritSecurity" runat="server" ControlName="chkInheritSecurity" Suffix=":"></dnn:Label>
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="chkInheritSecurity" runat="server" AutoPostBack="true" />
                                        </td>
                                    </tr>
                                    <tr runat="server" id="trPermissions">
                                        <td class="SubHead" width="150">
                                            <dnn:Label ID="plPermissions" ResourceKey="Permissions" runat="server" ControlName="chkPermissions" Suffix=":"></dnn:Label>
                                        </td>
                                        <td>
                                            <asp:DataGrid ID="grdCategoryPermissions" runat="server" AutoGenerateColumns="False" ItemStyle-CssClass="Normal"
                                                ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" HeaderStyle-HorizontalAlign="Center"
                                                HeaderStyle-CssClass="NormalBold" CellSpacing="0" CellPadding="0" GridLines="None" BorderWidth="1"
                                                BorderStyle="None" DataKeyField="Value">
                                                <Columns>
                                                    <asp:TemplateColumn>
                                                        <ItemStyle HorizontalAlign="Left" Wrap="False" />
                                                        <ItemTemplate>
                                                            <%# DataBinder.Eval(Container.DataItem, "Text") %>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn>
                                                        <HeaderTemplate>
                                                            &nbsp;
		                                    <asp:Label ID="lblView" runat="server" EnableViewState="False" ResourceKey="View" />&nbsp;
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkView" runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn>
                                                        <HeaderTemplate>
                                                            &nbsp;
		                                    <asp:Label ID="lblSubmit" runat="server" EnableViewState="False" ResourceKey="Submit" />&nbsp;
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkSubmit" runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                </Columns>
                                            </asp:DataGrid>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="trSecurityMode">
                                        <td class="SubHead" width="150">
                                            <dnn:Label ID="plSecurityMode" ResourceKey="SecurityMode" runat="server" ControlName="chkSecurityMode" Suffix=":"></dnn:Label>
                                        </td>
                                        <td>
                                            <asp:RadioButtonList ID="lstSecurityMode" runat="server" CssClass="Normal" RepeatDirection="Vertical" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td width="25"></td>
                            <td colspan="2">
                                <br />
                                <dnn:sectionhead ID="dshMeta" CssClass="Head" runat="server" Text="Meta" Section="tblMeta"
                                    ResourceKey="Meta" IncludeRule="True" IsExpanded="False" />
                                <table id="tblMeta" cellspacing="2" cellpadding="2" summary="Meta Design Table" border="0" runat="server">
                                    <tr>
                                        <td class="SubHead" width="150">
                                            <dnn:Label ID="plMetaTitle" runat="server" Suffix=":" ControlName="txtMetaTitle"></dnn:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtMetaTitle" CssClass="NormalTextBox" runat="server" MaxLength="200" Width="300"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="SubHead" width="150">
                                            <dnn:Label ID="plMetaDescription" runat="server" Suffix=":" ControlName="txtMetaDescription"></dnn:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtMetaDescription" CssClass="NormalTextBox" runat="server" MaxLength="500" Width="300"
                                                TextMode="MultiLine" Rows="3"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="auto-style1" width="150">
                                            <dnn:Label ID="plMetaKeyWords" runat="server" Suffix=":" ControlName="txtMetaKeyWords"></dnn:Label>
                                        </td>
                                        <td class="auto-style1">
                                            <asp:TextBox ID="txtMetaKeyWords" CssClass="NormalTextBox" runat="server" MaxLength="500" Width="300"
                                                TextMode="MultiLine" Rows="3"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <div id="editBtnGroup" runat="server" visible="false">
                    <asp:LinkButton ID="cmdUpdate" resourcekey="cmdUpdate" runat="server" CssClass="CommandButton" Text="Update" BorderStyle="none" />
                    &nbsp;
	<asp:LinkButton ID="cmdDelete" resourcekey="cmdDelete" runat="server" CssClass="CommandButton" Text="Delete" CausesValidation="False" BorderStyle="none" />
                </div>
            </div>
        </div>
    </div>
</div>

<script language="javascript" type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function setUpDnnUsers() {
            $('.dnnSecurityRolesGrid td input[type="image"]').click(function (e, isTrigger) {
                if (isTrigger) {
                    return true;
                }

                var $this = $(this);
                var name = $this.attr('name');
                var text = '<%= Localization.GetSafeJSString("RemoveItems.Confirm", Localization.SharedResourceFile) %>';
                if (name.indexOf('Delete') > 0) {
                    text = '<%= Localization.GetSafeJSString("Delete.Confirm", LocalResourceFile) %>';
                }
                else if (name.indexOf('Restore') > 0) {
                    text = '<%= Localization.GetSafeJSString("Restore.Confirm", LocalResourceFile) %>';
                }
                else if (name.indexOf('Remove') > 0) {
                    text = '<%= Localization.GetSafeJSString("Remove.Confirm", LocalResourceFile) %>';
                }

                var opts = {
                    text: text,
                    yesText: '<%= Localization.GetSafeJSString("Yes.Text", Localization.SharedResourceFile) %>',
                    noText: '<%= Localization.GetSafeJSString("No.Text", Localization.SharedResourceFile) %>',
                    title: '<%= Localization.GetSafeJSString("Confirm.Text", Localization.SharedResourceFile) %>',
                    autoOpen: false,
                    resizable: false,
                    modal: true,
                    dialogClass: 'dnnFormPopup dnnClear',
                    isButton: false
                };
                var $dnnDialog = $("<div class='dnnDialog'></div>").html(opts.text).dialog(opts);
                if ($dnnDialog.is(':visible')) {
                    $dnnDialog.dialog("close");
                    return false;
                }

                $dnnDialog.dialog({
                    open: function () {
                        $('.ui-dialog-buttonpane').find('button:contains("' + opts.noText + '")').addClass('dnnConfirmCancel');
                    },
                    position: 'center',
                    buttons: [
                        {
                            text: opts.yesText,
                            click: function () {
                                $dnnDialog.dialog("close");
                                $this.trigger("click", [true]);
                            }
                        },
                        {
                            text: opts.noText,
                            click: function () {
                                $(this).dialog("close");
                            }
                        }
                    ]
                });
                $dnnDialog.dialog('open');
                return false;
            });
        }

        $(document).ready(function () {
            setUpDnnUsers();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                setUpDnnUsers();
            });
        });
    }(jQuery, window.Sys));
</script>

<article:Header ID="ucHeader2" SelectedMenu="AdminOptions" runat="server" MenuPosition="Bottom"></article:Header>



