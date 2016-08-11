<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="GcSearchCourses.ascx.cs" Inherits="GcDesign.NewsArticles.GcSearchCourses" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="GcDesign" Namespace="GcDesign.NewsArticles.Components.WebControls" Assembly="GcDesign.NewsArticles" %>

<dnn:DnnCssInclude ID="DnnCssInclude1" runat="server" FilePath="~/DesktopModules/GcDesign-NewsArticles-NewsSearch/search.css" />

<link href="../search.css" rel="stylesheet" />

<asp:Label runat="server" EnableViewState="False" ID="lblNotConfigured" ResourceKey="NotConfigured" Visible="False" CssClass="Normal" />

<script type="text/javascript">
    var search_data = {
        courses_data: 48,
        organization_data: 49,
        postback: false
    }

    function ajaxPost() {
        $.post("<%= Request.RawUrl %>", search_data,
            function (data, textStatus) {
                var url = data.split("$link$")[1];
                if (url) {
                    // data.redirect contains the string URL to redirect to
                    //window.location.replace(url);
                    $(".newsarticles").html($(data).find(".newsarticles").html());
                }
                else {
                    // data.form contains the HTML for the replacement form
                    //$("#myform").replaceWith(data.form);
                }
            }
       );
    }

    $(function () {
        $("#courses_data > a").on('click', function () {
            search_data.courses_data = $(this).attr('data-value');
            search_data.postback = true;

            if ($(this).text() == $("#courses_data > a").first().text()) {
                $("#SearchConditionPane").find("a[name='courses_data']").remove();
            }
            else {
                if ($("#SearchConditionPane a[name='courses_data']") != undefined && $("#SearchConditionPane a[name='courses_data']").text() != $(this).text()) {
                    $("#SearchConditionPane a[name='courses_data']").remove();
                    var element = $("<a name='courses_data'>" + $(this).text() + "</a>");
                    element.on('click', element, function (e) { $(e.data).remove(); search_data.courses_data = 48; ajaxPost(); });
                    $("#SearchConditionPane").append(element);
                }
            }

            ajaxPost();
        })

        $("#organization_data > a").on('click', function () {
            search_data.organization_data = $(this).attr('data-value');
            search_data.postback = true;

            if ($(this).text() == $("#organization_data > a").first().text()) {
                $("#SearchConditionPane").find("a[name='organization_data']").remove();
            }
            else {
                if ($("#SearchConditionPane a[name='organization_data']") != undefined && $("#SearchConditionPane a[name='organization_data']").text() != $(this).text()) {
                    $("#SearchConditionPane a[name='organization_data']").remove();
                    var element = $("<a name='organization_data'>" + $(this).text() + "</a>");
                    element.on('click', element, function (e) { $(e.data).remove(); search_data.organization_data = 49; ajaxPost() });
                    $("#SearchConditionPane").append(element);
                }
            }

            ajaxPost();

        })

    })
</script>
<div class="search_wrap">
    <div class="searchWindow">
        <h2>课程选择</h2>
        <div class="clearfix">
            <h3>课程分类</h3>
            <div id="courses_data">
                <a href='javascript:void(0)' data-value='<%= courses.CategoryID %>'>全部</a>
                <asp:ListView ID="lvwCourses" runat="server">
                    <ItemTemplate>
                        <a href='javascript:void(0)' data-value='<%# Eval("CategoryID") %>'><%# Eval("Name") %></a>
                    </ItemTemplate>
                </asp:ListView>
            </div>
        </div>
        <div class="clearfix">
            <h3>高校所在</h3>
            <div id="organization_data">
                <a href='javascript:void(0)' data-value='<%= organization.CategoryID %>'>全部</a>
                <asp:ListView ID="lvwOrganization" runat="server">
                    <ItemTemplate>
                        <a href='javascript:void(0)' data-value='<%# Eval("CategoryID") %>'><%# Eval("Name") %></a>
                    </ItemTemplate>
                </asp:ListView>
            </div>
        </div>
        <div class="conditionPane">
            <span>已选条件</span><div id="SearchConditionPane"></div>
        </div>

    </div>
    <div class="newsarticles">
        <asp:PlaceHolder ID="phNoArticles" runat="Server" />
        <asp:Repeater ID="rptLatestArticles" runat="server" EnableViewState="False">
            <HeaderTemplate>
                <ul class="searchList clearfix">
            </HeaderTemplate>
            <ItemTemplate>
                <li>
                    <p class="coursep"><%# Eval("Summary")%></p>
                    <a href='<%# GenerateArticleUrl(Container.DataItem) %>' target="_blank"><%# Eval("Title")%></a>
                </li>
            </ItemTemplate>
            <FooterTemplate></ul></FooterTemplate>
        </asp:Repeater>
        <div class="tablepage">
            <GcDesign:PagingControl ID="ctlPagingControl" runat="server" Visible="false"></GcDesign:PagingControl>
        </div>
    </div>
</div>
