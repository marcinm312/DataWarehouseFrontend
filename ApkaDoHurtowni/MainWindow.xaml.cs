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

        private void btnDost1_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            string query =
                "with member measures.TotalSales as '[Measures].[Quantity] * [Measures].[Unit Price]' " +
                "select " +
                "{[Measures].[Quantity], " +
                "[Measures].[Unit Price], " +
                "[Measures].[TotalSales] " +
                "} " +
                "on columns," +
                "{ORDER([Products].[product name].Children, [Measures].[TotalSales], BDESC)} " +
                "on rows " +
                "from[Order_Products_Suppl]";
            zapytanieDoTabeli(query);
            connection.Close();
        }

        private void btnDost2_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            string query =
                "with member measures.TotalPrice as '[Measures].[Quantity] * [Measures].[Unit Price]' " +
                "select " +
                "{[Measures].[Quantity], [Measures].[Unit Price], measures.TotalPrice " +
                "} " +
                "on columns, " +
                "{Order([Hierarchy].[Category Name].Members, measures.TotalPrice, BDESC)} " +
                "on rows " +
                "from[Order_Products_Suppl]";
            zapytanieDoTabeli(query);
            connection.Close();
        }

        private void btnDost3_Click(object sender, RoutedEventArgs e)
        {
            string category = comboCategory.Text;
            connection.Open();
            string query =
                "with member measures.TotalPrice as '[Measures].[Quantity] * [Measures].[Unit Price]' " +
                "select " +
                "{[Measures].[Quantity], measures.TotalPrice " +
                "} " +
                "on columns, " +
                "{[Hierarchy].[Product Name].Members " +
                "} " +
                "on rows " +
                "from[Order_Products_Suppl] " +
                "where[Products].[Category Name].[" + category + "]";
            zapytanieDoTabeli(query);
            connection.Close();
        }

        private void btnDost4_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            string query =
                "with member measures.TotalPrice as '[Measures].[Quantity] * [Measures].[Unit Price]' " +
                "select " +
                "{[Measures].[Order Details Count], measures.TotalPrice " +
                "} " +
                "on columns, " +
                "{[Products].[Company Name].Children " +
                "} " +
                "on rows " +
                "from[Order_Products_Suppl]";
            zapytanieDoTabeli(query);
            connection.Close();
        }

        private void btnSprz1_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            string query =
                "select " +
                "{[Measures].[Quantity] " +
                "} " +
                "on columns, " +
                "crossjoin( " +
                "{[Orders].[Employee ID].Children " +
                "}, " +
                "{[Orders].[Last Name].Children}, " +
                "{[Orders].[Employees - Country].Children}, " +
                "{[Orders].[Employees - City].Children}, " +
                "{[Orders].[Employees - Address].Children}) " +
                "on rows " +
                "from[CubeCustomers]";
            zapytanieDoTabeli(query);
            connection.Close();
        }

        private void btnSprz2_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            string query =
                "select " +
                "{ [MEASURES].[Quantity] " +
                "} " +
                "on columns, " +
                "crossjoin( " +
                "{([Orders].[Customers - Company Name].Children)}, " +
                "{([Orders].[Last Name].Children)}, " +
                "{([Orders].[Employee ID].Children)}) on rows " +
                "from[CubeCustomers]";
            zapytanieDoTabeli(query);
            connection.Close();
        }

        private void btnSprz3_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            string query =
                "with member measures.TotalSales as '[Measures].[Quantity] * [Measures].[Unit Price]' " +
                "select " +
                "{ ([MEASURES].[Quantity]), [MEASURES].[TotalSales] " +
                "} " +
                "on columns, " +
                "Crossjoin( " +
                "{([Orders].[Customers - Company Name].Children)}, " +
                "{([Orders].[City].Children) }, " +
                "{([Products].[Category Name].Children)}) on rows " +
                "from[CubeCustomers]";
            zapytanieDoTabeli(query);
            connection.Close();
        }

        private void btnKoszt1_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            string query =
                "select " +
                "NON EMPTY( [Measures].[Freight])on columns, " +
                "NON EMPTY(crossjoin( " +
                "{ ([TimeOrder].[Year].Children)}, " +
                "{ ([TimeOrder].[Quarter].Children)}, " +
                "{ ([TimeOrder].[Month].Children)}, " +
                "{ ([Shippers].[Company Name].Children)}, " +
                "{ ([Customers].[Company Name].Children)})) on rows " +
                "from[CubeTime]";
            zapytanieDoTabeli(query);
            connection.Close();
        }

        private void btnProd1_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            string query = ""; /*Tu trzeba poprawić i dodać zapytanie*/
            if (radioTak.IsChecked == true)
            {
                zapytanieDoTabeli(query);
            }
            if (radioNie.IsChecked == true)
            {
                zapytanieDoTabeli(query);
            }
            connection.Close();
        }
    }
}
