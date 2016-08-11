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

namespace GcDesign.NewsArticles.Components.WebControls
{
    [ToolboxData("<{0}:PagingControl runat=server></{0}:PagingControl>")]
    public class PagingControl : System.Web.UI.WebControls.WebControl
    {

        protected System.Web.UI.WebControls.Table tablePageNumbers;
        protected System.Web.UI.WebControls.Repeater PageNumbers;
        protected System.Web.UI.WebControls.TableCell cellDisplayStatus;
        protected System.Web.UI.WebControls.TableCell cellDisplayLinks;

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
            tablePageNumbers = new System.Web.UI.WebControls.Table();
            cellDisplayStatus = new System.Web.UI.WebControls.TableCell();
            cellDisplayLinks = new System.Web.UI.WebControls.TableCell();
            cellDisplayStatus.CssClass = "Normal";
            cellDisplayLinks.CssClass = "Normal";

            if (this.CssClass == "")
            {
                tablePageNumbers.CssClass = "PagingTable";
            }
            else
            {
                tablePageNumbers.CssClass = this.CssClass;
            }

            int intRowIndex = tablePageNumbers.Rows.Add(new TableRow());

            PageNumbers = new Repeater();
            PageNumberLinkTemplate I = new PageNumberLinkTemplate(this);
            PageNumbers.ItemTemplate = I;
            BindPageNumbers(TotalRecords, PageSize);

            cellDisplayStatus.HorizontalAlign = HorizontalAlign.Left;
            cellDisplayStatus.Width = new Unit("50%");
            cellDisplayLinks.HorizontalAlign = HorizontalAlign.Right;
            cellDisplayLinks.Width = new Unit("50%");
            int intTotalPages = TotalPages;
            if (intTotalPages == 0)
            {
                intTotalPages = 1;
            }

            string str;
            str = string.Format(DotNetNuke.Services.Localization.Localization.GetString("Pages"), CurrentPage.ToString(), intTotalPages.ToString());
            LiteralControl lit = new LiteralControl(str);
            cellDisplayStatus.Controls.Add(lit);

            tablePageNumbers.Rows[intRowIndex].Cells.Add(cellDisplayStatus);
            tablePageNumbers.Rows[intRowIndex].Cells.Add(cellDisplayLinks);

        }

        protected override void Render(System.Web.UI.HtmlTextWriter output)
        {
            if (PageNumbers == null)
            {
                CreateChildControls();
            }

            System.Text.StringBuilder str = new System.Text.StringBuilder();

            str.Append(GetFirstLink() + "&nbsp;&nbsp;&nbsp;");
            str.Append(GetPreviousLink() + "&nbsp;&nbsp;&nbsp;");
            System.Text.StringBuilder result = new System.Text.StringBuilder(1024);
            PageNumbers.RenderControl(new HtmlTextWriter(new System.IO.StringWriter(result)));
            str.Append(result.ToString());
            str.Append(GetNextLink() + "&nbsp;&nbsp;&nbsp;");
            str.Append(GetLastLink() + "&nbsp;&nbsp;&nbsp;");
            cellDisplayLinks.Controls.Add(new LiteralControl(str.ToString()));

            tablePageNumbers.RenderControl(output);

        }


        private void BindPageNumbers(int TotalRecords, int RecordsPerPage)
        {
            int PageLinksPerPage = 10;
            if (TotalRecords / RecordsPerPage >= 1)
            {
                TotalPages = Convert.ToInt32(Math.Ceiling(((double)TotalRecords / RecordsPerPage)));
            }
            else
            {
                TotalPages = 0;
            }

            if (TotalPages > 0)
            {
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

                if (CurrentPage > (PageLinksPerPage / 2))
                {
                    LowNum = Convert.ToInt32(Math.Floor(tmpNum));
                }

                if (Convert.ToInt32(TotalPages) <= PageLinksPerPage)
                {
                    HighNum = Convert.ToInt32(TotalPages);
                }
                else
                {
                    HighNum = LowNum + PageLinksPerPage - 1;
                }

                if (HighNum > Convert.ToInt32(TotalPages))
                {
                    HighNum = Convert.ToInt32(TotalPages);
                    if (HighNum - LowNum < PageLinksPerPage)
                    {
                        LowNum = HighNum - PageLinksPerPage + 1;
                    }
                }

                if (HighNum > Convert.ToInt32(TotalPages))
                {
                    HighNum = Convert.ToInt32(TotalPages);
                }
                if (LowNum < 1)
                {
                    LowNum = 1;
                }

                int i;
                for (i = LowNum; i <= HighNum; i++)
                {
                    tmpRow = ht.NewRow();
                    tmpRow["PageNum"] = i;
                    ht.Rows.Add(tmpRow);
                }

                PageNumbers.DataSource = ht;
                PageNumbers.DataBind();
            }

        }

        private string CreateURL(string CurrentPage)
        {

            if (QuerystringParams != "")
            {
                if (CurrentPage != "")
                {
                    return Globals.NavigateURL(TabID, "", QuerystringParams, PageParam + "=" + CurrentPage);
                }
                else
                {
                    return Globals.NavigateURL(TabID, "", QuerystringParams);
                }
            }
            else
            {
                if (CurrentPage != "")
                {
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
                return "<a href=\"" + CreateURL((CurrentPage - 1).ToString()) + "\" class=\"" + CSSClassLinkActive + "\">" + DotNetNuke.Services.Localization.Localization.GetString("Previous") + "</a>";
            }
            else
            {
                return "<span class=\"" + CSSClassLinkInactive + "\">" + DotNetNuke.Services.Localization.Localization.GetString("Previous") + "</span>";
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
                return "<a href=\"" + CreateURL((CurrentPage + 1).ToString()) + "\" class=\"" + CSSClassLinkActive + "\">" + DotNetNuke.Services.Localization.Localization.GetString("Next") + "</a>";
            }
            else
            {
                return "<span class=\"" + CSSClassLinkInactive + "\">" + DotNetNuke.Services.Localization.Localization.GetString("Next") + "</span>";
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
                return "<a href=\"" + CreateURL("1") + "\" class=\"" + CSSClassLinkActive + "\">" + DotNetNuke.Services.Localization.Localization.GetString("First") + "</a>";
            }
            else
            {
                return "<span class=\"" + CSSClassLinkInactive + "\">" + DotNetNuke.Services.Localization.Localization.GetString("First") + "</span>";
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
                return "<a href=\"" + CreateURL(TotalPages.ToString()) + "\" class=\"" + CSSClassLinkActive + "\">" + DotNetNuke.Services.Localization.Localization.GetString("Last") + "</a>";
            }
            else
            {
                return "<span class=\"" + CSSClassLinkInactive + "\">" + DotNetNuke.Services.Localization.Localization.GetString("Last") + "</span>";
            }
        }




        public class PageNumberLinkTemplate : ITemplate
        {

            static int itemcount = 0;
            private PagingControl _PagingControl;

            public PageNumberLinkTemplate(PagingControl ctlPagingControl)
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
                lc.Text = _PagingControl.GetLink(Convert.ToInt32(DataBinder.Eval(container.DataItem, "PageNum"))) + "&nbsp;&nbsp;";
            }

        }

    }

}