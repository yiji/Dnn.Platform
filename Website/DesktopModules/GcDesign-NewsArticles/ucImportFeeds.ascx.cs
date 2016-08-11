using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Scheduling;
using DotNetNuke.UI.UserControls;

using GcDesign.NewsArticles.Import;
using DotNetNuke.Entities.Modules;

using GcDesign.NewsArticles.Base;
using System.Collections;

namespace GcDesign.NewsArticles
{
    public partial class ucImportFeeds : NewsArticleModuleBase
    {

        #region " private Methods "

        private void BindFeeds()
        {

            FeedController objFeedController = new FeedController();

            Localization.LocalizeDataGrid(ref grdFeeds, this.LocalResourceFile);

            grdFeeds.DataSource = objFeedController.GetFeedList(ModuleId, false);
            grdFeeds.DataBind();

            if (grdFeeds.Items.Count > 0)
            {
                grdFeeds.Visible = true;
                lblNoFeeds.Visible = false;
            }
            else
            {
                grdFeeds.Visible = false;
                lblNoFeeds.Visible = true;
                lblNoFeeds.Text = Localization.GetString("NoFeedsMessage.Text", LocalResourceFile);
            }

        }

        private void BindHistory(){

            string typeName = "GcDesign.NewsArticles.Import.RssImportJob, GcDesign.NewsArticles";
            ScheduleItem objSchedule = SchedulingProvider.Instance().GetSchedule(typeName, Null.NullString);

            if (objSchedule != null) {

                ArrayList arrSchedule = SchedulingProvider.Instance().GetScheduleHistory(objSchedule.ScheduleID);

                if (arrSchedule.Count > 0) {

                    arrSchedule.Sort(new ScheduleHistorySortStartDate());

                    //Localize Grid
                    Localization.LocalizeDataGrid(ref dgScheduleHistory, this.LocalResourceFile);

                    dgScheduleHistory.DataSource = arrSchedule;
                    dgScheduleHistory.DataBind();

                    lblNoHistory.Visible = false;
                    dgScheduleHistory.Visible = true;
                }else{
                    lblNoHistory.Visible = true;
                    dgScheduleHistory.Visible = false;
                }

            }else{

                lblNoHistory.Visible = true;
                dgScheduleHistory.Visible = false;

            }

        }

