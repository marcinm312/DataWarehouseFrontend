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
        void wypisz(string text)
        {
            tbOutput.AppendText(text + "\r\n");
        }
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
        private void testBtn_Click(object sender, RoutedEventArgs e)
        {
            wypisz("Lista kostek:");
            connection.Open();
            foreach (CubeDef def in connection.Cubes)
            {
                wypisz(def.Type + "\t" + def.Name);
            }
            connection.Close();
        }

        private void mdxBtn_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            string query = "with member measures.TotalSales as '[Measures].[Quantity] * [Measures].[Unit Price]' select {[Measures].[Order Details Count], measures.TotalSales} on columns, {[Products].[Company Name].Members} on rows from[Order_Products_Suppl]";
            AdomdCommand cmd = new AdomdCommand(query, connection);
            CellSet cellset = cmd.ExecuteCellSet();
            Cell cell = null;
            int countOnAxis0 = cellset.Axes[0].Positions.Count;
            int countOnAxis1 = cellset.Axes[1].Positions.Count;
            for (int i0 = 0; i0 < countOnAxis0; i0++)
            {
                for (int i1 = 0; i1 < countOnAxis1; i1++)
                {
                    cell = cellset.Cells[i0, i1];
                    wypisz("(" + cellset.Axes[0].Positions[i0].Members[0].Name
                    + " ,"
                    + cellset.Axes[1].Positions[i1].Members[0].Name
                    + ") :\t"
                    + cell.FormattedValue);
                }
            }
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
