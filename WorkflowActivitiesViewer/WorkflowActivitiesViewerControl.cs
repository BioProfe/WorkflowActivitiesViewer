using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using System.Net;
using System.Xml;

namespace WorkflowActivitiesViewer
{
    public partial class WorkflowActivitiesViewerControl : PluginControlBase
    {
        private Settings mySettings;

        #region Variables and Const

        Color assemblyColor = Color.DarkSlateGray;
        Color codeActivityColor = Color.DarkBlue;
        Color workflowColor = Color.Black;


        const int LINES_SPACE = 25;
        const int MARGIN_LEFT = 10;
        const int MARGIN_TOP = 30;

        IOrganizationService service = null;
        private List<WorkflowData> workflowList = null;
        private List<AssemblyData> assemblyList = null;

        #endregion Variables and Const

        #region Constructor

        public WorkflowActivitiesViewerControl()
        {
            InitializeComponent();
        }

        private void WorkflowActivitiesViewerControl_Load(object sender, EventArgs e)
        {
            ShowInfoNotification("This is a notification that can lead to XrmToolBox repository", new Uri("https://github.com/MscrmTools/XrmToolBox"));

            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }

            txtSearchCodeActivity.Text = string.Empty;
            txtSearchCodeActivity.TextChanged += TxtSearchCodeActivity_TextChanged;
        }

        private void FillAssemblies()
        {
            if (assemblyList == null)
                assemblyList = new List<AssemblyData>();
            else
                assemblyList.Clear();

            String consultaFetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                          <entity name='plugintype'>
                                            <attribute name='plugintypeid' />
                                            <attribute name='name' />
                                            <attribute name='typename' />
                                            <attribute name='createdon' />
                                            <attribute name='modifiedon' />
                                            <attribute name='createdby' />
                                            <attribute name='modifiedby' />
                                            <attribute name='version' />
                                            <attribute name='pluginassemblyid' />
                                            <attribute name='customworkflowactivityinfo' />
                                            <attribute name='assemblyname' />
                                            <order attribute='assemblyname' descending='true' />
                                            <filter type='and'>
                                              <condition attribute='isworkflowactivity' operator='eq' value='1' />
                                            </filter>
                                            <link-entity name='pluginassembly' from='pluginassemblyid' to='pluginassemblyid' link-type='inner' alias='al'>
                                              <filter type='and'>
                                                <condition attribute='sourcetype' operator='eq' value='0' />
                                              </filter>
                                            </link-entity>
                                          </entity>
                                        </fetch>
                                        ";

            EntityCollection resultado = service.RetrieveMultiple(new FetchExpression(consultaFetch));

            if (resultado != null)
            {
                foreach (Entity entidad in resultado.Entities)
                {
                    if (entidad.Contains("assemblyname") && entidad.Attributes["assemblyname"] != null)
                    {
                        string pluginassemblyid = entidad.Attributes["pluginassemblyid"].ToString();
                        string assemblyname = entidad.Attributes["assemblyname"].ToString();

                        CodeActivityData codeActivity = new CodeActivityData();

                        codeActivity.codeActivityid = entidad.Attributes["plugintypeid"].ToString().ToLower();
                        codeActivity.name = entidad.Attributes["name"].ToString();
                        codeActivity.typename = entidad.Attributes["typename"].ToString();
                        codeActivity.createdon = (DateTime)entidad.Attributes["createdon"];
                        codeActivity.modifiedon = (DateTime)entidad.Attributes["modifiedon"];
                        codeActivity.createdby = ((EntityReference)entidad.Attributes["createdby"]).Name;
                        codeActivity.modifiedby = ((EntityReference)entidad.Attributes["modifiedby"]).Name;
                        codeActivity.version = entidad.Attributes["version"].ToString();

                        if(entidad.Attributes.Contains("customworkflowactivityinfo") && entidad.Attributes["customworkflowactivityinfo"] != null)
                            codeActivity.customworkflowactivityinfo = entidad.Attributes["customworkflowactivityinfo"].ToString();

                        AssemblyData assemblyData_finded = GetAssemblyDataByName(assemblyname);

                        if (assemblyData_finded == null)
                        {
                            AssemblyData assemblyData_new = new AssemblyData(-1, assemblyname, pluginassemblyid);
                            assemblyData_new.codeActivityList = new List<CodeActivityData>();
                            assemblyData_new.codeActivityList.Add(codeActivity);
                            assemblyList.Add(assemblyData_new);
                        }
                        else
                        {
                            assemblyData_finded.codeActivityList.Add(codeActivity);
                        }
                    }
                }

                // Ordenar las entidades por nombre
                GFG gg = new GFG();
                assemblyList.Sort(gg);
            }
        }

