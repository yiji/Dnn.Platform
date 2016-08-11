<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ViewArticle.ascx.cs" Inherits="GcDesign.NewsArticles.ViewArticle" %>
<%@ Register TagPrefix="GcDesign" TagName="Header" Src="ucHeader.ascx" %>
<GcDesign:Header id="ucHeader1" SelectedMenu="CurrentArticles" runat="server" MenuPosition="Top" />
<asp:Literal ID="litPingback" Runat="server" EnableViewState="False" Visible="True"></asp:Literal>
<asp:Literal ID="litRDF" Runat="server" EnableViewState="False" Visible="True"></asp:Literal>
<asp:PlaceHolder ID="phArticle" runat="server" />
<GcDesign:Header id="ucHeader2" SelectedMenu="CurrentArticles" runat="server" MenuPosition="Bottom" />

<script type="text/javascript">
    $('.NewsArticles a[href]').filter(function () {
        return /(jpg|gif|png)$/.test($(this).attr('href'));
    }).attr('rel', 'shadowbox[<%= GetArticleID() %>]');

    Shadowbox.init({
        handleOversize: "drag"
    });



</script>