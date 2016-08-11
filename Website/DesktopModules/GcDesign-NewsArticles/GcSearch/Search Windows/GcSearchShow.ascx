<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="GcSearchShow.ascx.cs" Inherits="GcDesign.NewsArticles.GcSearchShow" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="GcDesign" Namespace="GcDesign.NewsArticles.Components.WebControls" Assembly="GcDesign.NewsArticles" %>

<dnn:DnnCssInclude ID="DnnCssInclude1" runat="server" FilePath="~/DesktopModules/GcDesign-NewsArticles-NewsSearch/search.css" />

<link href="../search.css" rel="stylesheet" />

<asp:Label runat="server" EnableViewState="False" ID="lblNotConfigured" ResourceKey="NotConfigured" Visible="False" CssClass="Normal" />

<script type="text/javascript">
    var search_data = {
        courses_data: 58,
        place_data: 59,
        time_data: "year", //escape((new Date()).toLocaleDateString()),
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
                    element.on('click', element, function (e) { $(e.data).remove(); search_data.courses_data = 58; ajaxPost(); });
                    $("#SearchConditionPane").append(element);
                }
            }

            ajaxPost();
        })


        $("#place_data > a").on('click', function () {
            search_data.place_data = $(this).attr('data-value');
            search_data.postback = true;

            if ($(this).text() == $("#place_data > a").first().text()) {
                $("#SearchConditionPane").find("a[name='place_data']").remove();
            }
            else {
                if ($("#SearchConditionPane a[name='place_data']") != undefined && $("#SearchConditionPane a[name='place_data']").text() != $(this).text()) {
                    $("#SearchConditionPane a[name='place_data']").remove();
                    var element = $("<a name='place_data'>" + $(this).text() + "</a>");
                    element.on('click', element, function (e) { $(e.data).remove(); search_data.place_data = 59; ajaxPost(); });
                    $("#SearchConditionPane").append(element);
                }
            }

            ajaxPost();

        })

        $("#time_data > a").on('click', function () {
            search_data.time_data = $(this).attr('data-value');
            search_data.postback = true;

            if ($(this).text() == $("#time_data > a").first().text()) {
                $("#SearchConditionPane").find("a[name='time_data']").remove();
            }
            else {
                if ($("#SearchConditionPane a[name='time_data']") != undefined && $("#SearchConditionPane a[name='time_data']").text() != $(this).text()) {
                    $("#SearchConditionPane a[name='time_data']").remove();
                    var element = $("<a name='time_data'>" + $(this).text() + "</a>");
                    element.on('click', element, function (e) { $(e.data).remove(); search_data.time_data = "year"; ajaxPost(); });
                    $("#SearchConditionPane").append(element);
                }
            }

            ajaxPost();

        })

    })
</script>

<div class="search_wrap">
    <div class="searchWindow">
        <h2>展会选择</h2>
        <div class="clearfix">
            <h3>展会分类</h3>
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
            <h3>展会地点</h3>
            <div id="place_data">
                <a href='javascript:void(0)' data-value='<%= place.CategoryID %>'>全部</a>
                <asp:ListView ID="lvwPlace" runat="server">
                    <ItemTemplate>
                        <a href='javascript:void(0)' data-value='<%# Eval("CategoryID") %>'><%# Eval("Name") %></a>
                    </ItemTemplate>
                </asp:ListView>
            </div>
        </div>
        <div class="clearfix">
            <h3>展会时间</h3>
            <div id="time_data">
                <a href='javascript:void(0)' data-value='year'>全部</a>
                <asp:ListView ID="lvwTime" runat="server">
                    <ItemTemplate>
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
                <ul class="searchList tradeList clearfix">
            </HeaderTemplate>
            <ItemTemplate>
                <li><a href='<%# GenerateArticleUrl(Container.DataItem) %>' target="_blank"><%# Eval("Title")%></a><div id="desWrap" runat="server"></div>
                </li>
            </ItemTemplate>
            <FooterTemplate></ul></FooterTemplate>
        </asp:Repeater>
        <div class="tablepage">
            <GcDesign:PagingControl ID="ctlPagingControl" runat="server" Visible="false"></GcDesign:PagingControl>
        </div>
    </div>
</div>
