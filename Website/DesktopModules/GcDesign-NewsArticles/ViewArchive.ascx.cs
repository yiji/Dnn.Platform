using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class ViewArchive : NewsArticleModuleBase
    {


        #region " Constants "

        private const string PARAM_YEAR = "Year";
        private const string PARAM_MONTH = "Month";

        #endregion

        #region " private Members "

        private int _year = Null.NullInteger;
        private int _month = Null.NullInteger;

        #endregion

        #region " private Methods "

        private void BindArchiveName()
        {

            if (_month == Null.NullInteger && _year == Null.NullInteger)
            {
                // No archive to view. 
                Response.Redirect(Globals.NavigateURL(), true);
            }

            if (_year == Null.NullInteger)
            {
                // No archive to view. 
                Response.Redirect(Globals.NavigateURL(), true);
            }

            if (_month != Null.NullInteger)
            {
                string entriesFrom = Localization.GetString("MonthYearEntries", LocalResourceFile);
                DateTime objDate = new DateTime(_year, _month, 1);
                if (entriesFrom.Contains("{0}") && entriesFrom.Contains("{1}"))
                {
                    lblArchive.Text = String.Format(entriesFrom, objDate.ToString("MMMM"), objDate.ToString("yyyy"));
                }
                else
                {
                    if (entriesFrom.Contains("{0}"))
                    {
                        lblArchive.Text = String.Format(entriesFrom, objDate.ToString("MMMM"));
                    }
                    else
                    {
                        lblArchive.Text = objDate.ToString("MMMM yyyy");
                    }
                }
                this.BasePage.Title = objDate.ToString("MMMM yyyy") + " " + Localization.GetString("Archive", this.LocalResourceFile) + " | " + this.BasePage.Title;

            }
            else
            {
                string entriesFrom = Localization.GetString("YearEntries", LocalResourceFile);
                if (entriesFrom.Contains("{0}"))
                {
                    lblArchive.Text = String.Format(entriesFrom, _year.ToString());
                }
                else
                {
                    lblArchive.Text = _year.ToString();
                }

                this.BasePage.Title = _year.ToString() + " " + Localization.GetString("Archive", this.LocalResourceFile) + " | " + this.BasePage.Title;
            }

        }

        private void ReadQueryString()
        {

            if (Request[PARAM_YEAR] != "" && Numeric.IsNumeric(Request[PARAM_YEAR]))
            {
                _year = Convert.ToInt32(Request[PARAM_YEAR]);
            }

            if (Request[PARAM_MONTH] != "" && Numeric.IsNumeric(Request[PARAM_MONTH]))
            {
                _month = Convert.ToInt32(Request[PARAM_MONTH]);
            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Init += new EventHandler(this.Page_Init);
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Init(object sender, EventArgs e)
        {

            try
            {

                ReadQueryString();
                BindArchiveName();

                Listing1.Month = _month;
                Listing1.Year = _year;
                Listing1.ShowExpired = true;
                Listing1.MaxArticles = Null.NullInteger;
                Listing1.IsIndexed = false;

                Listing1.BindListing();

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}