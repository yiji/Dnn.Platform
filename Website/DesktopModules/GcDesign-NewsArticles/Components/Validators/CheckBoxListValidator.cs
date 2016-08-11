using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace GcDesign.NewsArticles.Components.Validators
{
    public class CheckBoxListValidator : BaseValidator
    {
        [Description("The minimum number of CheckBoxes that must be checked to be considered valid.")]
        public int MinimumNumberOfSelectedCheckBoxes
        {
            get
            {
                object o = ViewState["MinimumNumberOfSelectedCheckBoxes"];
                if (o == null)
                {
                    return 1;
                }
                else
                {
                    return Convert.ToInt32(o);
                }
            }
            set
            {
                ViewState["MinimumNumberOfSelectedCheckBoxes"] = value;
            }
        }

        private CheckBoxList _ctrlToValidate = null;
        protected  CheckBoxList CheckBoxListToValidate
        {
            get
            {
                if (_ctrlToValidate == null)
                {
                    _ctrlToValidate = FindControl(this.ControlToValidate) as CheckBoxList;
                }

                return _ctrlToValidate;
            }
        }

        protected override bool ControlPropertiesValid()
        {
            // Make sure ControlToValidate is set
            if (this.ControlToValidate.Length == 0)
            {
                throw new HttpException(String.Format("The ControlToValidate property of '{0}' cannot be blank.", this.ID));
            }

            // Ensure that the control being validated is a CheckBoxList
            if (CheckBoxListToValidate == null)
            {
                throw new HttpException(String.Format("The CheckBoxListValidator can only validate controls of type CheckBoxList."));
            }

            // ... and that it has at least MinimumNumberOfSelectedCheckBoxes ListItems
            //If CheckBoxListToValidate.Items.Count < MinimumNumberOfSelectedCheckBoxes Then
            //    Throw New HttpException(String.Format("MinimumNumberOfSelectedCheckBoxes must be set to a value greater than or equal to the number of ListItems; MinimumNumberOfSelectedCheckBoxes is set to {0}, but there are only {1} ListItems in '{2}'", MinimumNumberOfSelectedCheckBoxes, CheckBoxListToValidate.Items.Count, CheckBoxListToValidate.ID))
            //End If

            return true;
            //if we reach here, everything checks out
        }

        protected override bool EvaluateIsValid()
        {
            //Make sure that the CheckBoxList has at least MinimumNumberOfSelectedCheckBoxes ListItems selected
            int selectedItemCount = 0;
            foreach (ListItem cb in CheckBoxListToValidate.Items)
            {
                if (cb.Selected)
                {
                    selectedItemCount += 1;
                }
            }

            return selectedItemCount >= MinimumNumberOfSelectedCheckBoxes;
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);

            //Add the client-side code (if needed)
            if (this.RenderUplevel)
            {
                // Indicate the mustBeChecked value and the client-side function to used for evaluation
                // Use AddAttribute if Helpers.EnableLegacyRendering is true; otherwise, use expando attributes
                if (EnableLegacyRendering())
                {
                    writer.AddAttribute("evaluationfunction", "CheckBoxListValidatorEvaluateIsValid", false);
                    writer.AddAttribute("minimumNumberOfSelectedCheckBoxes", MinimumNumberOfSelectedCheckBoxes.ToString(), false);
                }
                else
                {
                    this.Page.ClientScript.RegisterExpandoAttribute(this.ClientID, "evaluationfunction", "CheckBoxListValidatorEvaluateIsValid", false);
                    this.Page.ClientScript.RegisterExpandoAttribute(this.ClientID, "minimumNumberOfSelectedCheckBoxes", MinimumNumberOfSelectedCheckBoxes.ToString(), false);
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // Register the client-side function using WebResource.axd (if needed)
            // see: http://aspnet.4guysfromrolla.com/articles/080906-1.aspx
            if (this.RenderUplevel && this.Page != null && !this.Page.ClientScript.IsClientScriptIncludeRegistered(this.GetType(), "VentrianValidators"))
            {
                this.Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "VentrianValidators", this.Page.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Includes/VentrianValidators.js"));
            }
        }

        private bool EnableLegacyRendering()
        {
            bool result;

            try
            {
                string webConfigFile = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "web.config");
                XmlTextReader webConfigReader = new XmlTextReader(new StreamReader(webConfigFile));
                result = ((webConfigReader.ReadToFollowing("xhtmlConformance")) && (webConfigReader.GetAttribute("mode") == "Legacy"));
                webConfigReader.Close();
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}