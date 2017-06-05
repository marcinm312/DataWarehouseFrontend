using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AnalysisServices.AdomdClient;
using System.Data;

namespace ApkaDoHurtowni
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AdomdConnection connection = new AdomdConnection("DataSource=localhost; Catalog=ProjektNaHurtownieNorthwind");
        public MainWindow()
        {
            InitializeComponent();
        }
        /*void wypisz(string text)
        {
            tbOutput.AppendText(text + "\r\n");
        }*/
        void zapytanieDoTabeli(string query)
        {
            AdomdCommand cmd = new AdomdCommand(query, connection);
            DataSet dataSet = new DataSet();
            dataSet.EnforceConstraints = true;
            DataTable dt = new DataTable();
            //CellSet objCellSet = cmd.ExecuteCellSet();
            try
            {
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception exc)
            {

            }
            dataSet.Tables.Add(dt);
            var table = dataSet.Tables[0];
            dataGrid1.Columns.Clear();
            dataGrid1.AutoGenerateColumns = false;
            foreach (DataColumn dataColumn in dataSet.Tables[0].Columns)
            {
                dataGrid1.Columns.Add(new DataGridTextColumn
                {
                    Header = dataColumn.ColumnName,
                    //Header = cmd.ExecuteCellSet(),
                    Binding = new Binding("[" + dataColumn.ColumnName + "]")
                });
            }
            //dataGrid1.ItemsSource = dataSet.Tables[0].DefaultView;
            dataGrid1.ItemsSource = table.DefaultView;
        }

        private void mdxBtn_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            string query = tbInput.Text;
            zapytanieDoTabeli(query);
            connection.Close();
        }

        private void mdx2Btn_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            string query = "with member measures.TotalPrice as '[Measures].[Quantity] * [Measures].[Unit Price]' " +
                "select " +
                "{ [Measures].[Quantity], [Measures].[Unit Price], measures.TotalPrice " +
                "} on columns, " +
                "{[Hierarchy].[Product Name].Members " +
                "} " +
                "on rows " +
                "from[Order_Products_Suppl] " +
                "where[Products].[Category Name].[Seafood]";
            zapytanieDoTabeli(query);
            connection.Close();
        }

        private void mdx3Btn_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            string query = "with member measures.TotalSales as '[Measures].[Quantity] * [Measures].[Unit Price]' " +
                "select " +
                "{[Measures].[Order Details Count], measures.TotalSales " +
                    "} " +
                    "on columns, " +
                "{[Products].[Company Name].Members " +
                "} " +
                "on rows " +
                "from[Order_Products_Suppl];";
            zapytanieDoTabeli(query);
            connection.Close();
        }
    }
}
