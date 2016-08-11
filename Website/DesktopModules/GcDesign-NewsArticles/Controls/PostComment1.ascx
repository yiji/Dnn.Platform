<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="PostComment1.ascx.cs" Inherits="GcDesign.NewsArticles.Controls.PostComment1" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<asp:PlaceHolder ID="phCommentForm" runat="Server">
    <p id="pUrl" runat="server">
        <asp:Label ID="lblUrl" runat="server" CssClass="Normal w" Text="Website" />
        <asp:TextBox ID="txtURL" CssClass="NormalTextBox" runat="server" />

    </p>
    <p>
        <asp:Label ID="lblCourse" runat="server" CssClass="Normal w" Text="展会名" />
        <asp:TextBox ID="txtCourse" CssClass="NormalTextBox" runat="server" />
    </p>
    <p>
        <asp:Label ID="lblCustomerName" runat="server" CssClass="Normal w" Text="姓名" />
        <asp:TextBox ID="txtCustomerName" CssClass="NormalTextBox" runat="server" />
        <asp:RequiredFieldValidator ID="valName" CssClass="NormalRed" runat="server"
            ControlToValidate="txtCustomerName" Display="Dynamic" ErrorMessage="Name Is Required" SetFocusOnError="true" ValidationGroup="PostComment" />
    </p>
    <p>
        <asp:Label ID="lblPhone" runat="server" CssClass="Normal w" Text="联系电话" />
        <asp:TextBox ID="txtPhone" CssClass="NormalTextBox" runat="server" />
        <asp:RequiredFieldValidator ID="valphone" CssClass="NormalRed" runat="server"
            ControlToValidate="txtPhone" ErrorMessage="<br>phone Is Required" Display="Dynamic" SetFocusOnError="true" ValidationGroup="PostComment" />
    </p>
    <p>
        <asp:Label ID="lblQQ" runat="server" CssClass="Normal w" Text="QQ" />
        <asp:TextBox ID="txtQQ" CssClass="NormalTextBox" runat="server" />

    </p>
    <p id="pEmail" runat="server">
        <asp:Label ID="lblEmail" runat="server" CssClass="Normal w" Text="Email" />
        <asp:TextBox ID="txtEmail" CssClass="NormalTextBox" runat="server" />

        <asp:RequiredFieldValidator ID="valEmail" CssClass="NormalRed" runat="server"
            ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="Email Is Required" SetFocusOnError="true" ValidationGroup="PostComment" />
        <asp:RegularExpressionValidator ID="valEmailIsValid" CssClass="NormalRed" runat="server"
            ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="Invalid Email Address" SetFocusOnError="true" ValidationGroup="PostComment" ValidationExpression="^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$" />
    </p>
    <p>
        <asp:Label ID="lblComment" runat="server" CssClass="Normal w" Text="备注" />
        <asp:TextBox ID="txtComment" CssClass="NormalTextBox" runat="server" TextMode="MultiLine" Width="450px" Height="150px"></asp:TextBox>

    </p>
    <!-- 验证码 -->
    <dnn:captchacontrol id="ctlCaptcha" captchawidth="130" captchaheight="40" cssclass="Normal" runat="server" errorstyle-cssclass="NormalRed" />
    <p class="submitBtn">
        <asp:Button ID="btnAddComment" runat="server" Text="Add Comment" ValidationGroup="PostComment" UseSubmitBehavior="false" />
    </p>
</asp:PlaceHolder>

<asp:PlaceHolder ID="phCommentPosted" runat="Server" Visible="false">
    <asp:Label ID="lblRequiresApproval" runat="server" EnableViewState="False" CssClass="Normal" Text="Your comment has been submitted, but requires approval." />
</asp:PlaceHolder>
<asp:PlaceHolder ID="phCommentAnonymous" runat="Server" Visible="false">
    <asp:Label ID="lblRequiresAccess" runat="server" CssClass="Normal" Text="Only registered users may post comments." />
    <asp:HyperLink ID="loginLink" runat="server" CssClass="popurl" Text="点击登陆" />
</asp:PlaceHolder>
