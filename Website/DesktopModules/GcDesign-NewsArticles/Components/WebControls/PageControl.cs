using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel;
using System.Web.UI;
using DotNetNuke;
using DotNetNuke.Common;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;

namespace GcDesign.NewsArticles.Components.WebControls
{
    [ToolboxData("<{0}:PagingControl runat=server></{0}:PagingControl>")]
    public class PageControl : System.Web.UI.WebControls.WebControl
    {

        protected HtmlGenericControl divPageNumbers;
        protected System.Web.UI.WebControls.Repeater PageNumbers;
        protected HtmlGenericControl ulDisplayStatus;
        protected HtmlGenericControl ulDisplayLinks;

        private int TotalPages = -1;

        private int _TotalRecords;
        private int _PageSize;
        private int _CurrentPage;
        private string _QuerystringParams;
        private string _PageParam = "lapg";
        private int _TabID;
        private string _CSSClassLinkActive;
        private string _CSSClassLinkInactive;
        private string _CSSClassPagingStatus;

        [Bindable(true), Category("Behavior"), DefaultValue("0")]
        internal int TotalRecords
        {
            get
            {
                return _TotalRecords;
            }

            set
            {
                _TotalRecords = value;
            }
        }
        [Bindable(true), Category("Behavior"), DefaultValue("10")]
        internal int PageSize
        {
            get
            {
                return _PageSize;
            }

            set
            {
                _PageSize = value;
            }
        }
        [Bindable(true), Category("Behavior"), DefaultValue("1")]
        internal int CurrentPage
        {
            get
            {
                return _CurrentPage;
            }

            set
            {
                _CurrentPage = value;
            }
        }
        [Bindable(true), Category("Behavior"), DefaultValue("")]
        internal string QuerystringParams
        {
            get
            {
                return _QuerystringParams;
            }

            set
            {
                _QuerystringParams = value;
            }
        }
        [Bindable(true), Category("Behavior"), DefaultValue("-1")]
        internal int TabID
        {
            get
            {
                return _TabID;
            }

            set
            {
                _TabID = value;
            }
        }
        [Bindable(true), Category("Behavior"), DefaultValue("lapg")]
        internal string PageParam
        {
            get
            {
                return _PageParam;
            }

            set
            {
                _PageParam = value;
            }
        }
        [Bindable(true), Category("Behavior"), DefaultValue("Normal")]
        internal string CSSClassLinkActive
        {
            get
            {
                if (_CSSClassLinkActive == "")
                {
                    return "CommandButton";
                }
                else
                {
                    return _CSSClassLinkActive;
                }
            }

            set
            {
                _CSSClassLinkActive = value;
            }
        }
        [Bindable(true), Category("Behavior"), DefaultValue("CommandButton")]
        internal string CSSClassLinkInactive
        {
            get
            {
                if (_CSSClassLinkInactive == "")
                {
                    return "NormalDisabled";
                }
                else
                {
                    return _CSSClassLinkInactive;
                }
            }

            set
            {
                _CSSClassLinkInactive = value;
            }
        }
        [Bindable(true), Category("Behavior"), DefaultValue("Normal")]
        internal string CSSClassPagingStatus
        {
            get
            {
                if (_CSSClassPagingStatus == "")
                {
                    return "Normal";
                }
                else
                {
                    return _CSSClassPagingStatus;
                }
            }

            set
            {
                _CSSClassPagingStatus = value;
            }
        }

        protected override void CreateChildControls()
        {
            divPageNumbers = new HtmlGenericControl("div");
            ulDisplayStatus = new HtmlGenericControl("ul");
            ulDisplayLinks = new HtmlGenericControl("ul");
            ulDisplayStatus.Attributes["class"] = "Normal";
            ulDisplayLinks.Attributes["class"] = "pagination";

            if (this.CssClass == "")
            {
                divPageNumbers.Attributes["class"] = "PagingDiv";
            }
            else
            {
                divPageNumbers.Attributes["class"] = this.CssClass;
            }

            //divPageNumbers.Controls.Add(new HtmlGenericControl("li"));

            PageNumbers = new Repeater();
            PageNumLinkTemplate I = new PageNumLinkTemplate(this);
            PageNumbers.ItemTemplate = I;
            BindPageNumbers(TotalRecords, PageSize);

            int intTotalPages = TotalPages;
            if (intTotalPages == 0)
            {
                intTotalPages = 1;
            }

            //显示 page 1 of 10
            //string str;
            //str = string.Format(DotNetNuke.Services.Localization.Localization.GetString("Pages", "~/GcDesign-NewsArticles/App_LocalResources/PageControl.resx"), CurrentPage.ToString(), intTotalPages.ToString());
            //LiteralControl lit = new LiteralControl(str);
            //ulDisplayStatus.Controls.Add(lit);

            //divPageNumbers.Controls.Add(ulDisplayStatus);
            divPageNumbers.Controls.Add(ulDisplayLinks);

        }

