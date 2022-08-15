using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraEditors;
//ALTER DATABASE HOB SET ENABLE BROKER
namespace DemoSQL
{
    public partial class Form1 : Form
    {
        public string m_connect = "Server=113.161.161.109,14333;Database = DPSS_ERP;User ID=sa;Password=1Te@mdbschenker";
        SqlConnection con = null;
        public delegate void NewHome();
        public event NewHome OnNewHome;
        public Form1()
        {
            InitializeComponent();
            try
            {
                SqlClientPermission ss = new SqlClientPermission(System.Security.Permissions.PermissionState.Unrestricted);
                ss.Demand();
            }
            catch (Exception)
            {

                throw;
            }
            SqlDependency.Stop(m_connect);
            SqlDependency.Start(m_connect);
            con = new SqlConnection(m_connect);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OnNewHome+=new NewHome(Form1_OnNewHome);//tab
            //load data vao datagrid
            LoadData();
        }

        public void Form1_OnNewHome()
        {
            ISynchronizeInvoke i = (ISynchronizeInvoke)this;
            if (i.InvokeRequired)//tab
            {
                NewHome dd = new NewHome(Form1_OnNewHome);
                i.BeginInvoke(dd, null);
                return;
            }
            LoadData();
        }

        //Ham load data
        void LoadData()
        {
            DataTable dt = new DataTable();
            if (con.State==ConnectionState.Closed)
            {
                con.Open();
            }
            string _sql = @"select 
	                            ID,
	                            ShipmentID,
	                            RequestDepartment,
	                            ConfirmDepartment,
	                            FromFinalDate,
	                            ToFinalDate,
	                            FromConfirmDate,
	                            ToConfirmDate,
	                            RequestDate,
	                            ConfirmDate,
	                            ShipmentQty
                            from dbo.ShipmentSelectedChange
                            order by Editdate desc";

            //string _sql1 = @"select 
	           //                                 sv.masv,
	           //                                 sv.tensv,
	           //                                 k.Tenkhoa
            //                                from dbo.tbl_sinhvien sv
            //                                INNER JOIN dbo.tbl_khoa k on sv.khoa = k.Makhoa
            //                                Order by sv.Modified desc";
            SqlCommand cmd = new SqlCommand(_sql, con);
            cmd.Notification = null;

            SqlDependency de = new SqlDependency(cmd);
            de.OnChange += new OnChangeEventHandler(de_OnChange);

            dt.Load(cmd.ExecuteReader(CommandBehavior.CloseConnection));
            gridControl1.DataSource = dt;
            gridControl1.ForceInitialize();
        }
        public void de_OnChange(object sender, SqlNotificationEventArgs e)
        {
            SqlDependency de = sender as SqlDependency;
            de.OnChange -= de_OnChange;
            if (OnNewHome!=null)
            {
                OnNewHome();
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            gridView1.IndicatorWidth = 60;
            if (e.RowHandle >= 0)
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //XtraMessageBox.Show(gridView1.GetRowCellValue(e.RowHandle, "tensv").ToString());
        }
    }
}
