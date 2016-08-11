using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;

using DotNetNuke.Services.Exceptions;
using DotNetNuke.Security;

using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class ucTemplateEditor : NewsArticleModuleBase
    { 

#region " private Methods "

        private void BindTemplates(string selectedValue){

            drpTemplate.Items.Clear();

            string templateRoot = this.MapPath("Templates");
            if (Directory.Exists(templateRoot)) {
                string[] arrFolders = Directory.GetDirectories(templateRoot);
                foreach( string folder in arrFolders){
                    string folderName = folder.Substring(folder.LastIndexOf(@"\") + 1);
                    ListItem objListItem = new ListItem();
                    objListItem.Text = folderName;
                    objListItem.Value = folderName;
                    drpTemplate.Items.Add(objListItem);
                }
            }

            if  (drpTemplate.Items.FindByValue(ArticleSettings.Template) != null) {
                drpTemplate.SelectedValue = ArticleSettings.Template;
            }

            if (selectedValue != "") {
                if  (drpTemplate.Items.FindByValue(selectedValue) != null) {
                    drpTemplate.SelectedValue = selectedValue;
                }
            }

        }

        private void BindFile(){

            string pathToTemplate = this.MapPath("Templates/" + drpTemplate.SelectedItem.Text + "/");
            string path = pathToTemplate + drpFile.SelectedItem.Text;

            if (!File.Exists(path)) {
                pathToTemplate = this.MapPath("Templates/Standard/");
                path = pathToTemplate + drpFile.SelectedItem.Text;
            }

            if (File.Exists(path)) {
                StreamReader sr = new StreamReader(path);
                try{
                    txtTemplate.Text = sr.ReadToEnd();
                }
                catch{
                
                }
                finally{
                    if ( sr != null) { sr.Close();}
                }
            }
            else
            {
                txtTemplate.Text = "";
            }

        }

#endregion

#region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            drpTemplate.SelectedIndexChanged+=drpTemplate_SelectedIndexChanged;
            drpFile.SelectedIndexChanged+=drpFile_SelectedIndexChanged;
            cmdUpdate.Click+=cmdUpdate_Click;
            cmdCreate.Click+=cmdCreate_Click;
            cmdCancel.Click+=cmdCancel_Click;
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Load(object sender, EventArgs e){

            try{

                if (!this.UserInfo.IsSuperUser) {
                    if (Settings.Contains(ArticleConstants.PERMISSION_SITE_TEMPLATES_SETTING)) {
                        if (Settings[ArticleConstants.PERMISSION_SITE_TEMPLATES_SETTING].ToString() != "") {
                            if (!PortalSecurity.IsInRoles(Settings[ArticleConstants.PERMISSION_SITE_TEMPLATES_SETTING].ToString())) {
                                Response.Redirect(EditArticleUrl("AdminOptions"), true);
                            }
                        }
                        else
                        {
                            Response.Redirect(EditArticleUrl("AdminOptions"), true);
                        }
                    }
                    else
                    {
                        Response.Redirect(EditArticleUrl("AdminOptions"), true);
                    }
                }

                if (!Page.IsPostBack) {
                    BindTemplates("");
                    BindFile();
                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void drpTemplate_SelectedIndexChanged(object sender, EventArgs e){

            try{

                BindFile();

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void drpFile_SelectedIndexChanged(object sender, EventArgs e){

            try{

                BindFile();

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdUpdate_Click(object sender, EventArgs e){

            try{

                string pathToTemplate = this.MapPath("Templates/" + drpTemplate.SelectedItem.Text + "/");
                string path   = pathToTemplate + drpFile.SelectedItem.Text;

                StreamWriter sw = new StreamWriter(path);
                try{
                    sw.Write(txtTemplate.Text);
                }
                catch
                {
                
                }
                finally{
                    if ( sw != null) { sw.Close();}
                    }

                lblUpdated.Visible = true;

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdCreate_Click(object sender, EventArgs e){

            try{

                if (txtNewTemplate.Text != "") {
                    string pathToTemplate= this.MapPath("Templates/" + txtNewTemplate.Text + "/");
                    System.IO.Directory.CreateDirectory(pathToTemplate);

                    if (Directory.Exists(this.MapPath("Templates/Standard/"))) {

                        DirectoryInfo copyDirectory = new DirectoryInfo(this.MapPath("Templates/Standard/"));
                        DirectoryInfo DestDir = new DirectoryInfo(pathToTemplate);


                        foreach(System.IO.FileInfo ChildFile in copyDirectory.GetFiles()){
                            ChildFile.CopyTo(Path.Combine(DestDir.FullName, ChildFile.Name), true);
                    }

                    }

                    pathToTemplate = pathToTemplate + "Images/";
                    System.IO.Directory.CreateDirectory(pathToTemplate);

                    if (Directory.Exists(this.MapPath("Templates/Standard/Images/"))) {

                        DirectoryInfo imagesDirectory = new DirectoryInfo(this.MapPath("Templates/Standard/Images/"));
                        DirectoryInfo DestDir = new DirectoryInfo(pathToTemplate);

                        foreach(System.IO.FileInfo ChildFile in imagesDirectory.GetFiles()){
                            ChildFile.CopyTo(Path.Combine(DestDir.FullName, ChildFile.Name), true);
                        }

                    }


                    lblTemplateCreated.Visible = true;
                    BindTemplates(txtNewTemplate.Text);
                    BindFile();
                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdCancel_Click(object sender, EventArgs e){

            try{

                Response.Redirect(EditArticleUrl("AdminOptions"), true);

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

#endregion
    }
}