        protected override void Render(System.Web.UI.HtmlTextWriter output)
        {
            if (PageNumbers == null)
            {
                CreateChildControls();
            }

            System.Text.StringBuilder str = new System.Text.StringBuilder();

            //str.Append(GetFirstLink());
            str.Append(GetPreviousLink());
            System.Text.StringBuilder result = new System.Text.StringBuilder(1024);
            PageNumbers.RenderControl(new HtmlTextWriter(new System.IO.StringWriter(result)));
            str.Append(result.ToString());
            str.Append(GetNextLink());
            //str.Append(GetLastLink());
            ulDisplayLinks.Controls.Add(new LiteralControl(str.ToString()));

            divPageNumbers.RenderControl(output);

        }

        /// <summary>
        /// 要修改成根据当前页生成分页，最后一页时从最后一页到-显示的页数，中间时判断与最后一页的间隔，再生成
        /// </summary>
        /// <param name="TotalRecords"></param>
        /// <param name="RecordsPerPage"></param>
        private void BindPageNumbers(int TotalRecords, int RecordsPerPage){
            int PageLinksPerPage = 5;
            if(TotalRecords / RecordsPerPage >= 1){
                TotalPages = Convert.ToInt32(Math.Ceiling(((double)TotalRecords / RecordsPerPage)));
            }
            else
            {
                TotalPages = 0;
            }

            if (TotalPages > 0) {
                DataTable ht = new DataTable();  
                ht.Columns.Add("PageNum");
                DataRow tmpRow;

                int LowNum = 1;
                int HighNum = Convert.ToInt32(TotalPages);

                double tmpNum;
                tmpNum = CurrentPage - PageLinksPerPage / 2;
                if (tmpNum < 1)
                {
                    tmpNum = 1;
                }

                if (CurrentPage > (PageLinksPerPage / 2)) {
                    LowNum =Convert.ToInt32(Math.Floor(tmpNum));
                }

                if (Convert.ToInt32(TotalPages) <= PageLinksPerPage){
                    HighNum = Convert.ToInt32(TotalPages);
                }
                else
                {
                    HighNum = LowNum + PageLinksPerPage - 1;
                }

                if (HighNum > Convert.ToInt32(TotalPages) ){
                    HighNum = Convert.ToInt32(TotalPages) ;
                    if (HighNum - LowNum < PageLinksPerPage){
                        LowNum = HighNum - PageLinksPerPage + 1;
                    }
                }

                if (HighNum > Convert.ToInt32(TotalPages))
                { HighNum = Convert.ToInt32(TotalPages);
                }
                if (LowNum < 1){ 
                    LowNum = 1;
                }

                int i;
                for( i = LowNum;i<=HighNum;i++){
                    tmpRow = ht.NewRow();
                    tmpRow["PageNum"] = i;
                    ht.Rows.Add(tmpRow);
                }

                PageNumbers.DataSource = ht;
                PageNumbers.DataBind();
            }

        }