        private void FillWorkflows()
        {
            if (workflowList == null)
                workflowList = new List<WorkflowData>();
            else
                workflowList.Clear();

            String consultaFetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                              <entity name='workflow'>
                                                <attribute name='workflowid' />
                                                <attribute name='primaryentity' />
                                                <attribute name='name' />
                                                <attribute name='createdon' />
                                                <attribute name='modifiedon' />
                                                <attribute name='type' />
                                                <attribute name='ondemand' />
                                                <attribute name='triggeroncreate' />
                                                <attribute name='triggeronupdateattributelist' />
                                                <attribute name='triggerondelete' />
                                                <attribute name='subprocess' />
                                                <attribute name='xaml' />
                                                <order attribute='name' descending='false' />
                                                <filter type='and'>
                                                  <condition attribute='type' operator='eq' value='1' />
                                                  <condition attribute='ownerid' operator='not-null' />
                                                  <condition attribute='rendererobjecttypecode' operator='null' />
                                                  <condition attribute='category' operator='eq' value='0' />
                                                  <condition attribute='statecode' operator='eq' value='1' />
                                                  <condition attribute='primaryentity' operator='not-null' />
                                                </filter>
                                              </entity>
                                            </fetch>";

            EntityCollection resultado = service.RetrieveMultiple(new FetchExpression(consultaFetch));

            if (resultado != null)
            {
                foreach (Entity entidad in resultado.Entities)
                {
                    if (entidad.Contains("primaryentity") && entidad.Attributes["primaryentity"] != null)
                    {
                        string primaryentity = entidad.Attributes["primaryentity"].ToString();

                        WorkflowData workflow = new WorkflowData();

                        workflow.workflowid = entidad.Attributes["workflowid"].ToString().ToLower();
                        workflow.name = entidad.Attributes["name"].ToString();
                        workflow.entityName = primaryentity;
                        workflow.createdon = (DateTime)entidad.Attributes["createdon"];
                        workflow.modifiedon = (DateTime)entidad.Attributes["modifiedon"];
                        workflow.ondemand = (bool)entidad.Attributes["ondemand"];
                        workflow.triggeroncreate = (bool)entidad.Attributes["triggeroncreate"];
                        workflow.triggerondelete = (bool)entidad.Attributes["triggerondelete"];

                        if (entidad.Contains("triggeronupdateattributelist") && entidad.Attributes["triggeronupdateattributelist"] != null)
                        {
                            workflow.triggeronupdateattributelist = entidad.Attributes["triggeronupdateattributelist"].ToString();
                        }
                        workflow.subprocess = (bool)entidad.Attributes["subprocess"];
                        workflow.xaml = entidad.Attributes["xaml"].ToString().ToLower();

                        workflowList.Add(workflow);

                    }
                }
            }
        }

        #endregion Constructor

