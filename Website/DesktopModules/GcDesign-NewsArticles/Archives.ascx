<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Archives.ascx.cs" Inherits="GcDesign.NewsArticles.Archives" %>
<%@ Register TagPrefix="article" TagName="Header" Src="ucHeader.ascx" %>
<article:header id="Header1" SelectedMenu="Categories" runat="server" MenuPosition="Top" />
<div align="left">

    <asp:PlaceHolder ID="phCurrentArticles" runat="server">
    <h2><asp:Label ID="lblCurrentArticles" runat="server" EnableViewState="False" ResourceKey="CurrentArticles" /></h2>
    <div class="Normal" style="margin-left: 25px;">
        <asp:PlaceHolder ID="phArchives" runat="server"><a href="<%= this.GetCurrentLinkRss() %>" rel="nofollow"><img src="<%= GetRssPath() %>" alt="Latest RSS Link" border="0"  /></a>&nbsp;</asp:PlaceHolder>
        <a href="<%= GetCurrentLink() %>"><asp:Label ID="lblLatest" runat="server" resourcekey="Latest25" /></a>
    </div>
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="phCategory" runat="server">
    <h2><asp:Label ID="lblByCategory" runat="server" EnableViewState="False" ResourceKey="ByCategory" /></h2>
    <asp:Repeater ID="rptCategories" Runat="server" EnableViewState="False">
        <HeaderTemplate>
            <div class="Normal" style="margin-left: 25px;">
        </HeaderTemplate>
	    <ItemTemplate>
                <asp:PlaceHolder ID="phArchives" runat="server" Visible="<%# IsSyndicationEnabled() %>"><a href="<%# GetCategoryLinkRss(DataBinder.Eval(Container.DataItem, "CategoryID").ToString()) %>" rel="nofollow"><img src="<%# GetRssPath() %>" alt="Category RSS Link" border="0"  /></a>&nbsp;</asp:PlaceHolder>
                <a href="<%# GetCategoryLink(DataBinder.Eval(Container.DataItem, "CategoryID").ToString(), DataBinder.Eval(Container.DataItem, "Name").ToString()) %>"><%#DataBinder.Eval(Container.DataItem, "NameIndented")%> (<%# DataBinder.Eval(Container.DataItem, "NumberOfArticles") %>)</a>
                <br />
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phAuthor" runat="server">
    <h2><asp:Label ID="lblByAuthor" runat="server" EnableViewState="False" ResourceKey="ByAuthor" /></h2>
    <asp:Repeater ID="rptAuthors" Runat="server" EnableViewState="False">
        <HeaderTemplate>
            <div class="Normal" style="margin-left: 25px;">
        </HeaderTemplate>
	    <ItemTemplate>
                <asp:PlaceHolder ID="phArchives" runat="server" Visible="<%# IsSyndicationEnabled() %>"><a href="<%# GetAuthorLinkRss(DataBinder.Eval(Container.DataItem, "UserID").ToString()) %>" rel="nofollow"><img src="<%# GetRssPath() %>" alt="Author RSS Link" border="0" /></a>&nbsp;</asp:PlaceHolder>
                <a href="<%# GetAuthorLink((int)DataBinder.Eval(Container.DataItem, "UserID"), DataBinder.Eval(Container.DataItem, "Username").ToString()) %>"><%#DataBinder.Eval(Container.DataItem, "DisplayName")%> (<%#DataBinder.Eval(Container.DataItem, "ArticleCount")%>)</a>
                <br />
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phMonth" runat="server">
    <h2><asp:Label ID="lblByMonth" runat="server" EnableViewState="False" ResourceKey="ByMonth" /></h2>
    <asp:Repeater ID="rptMonth" Runat="server" EnableViewState="False">
        <HeaderTemplate>
            <div class="Normal" style="margin-left: 25px;">
        </HeaderTemplate>
	    <ItemTemplate>
                <asp:PlaceHolder ID="phArchives" runat="server" Visible="<%# IsSyndicationEnabled() %>"><a href="<%# GetMonthLinkRss((int)DataBinder.Eval(Container.DataItem, "Month"), (int)DataBinder.Eval(Container.DataItem, "Year")) %>" rel="nofollow"><img src="<%# GetRssPath() %>" alt="Month RSS Link" border="0" /></a>&nbsp;</asp:PlaceHolder>
                <a href="<%# GetMonthLink((int)DataBinder.Eval(Container.DataItem, "Month"), (int)DataBinder.Eval(Container.DataItem, "Year")) %>"><%#GetMonthName((int)DataBinder.Eval(Container.DataItem, "Month"))%> <%#DataBinder.Eval(Container.DataItem, "Year")%> (<%#DataBinder.Eval(Container.DataItem, "Count")%>)</a>
                <br />
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>
    </asp:PlaceHolder>
    
</div>
<article:header id="Header2" SelectedMenu="Categories" runat="server" MenuPosition="Bottom" />