        private string CreateURL(string CurrentPage) {

            if (QuerystringParams != ""){
                if (CurrentPage != ""){
                    return Globals.NavigateURL(TabID, "", QuerystringParams, PageParam + "=" + CurrentPage);
                }
                else
                {
                    return Globals.NavigateURL(TabID, "", QuerystringParams);
                }
            }
            else
            {
                if (CurrentPage != ""){
                    return Globals.NavigateURL(TabID, "", PageParam + "=" + CurrentPage);
                }
                else
                {
                    return Globals.NavigateURL(TabID);
                }
            }

        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetLink returns the page number links for paging.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[dancaron]	10/28/2004	Initial Version
        /// </history>
        /// -----------------------------------------------------------------------------
        private string GetLink(int PageNum)
        {
            if (PageNum == CurrentPage)
            {
                return "<span class=\"" + CSSClassLinkInactive + "\">" + PageNum.ToString() + "</span>";
            }
            else
            {
                return "<a href=\"" + CreateURL(PageNum.ToString()) + "\" class=\"" + CSSClassLinkActive + "\">" + PageNum.ToString() + "</a>";
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetPreviousLink returns the link for the Previous page for paging.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[dancaron]	10/28/2004	Initial Version
        /// </history>
        /// -----------------------------------------------------------------------------
        private string GetPreviousLink()
        {
            if (CurrentPage > 1 && TotalPages > 0)
            {
                return "<li><a href=\"" + CreateURL((CurrentPage - 1).ToString()) + "\" class=\"" + CSSClassLinkActive + "\">" + DotNetNuke.Services.Localization.Localization.GetString("Previous", "~/DesktopModules/GcDesign-NewsArticles/App_LocalResources/PageControl.resx") + "</a></li>";
            }
            else
            {
                return "<li><span class=\"" + CSSClassLinkInactive + "\">" + DotNetNuke.Services.Localization.Localization.GetString("Previous", "~/DesktopModules/GcDesign-NewsArticles/App_LocalResources/PageControl.resx") + "</span></li>";
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetNextLink returns the link for the Next Page for paging.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[dancaron]	10/28/2004	Initial Version
        /// </history>
        /// -----------------------------------------------------------------------------
        private string GetNextLink()
        {
            if (CurrentPage != TotalPages && TotalPages > 0)
            {
                return "<li><a href=\"" + CreateURL((CurrentPage + 1).ToString()) + "\" class=\"" + CSSClassLinkActive + "\">" + DotNetNuke.Services.Localization.Localization.GetString("Next", "~/DesktopModules/GcDesign-NewsArticles/App_LocalResources/PageControl.resx") + "</a></li>";
            }
            else
            {
                return "<li><span class=\"" + CSSClassLinkInactive + "\">" + DotNetNuke.Services.Localization.Localization.GetString("Next", "~/DesktopModules/GcDesign-NewsArticles/App_LocalResources/PageControl.resx") + "</span></li>";
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetFirstLink returns the First Page link for paging.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[dancaron]	10/28/2004	Initial Version
        /// </history>
        /// -----------------------------------------------------------------------------
        private string GetFirstLink()
        {
            if (CurrentPage > 1 && TotalPages > 0)
            {
                return "<a href=\"" + CreateURL("1") + "\" class=\"" + CSSClassLinkActive + "\">" + DotNetNuke.Services.Localization.Localization.GetString("First", DotNetNuke.Services.Localization.Localization.SharedResourceFile) + "</a>";
            }
            else
            {
                return "<span class=\"" + CSSClassLinkInactive + "\">" + DotNetNuke.Services.Localization.Localization.GetString("First", DotNetNuke.Services.Localization.Localization.SharedResourceFile) + "</span>";
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetLastLink returns the Last Page link for paging.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[dancaron]	10/28/2004	Initial Version
        /// </history>
        /// -----------------------------------------------------------------------------
        private string GetLastLink()
        {
            if (CurrentPage != TotalPages && TotalPages > 0)
            {
                return "<a href=\"" + CreateURL(TotalPages.ToString()) + "\" class=\"" + CSSClassLinkActive + "\">" + DotNetNuke.Services.Localization.Localization.GetString("Last", DotNetNuke.Services.Localization.Localization.SharedResourceFile) + "</a>";
            }
            else
            {
                return "<span class=\"" + CSSClassLinkInactive + "\">" + DotNetNuke.Services.Localization.Localization.GetString("Last", DotNetNuke.Services.Localization.Localization.SharedResourceFile) + "</span>";
            }
        }




        public class PageNumLinkTemplate : ITemplate
        {

            static int itemcount = 0;
            private PageControl _PagingControl;

            public PageNumLinkTemplate(PageControl ctlPagingControl)
            {
                _PagingControl = ctlPagingControl;
            }

            void ITemplate.InstantiateIn(Control container)
            {
                Literal l = new Literal();
                l.DataBinding += BindData;
                container.Controls.Add(l);
            }

            private void BindData(object sender, System.EventArgs e)
            {
                Literal lc;
                lc = (Literal)sender;
                RepeaterItem container;
                container = (RepeaterItem)lc.NamingContainer;
                lc.Text ="<li>" + _PagingControl.GetLink(Convert.ToInt32(DataBinder.Eval(container.DataItem, "PageNum"))) + "</li>";
            }

        }

    }
    
}