        #region Plugin events

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }


        private void tsbRefresh_Click(object sender, EventArgs e)
        {
            txtSearchCodeActivity.Text = string.Empty;
            FillAssemblies();
            FillWorkflows();

            ShowAssemblies(VISIBILITY_TYPES.COLLAPSED);
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkflowActivitiesViewerControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);
            service = newService;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            txtSearchCodeActivity.Text = string.Empty;
            FillAssemblies();
            FillWorkflows();

            ShowAssemblies(VISIBILITY_TYPES.COLLAPSED);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
        }

        #endregion Plugin events


        #region Get functions

        private AssemblyData GetAssemblyDataByName(string name)
        {
            AssemblyData assemblyData_finded = null;

            foreach (AssemblyData assemblyData in assemblyList)
            {
                if (assemblyData.Name.Equals(name))
                {
                    assemblyData_finded = assemblyData;
                    break;
                }
            }

            return assemblyData_finded;
        }

        private AssemblyData GetAssemblyDataByWorkflowID(string codeactivityid)
        {
            AssemblyData assemblySelected = null;

            foreach (AssemblyData assemblyData in assemblyList)
            {
                if (assemblyData.codeActivityList != null)
                {
                    foreach (CodeActivityData codeActivity in assemblyData.codeActivityList)
                    {
                        if (codeActivity.codeActivityid.Equals(codeactivityid))
                        {
                            assemblySelected = assemblyData;
                            break;
                        }
                    }
                    if (assemblySelected != null)
                        break;
                }
            }

            return assemblySelected;
        }

        private CodeActivityData GetCodeActivityDataByID(string codeactivityid)
        {
            CodeActivityData assemblySelected = null;

            foreach (AssemblyData assemblyData in assemblyList)
            {
                if (assemblyData.codeActivityList != null)
                {
                    foreach (CodeActivityData codeActivity in assemblyData.codeActivityList)
                    {
                        if (codeActivity.codeActivityid.Equals(codeactivityid))
                        {
                            assemblySelected = codeActivity;
                            break;
                        }
                    }
                    if (assemblySelected != null)
                        break;
                }
            }

            return assemblySelected;
        }

        private Panel GetWorkflowBox(string workflowid)
        {
            Panel workflowbox = null;

            foreach (var control in panelProcessInfo.Controls)
            {
                if (control is Panel)
                {
                    Panel controlPanel = control as Panel;
                    if (controlPanel.Name.Contains(workflowid))
                    {
                        workflowbox = controlPanel;
                        break;
                    }
                }
            }

            return workflowbox;
        }

        private Panel GetCodeActivityBox(string codeactivityid)
        {
            Panel codeactivity = null;

            foreach (var control in panelProcessInfo.Controls)
            {
                if (control is Panel)
                {
                    Panel controlPanel = control as Panel;
                    if (controlPanel.Name.Contains(codeactivityid))
                    {
                        codeactivity = controlPanel;
                        break;
                    }
                }
            }

            return codeactivity;
        }

        #endregion Get functions

        #region Front end

        private void ShowCodeActivityInfo(CodeActivityData codeActivitySelected)
        {
            int y = MARGIN_TOP;

            panelProcessInfo.Controls.Clear();

            #region Code Activity data

            System.Windows.Forms.Label txtCodeActivityName = new System.Windows.Forms.Label();
            txtCodeActivityName.Text = codeActivitySelected.name;
            txtCodeActivityName.Font = new Font("Verdana", 10, FontStyle.Bold);
            txtCodeActivityName.AutoSize = true;
            txtCodeActivityName.Location = new Point(MARGIN_LEFT, y);
            txtCodeActivityName.ForeColor = codeActivityColor;

            panelProcessInfo.Controls.Add(txtCodeActivityName);

            y += LINES_SPACE;
            y += LINES_SPACE;
            System.Windows.Forms.Label txtCodeActivityCreatedOn = new System.Windows.Forms.Label();
            txtCodeActivityCreatedOn.Text = "Created on " + codeActivitySelected.createdon.ToString() + " by " + codeActivitySelected.createdby.ToString();
            txtCodeActivityCreatedOn.Font = new Font("Verdana", 9, FontStyle.Regular);
            txtCodeActivityCreatedOn.AutoSize = true;
            txtCodeActivityCreatedOn.Location = new Point(MARGIN_LEFT, y);
            txtCodeActivityCreatedOn.ForeColor = codeActivityColor;
            panelProcessInfo.Controls.Add(txtCodeActivityCreatedOn);

            y += LINES_SPACE;
            System.Windows.Forms.Label txtCodeActivityModifiedOn = new System.Windows.Forms.Label();
            txtCodeActivityModifiedOn.Text = "Modified on " + codeActivitySelected.modifiedon.ToString() + " by " + codeActivitySelected.modifiedby.ToString();
            txtCodeActivityModifiedOn.Font = new Font("Verdana", 9, FontStyle.Regular);
            txtCodeActivityModifiedOn.AutoSize = true;
            txtCodeActivityModifiedOn.Location = new Point(MARGIN_LEFT, y);
            txtCodeActivityModifiedOn.ForeColor = codeActivityColor;
            panelProcessInfo.Controls.Add(txtCodeActivityModifiedOn);

            #endregion Code Activity data

            #region Fill the Argument list

            if(codeActivitySelected.inArgumentList == null)
            {
                XmlDocument xDoc = new XmlDocument();

                codeActivitySelected.inArgumentList = new List<string>();
                try
                {
                    xDoc.LoadXml(codeActivitySelected.customworkflowactivityinfo);

                    // Inputs
                    XmlNodeList Inputs = xDoc.GetElementsByTagName("Inputs");

                    foreach (XmlElement node in Inputs)
                    {
                        XmlNodeList xLista = node.GetElementsByTagName("Name");

                        foreach (XmlElement nodo in xLista)
                        {
                            codeActivitySelected.inArgumentList.Add(nodo.InnerText);
                        }
                    }
                }
                catch { }

                codeActivitySelected.outArgumentList = new List<string>();
                try
                { 
                // Outputs
                XmlNodeList Outputs = xDoc.GetElementsByTagName("Outputs");

                    foreach (XmlElement node in Outputs)
                    {
                        XmlNodeList xLista = node.GetElementsByTagName("Name");

                        foreach (XmlElement nodo in xLista)
                        {
                            codeActivitySelected.outArgumentList.Add(nodo.InnerText);
                        }
                    }
                }
                catch { }
            }

            #endregion Fill the Argument list

            #region Write the Argument list

            if (codeActivitySelected.inArgumentList != null && codeActivitySelected.outArgumentList != null)
            {
                if (codeActivitySelected.inArgumentList.Count > 0)
                {
                    // Input params
                    StringBuilder inputParams = new StringBuilder();

                    inputParams.AppendLine("Input arguments:");

                    foreach (string argument in codeActivitySelected.inArgumentList)
                    {
                        string argumentLine = "     • " + argument;
                        inputParams.AppendLine(argumentLine);
                    }


                    y += LINES_SPACE;
                    System.Windows.Forms.Label txtInputParams = new System.Windows.Forms.Label();
                    txtInputParams.Text = inputParams.ToString();
                    txtInputParams.Font = new Font("Verdana", 9, FontStyle.Regular);
                    txtInputParams.AutoSize = true;
                    txtInputParams.Margin = new Padding(20, 0, 10, 0);
                    txtInputParams.Location = new Point(MARGIN_LEFT, y);
                    txtInputParams.ForeColor = codeActivityColor;
                    panelProcessInfo.Controls.Add(txtInputParams);

                    y += txtInputParams.Height;
                }

                if (codeActivitySelected.inArgumentList.Count > 0)
                {
                    // Output params
                    StringBuilder outputParams = new StringBuilder();

                    outputParams.AppendLine("Output arguments:");

                    foreach (string argument in codeActivitySelected.outArgumentList)
                    {
                        string argumentLine = "     • " + argument;
                        outputParams.AppendLine(argumentLine);
                    }

                    y += LINES_SPACE;
                    System.Windows.Forms.Label txtOutputParams = new System.Windows.Forms.Label();
                    txtOutputParams.Text = outputParams.ToString();
                    txtOutputParams.Font = new Font("Verdana", 9, FontStyle.Regular);
                    txtOutputParams.AutoSize = true;
                    txtOutputParams.Margin = new Padding(20, 0, 10, 0);
                    txtOutputParams.Location = new Point(MARGIN_LEFT, y);
                    txtOutputParams.ForeColor = codeActivityColor;
                    panelProcessInfo.Controls.Add(txtOutputParams);

                    y += txtOutputParams.Height;
                }
            }

            #endregion Write the Argument list

            #region Fill the Workflow list

            if (codeActivitySelected.workflowsList == null)
            {
                foreach (WorkflowData workflow in workflowList)
                {
                    if (workflow.xaml.Contains(codeActivitySelected.name.ToLower()))
                    {
                        if (codeActivitySelected.workflowsList == null)
                            codeActivitySelected.workflowsList = new List<WorkflowData>();
                        codeActivitySelected.workflowsList.Add(workflow);
                    }
                }
            }

            #endregion Fill the Workflow list

            #region Write the workflows info

            y += LINES_SPACE;

            if (codeActivitySelected.workflowsList != null && codeActivitySelected.workflowsList.Count > 0)
            {
                System.Windows.Forms.Label txtDependences = new System.Windows.Forms.Label();
                txtDependences.Text = "Dependences:";
                txtDependences.Font = new Font("Verdana", 9, FontStyle.Underline);
                txtDependences.AutoSize = true;
                txtDependences.Location = new Point(MARGIN_LEFT, y);
                txtDependences.ForeColor = workflowColor;
                panelProcessInfo.Controls.Add(txtDependences);

                y += LINES_SPACE;

                foreach (var workflow in codeActivitySelected.workflowsList)
                {
                    Panel workflowBox = workflow.CreateWorkflowBox(MARGIN_LEFT * 2, y);
                    workflowBox.Click += WorkflowBox_Click;
                    foreach (var child in workflowBox.Controls)
                    {
                        if (child is TextBox)
                            ((TextBox)child).Click += WorkflowBox_Click;
                    }

                    panelProcessInfo.Controls.Add(workflowBox);
                    y += workflowBox.Height + LINES_SPACE;
                }

            }
            else
            {
                System.Windows.Forms.Label txtDependences = new System.Windows.Forms.Label();
                txtDependences.Text = "There are no dependencies";
                txtDependences.Font = new Font("Verdana", 9, FontStyle.Underline);
                txtDependences.AutoSize = true;
                txtDependences.Location = new Point(MARGIN_LEFT, y);
                txtDependences.ForeColor = workflowColor;
                panelProcessInfo.Controls.Add(txtDependences);
            }
            

            #endregion  Write the workflows info


            #region leave more space in the right side and below

            int x = 200;

            y += LINES_SPACE;
            y += LINES_SPACE;

            System.Windows.Forms.Label txtSpaceBelow = new System.Windows.Forms.Label();
            txtSpaceBelow.Text = ".";
            txtSpaceBelow.Font = new Font("Verdana", 8, FontStyle.Regular);
            txtSpaceBelow.AutoSize = true;
            txtSpaceBelow.Margin = new Padding(10, 0, 10, 0);

            txtSpaceBelow.Location = new Point(x, y);
            txtSpaceBelow.ForeColor = Color.White;

            panelProcessInfo.Controls.Add(txtSpaceBelow);
            #endregion leave more space below

        }

        private void ShowAssemblies(VISIBILITY_TYPES visibility)
        {
            panelAssemblies.Controls.Clear();
            panelProcessInfo.Controls.Clear();

            int y = 5;
            int index = 0;

            foreach (AssemblyData assembly in assemblyList)
            {
                if (visibility == VISIBILITY_TYPES.COLLAPSED)
                    assembly.IsExpanded = false;
                else if (visibility == VISIBILITY_TYPES.EXPANDED)
                    assembly.IsExpanded = true;

                bool isAssemblyNameWritting = false;
                foreach (CodeActivityData codeActivity in assembly.codeActivityList)
                {
                    if (string.IsNullOrWhiteSpace(txtSearchCodeActivity.Text) || codeActivity.name.ToUpper().Contains(txtSearchCodeActivity.Text.ToUpper()))
                    {
                        // chechk if the assembly name has been written
                        if (!isAssemblyNameWritting)
                        {
                            assembly.Index = index;
                            LinkLabel linkEntityLabel = new LinkLabel();
                            linkEntityLabel.Name = "linkEntity_" + index.ToString();
                            // If the assembly menu is expanded, show "-" before the name
                            if (assembly.IsExpanded)
                                linkEntityLabel.Text = "- " + assembly.Name;
                            else // If the assembly menu is not expanded, show "+" before the name
                                linkEntityLabel.Text = "+ " + assembly.Name;
                            linkEntityLabel.AutoSize = true;
                            linkEntityLabel.Font = new Font("Verdana", 9, FontStyle.Regular);
                            linkEntityLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                            linkEntityLabel.ForeColor = assemblyColor;
                            linkEntityLabel.LinkColor = assemblyColor;
                            linkEntityLabel.Margin = new Padding(10, 5, 10, 5);
                            linkEntityLabel.Location = new Point(10, y);
                            linkEntityLabel.Click += LinkAssemblyLabel_Click;
                            panelAssemblies.Controls.Add(linkEntityLabel);

                            y += LINES_SPACE;

                            isAssemblyNameWritting = true;
                            index++;
                        }

                        // If the assembly menu is expanded, show the code activities inside
                        if (assembly.IsExpanded)
                        {
                            ShowCodeActivity(codeActivity, 30, y);
                            y += LINES_SPACE;
                        }
                    }
                }
            }
        }

        private void ShowCodeActivity(CodeActivityData codeActivity, int x, int y)
        {
            LinkLabel lblCodeActivity = new LinkLabel();
            lblCodeActivity.Name = "id_" + codeActivity.codeActivityid;
            lblCodeActivity.Font = new Font("Verdana", 9, FontStyle.Regular);
            lblCodeActivity.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            lblCodeActivity.AutoSize = true;
            lblCodeActivity.Margin = new Padding(10, 5, 10, 5);
            lblCodeActivity.Location = new Point(x, y);
            lblCodeActivity.ForeColor = codeActivityColor;
            lblCodeActivity.LinkColor = codeActivityColor;

            lblCodeActivity.Click += TxtCodeActivity_Click;

            lblCodeActivity.Text = codeActivity.name;

            panelAssemblies.Controls.Add(lblCodeActivity);
        }

        #endregion Front end

        #region Link Events

        private void LinkAssemblyLabel_Click(object sender, EventArgs e)
        {
            String outputMessage = string.Empty;

            LinkLabel linkLabel = sender as LinkLabel;

            int indexPosition = linkLabel.Name.IndexOf("_");

            int selectedIndex = Convert.ToInt32(linkLabel.Name.Substring(indexPosition + 1));

            AssemblyData assemblySelected = null;
            foreach (AssemblyData entityData in assemblyList)
            {
                if (selectedIndex == entityData.Index)
                {
                    assemblySelected = entityData;
                    break;
                }
            }

            if (assemblySelected == null)
            {
                outputMessage = "The selected assembly does not exist";
                txtSearchCodeActivity.Text = string.Empty;
                txtSearchCodeActivity.Enabled = false;
            }
            else
            {
                txtSearchCodeActivity.Enabled = true;
                txtSearchCodeActivity.Text = string.Empty;
                try
                {
                    if (assemblySelected.codeActivityList == null)
                    {
                        outputMessage = "Assembly with no code activities";
                    }
                    else
                    {
                        assemblySelected.IsExpanded = !assemblySelected.IsExpanded;
                        ShowAssemblies(VISIBILITY_TYPES.SOME);
                    }
                }
                catch (Exception ex)
                {
                    outputMessage = "Error: " + ex.Message;
                }

                

            }

            if (!string.IsNullOrWhiteSpace(outputMessage))
            {
                MessageBox.Show(outputMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                /*PopupWindow popup = new PopupWindow();
                popup.TxtContent.Text = outputMessage;
                popup.Location = listWorkflows.Location;

                popup.ShowDialog();

                popup.Dispose();*/
            }
        }

        private void TxtCodeActivity_Click(object sender, EventArgs e)
        {
            if (assemblyList == null)
                return;

            LinkLabel lblCodeActivity = sender as LinkLabel;
            string codeactivityid = lblCodeActivity.Name.Substring(3);
            AssemblyData assemblySelected = GetAssemblyDataByWorkflowID(codeactivityid);
            CodeActivityData codeactivitySelected = GetCodeActivityDataByID(codeactivityid);

            ShowCodeActivityInfo(codeactivitySelected);
        }

        private void WorkflowBox_Click(object sender, EventArgs e)
        {
            if (assemblyList == null)
                return;

            string workflowid = string.Empty;

            if (sender is TextBox)
            {
                TextBox workflowBox = sender as TextBox;
                workflowid = workflowBox.Name.Substring(3);
            }
            else if (sender is Panel)
            {
                Panel workflowBox = sender as Panel;
                workflowid = workflowBox.Name.Substring(3);
            }

            /*EntityData entitySelected = GetAssemblyDataByWorkflowID(workflowid);
            WorkflowData workflowSelected = GetCodeActivityDataByID(workflowid);

            ShowProcessTree(workflowSelected);*/
        }

        #endregion Link Events

        #region Search events

        private void TxtSearchCodeActivity_TextChanged(object sender, EventArgs e)
        {
            if (assemblyList == null)
                FillAssemblies();

            if(string.IsNullOrWhiteSpace(txtSearchCodeActivity.Text))
                ShowAssemblies(VISIBILITY_TYPES.COLLAPSED);
            else
                ShowAssemblies(VISIBILITY_TYPES.EXPANDED);
        }

        #endregion Search events
    }

    #region clases auxiliares

    public class AssemblyData
    {
        public int Index { get; set; }

        public string Name { get; set; }

        public string AssemblyId { get; set; }
        public List<CodeActivityData> codeActivityList { get; set; }

        public bool IsExpanded { get; set; }

        public AssemblyData(int index, string name, string pluginassemblyid, bool isExpanded = false)
        {
            Index = index;
            Name = name;
            AssemblyId = pluginassemblyid;
            IsExpanded = isExpanded;
        }
    }

    public class CodeActivityData
    {
        #region Parameters

        public string codeActivityid { get; set; }
        public string name { get; set; }

        public string typename { get; set; }
        public DateTime createdon { get; set; }
        public DateTime modifiedon { get; set; }
        public string createdby { get; set; }
        public string modifiedby { get; set; }

        public string version { get; set; }

        public string customworkflowactivityinfo { get; set; }
        public List<WorkflowData> workflowsList { get; set; }

        public List<string> inArgumentList { get; set; }

        public List<string> outArgumentList { get; set; }


        #endregion Parameters

    }

    public class WorkflowData
    {
        #region Const

        const int LINES_SPACE = 20;
        Color workflowColor = Color.Black;

        #endregion Const

        #region Parameters
        public string workflowid { get; set; }
        public string name { get; set; }
        public string entityName { get; set; }
        public DateTime createdon { get; set; }
        public DateTime modifiedon { get; set; }
        public bool ondemand { get; set; }
        public bool triggeroncreate { get; set; }
        public string triggeronupdateattributelist { get; set; }
        public bool triggerondelete { get; set; }
        public bool subprocess { get; set; }
        public string xaml { get; set; }

        #endregion Parameters

        public WorkflowData()
        {
            workflowid = string.Empty;
            name = string.Empty;
            entityName = string.Empty;
            createdon = DateTime.MinValue;
            modifiedon = DateTime.MinValue;
            ondemand = false;
            triggeroncreate = false;
            triggeronupdateattributelist = "";
            triggerondelete = false;
            subprocess = false;
            xaml = string.Empty;
        }

        public Panel CreateWorkflowBox(int x, int y)
        {
            const int MARGIN_LEFT = 5;
            int maxwidth = 100;

            Panel workflowBoxPanel = new Panel();
            workflowBoxPanel.Name = "id_" + workflowid;
            workflowBoxPanel.Margin = new Padding(20, 0, 0, 0);
            workflowBoxPanel.BorderStyle = BorderStyle.Fixed3D;
            workflowBoxPanel.Location = new Point(x, y);
            workflowBoxPanel.AutoSize = true;

            int y_local = 0;

            System.Windows.Forms.Label workflowNameBox = new System.Windows.Forms.Label();
            workflowNameBox.Name = "id_" + workflowid;
            workflowNameBox.Text = name;
            workflowNameBox.Font = new Font("Calibri", 10, FontStyle.Bold);
            workflowNameBox.AutoSize = false;
            workflowNameBox.Margin = new Padding(0, 0, 0, 0);
            workflowNameBox.Padding = new Padding(MARGIN_LEFT, 0, 0, 0);
            workflowNameBox.BorderStyle = BorderStyle.FixedSingle;
            workflowNameBox.BackColor = Color.LightGray;

            workflowBoxPanel.Controls.Add(workflowNameBox);

            if (workflowNameBox.Width > maxwidth)
                maxwidth = workflowNameBox.Width;

            y_local += LINES_SPACE;
            y_local += LINES_SPACE;

            System.Windows.Forms.Label primaryEntityNameBox = new System.Windows.Forms.Label();
            primaryEntityNameBox.Name = "id_" + workflowid;
            primaryEntityNameBox.Text = "Primary entity: " + entityName;
            primaryEntityNameBox.Font = new Font("Calibri", 9, FontStyle.Bold);
            primaryEntityNameBox.AutoSize = true;
            primaryEntityNameBox.Margin = new Padding(0, 0, MARGIN_LEFT, 0);
            primaryEntityNameBox.Location = new Point(MARGIN_LEFT*2, y_local);
            primaryEntityNameBox.BackColor = Color.White;

            workflowBoxPanel.Controls.Add(primaryEntityNameBox);

            if (primaryEntityNameBox.Width > maxwidth)
                maxwidth = primaryEntityNameBox.Width;

            y_local += LINES_SPACE;
            System.Windows.Forms.Label txtCreatedOn = new System.Windows.Forms.Label();
            txtCreatedOn.Text = "createdon: " + createdon.ToString();
            txtCreatedOn.Font = new Font("Calibri", 9, FontStyle.Regular);
            txtCreatedOn.AutoSize = true;
            txtCreatedOn.Margin = new Padding(0, 0, MARGIN_LEFT, 0);
            txtCreatedOn.Location = new Point(MARGIN_LEFT*2, y_local);
            txtCreatedOn.ForeColor = workflowColor;
            workflowBoxPanel.Controls.Add(txtCreatedOn);

            if (txtCreatedOn.Width > maxwidth)
                maxwidth = txtCreatedOn.Width;


            y_local += LINES_SPACE;
            System.Windows.Forms.Label txtModifiedOn = new System.Windows.Forms.Label();
            txtModifiedOn.Text = "modifiedon: " + modifiedon.ToString();
            txtModifiedOn.Font = new Font("Calibri", 9, FontStyle.Regular);
            txtModifiedOn.AutoSize = true;
            txtModifiedOn.Margin = new Padding(0, 0, MARGIN_LEFT, 0);
            txtModifiedOn.Location = new Point(MARGIN_LEFT*2, y_local);
            txtModifiedOn.ForeColor = workflowColor;
            workflowBoxPanel.Controls.Add(txtModifiedOn);

            if (txtModifiedOn.Width > maxwidth)
                maxwidth = txtModifiedOn.Width;


            StringBuilder outpuData = new StringBuilder();

            outpuData.AppendLine("The process starts:");
            if (ondemand)
                outpuData.AppendLine("     • On demand");

            if (triggeroncreate)
                outpuData.AppendLine("     • When record is created");

            if (!string.IsNullOrWhiteSpace(triggeronupdateattributelist))
                outpuData.AppendLine("     • When record fields change: " + triggeronupdateattributelist.Replace(",", ", "));

            if (triggerondelete)
                outpuData.AppendLine("     • When record is deleted");

            y_local += LINES_SPACE;
            System.Windows.Forms.Label txtProcessStartWhen = new System.Windows.Forms.Label();
            txtProcessStartWhen.Text = outpuData.ToString();
            txtProcessStartWhen.Font = new Font("Calibri", 9, FontStyle.Regular);
            txtProcessStartWhen.AutoSize = true;
            txtProcessStartWhen.Margin = new Padding(0, 0, MARGIN_LEFT, 10);
            txtProcessStartWhen.Location = new Point(MARGIN_LEFT*2, y_local);
            txtProcessStartWhen.ForeColor = workflowColor;
            workflowBoxPanel.Controls.Add(txtProcessStartWhen);

            if (txtProcessStartWhen.Width > maxwidth)
                maxwidth = txtProcessStartWhen.Width;

            maxwidth += MARGIN_LEFT * 3;

            workflowNameBox.SetBounds(0, 0, maxwidth, 25);

            return workflowBoxPanel;
        }
    }

    class GFG : IComparer<AssemblyData>
    {
        public int Compare(AssemblyData x, AssemblyData y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            // "CompareTo()" method 
            return x.Name.CompareTo(y.Name);

        }
    }

    #endregion clases auxiliares

    #region enums

    public enum VISIBILITY_TYPES { EXPANDED, COLLAPSED, SOME}

    #endregion
}