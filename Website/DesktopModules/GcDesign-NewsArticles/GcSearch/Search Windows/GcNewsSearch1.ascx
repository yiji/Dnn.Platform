<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="GcNewsSearch1.ascx.cs" Inherits="GcDesign.NewsArticles.GcNewsSearch1" %>
<link rel="stylesheet" type="text/css" href="<%= ControlPath %>search.css"/>
<link rel="stylesheet" type="text/css" href="<%= ControlPath %>js/calendar.css"/>
<asp:Label Runat="server" EnableViewState="False" ID="lblNotConfigured" ResourceKey="NotConfigured"
	Visible="False" CssClass="Normal" />
<asp:PlaceHolder ID="phSearchForm" runat="server">
<div align="center" id="articleSearchFormSmall" >
    <asp:Panel ID="pnlSearch" runat="server">
        <%--<asp:TextBox ID="txtSearch" runat="server" CssClass="NormalTextBox" />
        <asp:Button ID="btnSearch" runat="server" Text="Search" ResourceKey="Search"  />--%>
        <div id="calender_wrap">
      <h2>搜索旅行中的首选之家</h2>
      <p><span>入住城市</span>
        <input type="text" disabled="disabled" class="f-text c9" autocomplete="off" value="三亚" >
      </p>
      <p><span>入住日期</span>
        <input type="text" name="startDate" readonly="" autocomplete="off" value="" class="f-text c9" id="lv_date_y" >
      </p>
      <p><span>退房日期</span>
        <input type="text" name="endDate" readonly="" autocomplete="off" value="" class="f-text c9" id="lv_date_r">
      </p>
      <p><span>酒店级别</span>
        <select name="" style="width:169px; height:30px; vertical-align:middle">
          <option selected="selected">不限</option>
        </select>
      </p>
      <p><span>关键词</span>
          <%--<input type="text" name="keyword" autocomplete="off" class="f-text c9" >--%>
          <asp:TextBox ID="txtSearch" runat="server" CssClass="f-text c9" />
      </p>
      <p><asp:Button ID="btnSearch" runat="server" Text="Search" ResourceKey="Search" CssClass="btnserach_bg"  /></p>
      <script src="<%= ControlPath %>js/calendar.js" type="text/javascript" ></script> 
      <script src="<%= ControlPath %>js/calendar-data.js" type="text/javascript" ></script> 
    </div>
    </asp:Panel>
</div>
</asp:PlaceHolder>