        private void BindSchedulerSettings()
        {

            string typeName = "GcDesign.NewsArticles.Import.RssImportJob, GcDesign.NewsArticles";

            ScheduleItem objSchedule = SchedulingProvider.Instance().GetSchedule(typeName, Null.NullString);

            if (objSchedule != null)
            {
                chkEnabled.Checked = objSchedule.Enabled;

                txtTimeLapse.Text = objSchedule.TimeLapse.ToString();
                if (drpTimeLapseMeasurement.Items.FindByValue(objSchedule.TimeLapseMeasurement) != null)
                {
                    drpTimeLapseMeasurement.SelectedValue = objSchedule.TimeLapseMeasurement;
                }
                else
                {
                    drpTimeLapseMeasurement.SelectedValue = "m";
                }

                txtRetryTimeLapse.Text = objSchedule.RetryTimeLapse.ToString();
                if (drpRetryTimeLapseMeasurement.Items.FindByValue(objSchedule.RetryTimeLapseMeasurement) != null)
                {
                    drpRetryTimeLapseMeasurement.SelectedValue = objSchedule.RetryTimeLapseMeasurement;
                }
                else
                {
                    drpRetryTimeLapseMeasurement.SelectedValue = "m";
                }
            }
            else
            {
                txtTimeLapse.Text = "30";
                drpTimeLapseMeasurement.SelectedValue = "m";

                txtRetryTimeLapse.Text = "60";
                drpRetryTimeLapseMeasurement.SelectedValue = "m";
            }

            ModuleController objModuleController = new ModuleController();
            if (Settings.Contains("NewsArticles-Import-Clear-" + this.ModuleId))
            {
                chkDeleteArticles.Checked = Convert.ToBoolean(Settings["NewsArticles-Import-Clear-" + this.ModuleId].ToString());
            }
            else
            {
                chkDeleteArticles.Checked = false;
            }


        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            cmdUpdate.Click += cmdUpdate_Click;
            cmdCancel.Click += cmdCancel_Click;
            cmdAddFeed.Click+=cmdAddFeed_Click;
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Load(object sender, EventArgs e)
        {

            try
            {

                phScheduler.Visible = this.UserInfo.IsSuperUser;


                if (!IsPostBack)
                {

                    BindFeeds();

                    if (phScheduler.Visible)
                    {
                        BindSchedulerSettings();
                        BindHistory();
                    }
                    else
                    {
                        string typeName = "GcDesign.NewsArticles.Import.RssImportJob, GcDesign.NewsArticles";
                        ScheduleItem objSchedule = SchedulingProvider.Instance().GetSchedule(typeName, Null.NullString);

                        if (objSchedule != null)
                        {
                            if (objSchedule.Enabled)
                            {
                                lblScheduler.Visible = false;
                            }
                            else
                            {
                                lblScheduler.Text = Localization.GetString("SchedulerNotEnabled", this.LocalResourceFile);
                                lblScheduler.Visible = true;
                            }
                        }
                        else
                        {
                            lblScheduler.Text = Localization.GetString("SchedulerNotEnabled", this.LocalResourceFile);
                            lblScheduler.Visible = true;
                        }
                    }

                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }


        protected void cmdAddFeed_Click(object sender, EventArgs e)
        {

            try
            {

                Response.Redirect(EditUrl("ImportFeed"), true);

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void cmdUpdate_Click(object sender, EventArgs e){

            try{

                string typeName = "GcDesign.NewsArticles.Import.RssImportJob, GcDesign.NewsArticles";

                ScheduleItem objSchedule = SchedulingProvider.Instance().GetSchedule(typeName, Null.NullString);

                if (objSchedule != null) {

                    objSchedule.Enabled = chkEnabled.Checked;

                    if (Numeric.IsNumeric(txtTimeLapse.Text)) {
                        objSchedule.TimeLapse = Convert.ToInt32(txtTimeLapse.Text);
                    }else{
                        objSchedule.TimeLapse = 30;
                    }
                    objSchedule.TimeLapseMeasurement = drpTimeLapseMeasurement.SelectedValue;

                    if (Numeric.IsNumeric(txtTimeLapse.Text)) {
                        objSchedule.RetryTimeLapse = Convert.ToInt32(txtRetryTimeLapse.Text);
                    }else{
                        objSchedule.RetryTimeLapse = 60;
                    }
                    objSchedule.RetryTimeLapseMeasurement = drpRetryTimeLapseMeasurement.SelectedValue;

                    SchedulingProvider.Instance().UpdateSchedule(objSchedule);

                }else{

                    objSchedule = new ScheduleItem();

                    objSchedule.TypeFullName = typeName;
                    objSchedule.Enabled = chkEnabled.Checked;

                    if (Numeric.IsNumeric(txtTimeLapse.Text)) {
                        objSchedule.TimeLapse = Convert.ToInt32(txtTimeLapse.Text);
                    }else{
                        objSchedule.TimeLapse = 30;
                    }
                    objSchedule.TimeLapseMeasurement = drpTimeLapseMeasurement.SelectedValue;

                    if (Numeric.IsNumeric(txtTimeLapse.Text)) {
                        objSchedule.RetryTimeLapse = Convert.ToInt32(txtRetryTimeLapse.Text);
                    }else{
                        objSchedule.RetryTimeLapse = 60;
                    }
                    objSchedule.RetryTimeLapseMeasurement = drpRetryTimeLapseMeasurement.SelectedValue;

                    objSchedule.RetainHistoryNum = 10;
                    objSchedule.AttachToEvent = "";
                    objSchedule.CatchUpEnabled = false;
                    objSchedule.Enabled = chkEnabled.Checked;
                    objSchedule.ObjectDependencies = "";
                    objSchedule.Servers = "";

                    objSchedule.ScheduleID = SchedulingProvider.Instance().AddSchedule(objSchedule);

                }

                ModuleController objModuleController = new ModuleController();
                objModuleController.UpdateModuleSetting(this.ModuleId, "NewsArticles-Import-Clear-" + this.ModuleId, chkDeleteArticles.Checked.ToString());
                SchedulingProvider.Instance().AddScheduleItemSetting(objSchedule.ScheduleID, "NewsArticles-Import-Clear-" + this.ModuleId, chkDeleteArticles.Checked.ToString());

                Response.Redirect(EditUrl("AdminOptions"), true);

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {

            try
            {

                Response.Redirect(EditUrl("AdminOptions"), true);

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}