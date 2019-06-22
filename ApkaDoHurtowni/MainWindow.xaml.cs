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

        ComboBoxItem allQuarters = new ComboBoxItem();
        ComboBoxItem quarter1 = new ComboBoxItem();
        ComboBoxItem quarter2 = new ComboBoxItem();
        ComboBoxItem quarter3 = new ComboBoxItem();
        ComboBoxItem quarter4 = new ComboBoxItem();

        ComboBoxItem allMonths = new ComboBoxItem();
        ComboBoxItem allMonthsQuarter = new ComboBoxItem();
        ComboBoxItem january = new ComboBoxItem();
        ComboBoxItem february = new ComboBoxItem();
        ComboBoxItem march = new ComboBoxItem();
        ComboBoxItem april = new ComboBoxItem();
        ComboBoxItem may = new ComboBoxItem();
        ComboBoxItem june = new ComboBoxItem();
        ComboBoxItem july = new ComboBoxItem();
        ComboBoxItem august = new ComboBoxItem();
        ComboBoxItem september = new ComboBoxItem();
        ComboBoxItem october = new ComboBoxItem();
        ComboBoxItem november = new ComboBoxItem();
        ComboBoxItem december = new ComboBoxItem();

        public MainWindow()
        {
            InitializeComponent();

            allQuarters.Content = "Wszystkie kwartaly";
            allQuarters.IsSelected = true;
            quarter1.Content = "Quarter 1";
            quarter2.Content = "Quarter 2";
            quarter3.Content = "Quarter 3";
            quarter4.Content = "Quarter 4";

            comboQuarter.Items.Add(allQuarters);

            allMonths.Content = "Wszystkie miesiace";
            allMonths.IsSelected = true;
            allMonthsQuarter.Content = "Caly kwartal";
            january.Content = "January";
            february.Content = "February";
            march.Content = "March";
            april.Content = "April";
            may.Content = "May";
            june.Content = "June";
            july.Content = "July";
            august.Content = "August";
            september.Content = "September";
            october.Content = "October";
            november.Content = "November";
            december.Content = "December";

            comboMonth.Items.Add(allMonths);
        }
        /*void wypisz(string text)
        {
            tbOutput.AppendText(text + "\r\n");
        }*/
        void zapytanieDoTabeli(string query)
        {
            tbInput.Text = query;
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

        private void btnSprz1_1_Click(object sender, RoutedEventArgs e)
        {
            string zmiennaCombo;
            string iloscSztuk = tbSztuki.Text;

            if (comboMniejWiecej1.Text == "Powyżej")
            {
                zmiennaCombo = ">";
            }
            else
            {
                zmiennaCombo = "<";
            }

            connection.Open();
            string query =
                "select " +
                "{[Measures].[Quantity] " +
                "} " +
                "on columns, " +
                "filter(crossjoin( " +
                "{[Orders].[Employee ID].Children}, " +
                "{[Orders].[Last Name].Children}, " +
                "{[Orders].[Employees - Country].Children}, " +
                "{[Orders].[Employees - City].Children}, " +
                "{[Orders].[Employees - Address].Children}), ([Measures].[Quantity] " +
                zmiennaCombo + " " + iloscSztuk + "))" +
                "on rows " +
                "from[CubeCustomers]";
            zapytanieDoTabeli(query);
            connection.Close();
        }

        private void btnSprz1_2_Click(object sender, RoutedEventArgs e)
        {
            string zmiennaCombo;
            string iloscRekordow = tbRekordy1.Text;

            if (comboLepsiGorsi1.Text == "Najlepsi")
            {
                zmiennaCombo = "TOPCOUNT";
            }
            else
            {
                zmiennaCombo = "BOTTOMCOUNT";
            }

            connection.Open();
            string query =
                "select " +
                "{[Measures].[Quantity] " +
                "} " +
                "on columns, " +
                zmiennaCombo + "(crossjoin( " +
                "{[Orders].[Employee ID].Children}, " +
                "{[Orders].[Last Name].Children}, " +
                "{[Orders].[Employees - Country].Children}, " +
                "{[Orders].[Employees - City].Children}, " +
                "{[Orders].[Employees - Address].Children}), " + iloscRekordow +
                ", ([Measures].[Quantity])) " +
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

        private void btnSprz2_1_Click(object sender, RoutedEventArgs e)
        {
            string zmiennaCombo;
            string iloscRekordow = tbRekordy2.Text;

            if (comboMniejWiecej2.Text == "Najwięcej")
            {
                zmiennaCombo = "TOPCOUNT";
            }
            else
            {
                zmiennaCombo = "BOTTOMCOUNT";
            }

            connection.Open();
            string query =
                "select " +
                "{[MEASURES].[Quantity] " +
                "} " +
                "on columns, " +
                zmiennaCombo + "(crossjoin( " +
                "{([Orders].[Customers - Company Name].Children)}, " +
                "{([Orders].[Last Name].Children)}, " +
                "{([Orders].[Employee ID].Children)}), " + iloscRekordow +
                ", ([MEASURES].[Quantity])) on rows " +
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
            string zmiennaRok;
            string zmiennaKwartal;
            string zmiennaMiesiac;

            if (comboYear.Text == "Wszystkie lata")
            {
                zmiennaRok = "Children";
            }
            else
            {
                zmiennaRok = "[Calendar " + comboYear.Text + "]";
            }

            if (comboQuarter.Text == "Wszystkie kwartaly")
            {
                zmiennaKwartal = "Children";
            }
            else
            {
                zmiennaKwartal = "[" + comboQuarter.Text + ", " + comboYear.Text + "]";
            }

            if (comboMonth.Text == "Wszystkie miesiace" || comboMonth.Text == "Caly kwartal")
            {
                zmiennaMiesiac = "Children";
            }
            else
            {
                zmiennaMiesiac = "[" + comboMonth.Text + " " + comboYear.Text + "]";
            }

            connection.Open();
            string query =
                "select " +
                "NON EMPTY( [Measures].[Freight])on columns, " +
                "NON EMPTY(crossjoin( " +
                "{ ([TimeOrder].[Year]." + zmiennaRok + ")}, " +
                "{ ([TimeOrder].[Quarter]." + zmiennaKwartal + ")}, " +
                "{ ([TimeOrder].[Month]." + zmiennaMiesiac + ")}, " +
                "{ ([Shippers].[Company Name].Children)}, " +
                "{ ([Customers].[Company Name].Children)})) on rows " +
                "from[CubeTime]";
            zapytanieDoTabeli(query);
            connection.Close();
        }

        private void btnProd1_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            string zmienna;
            string query = 
                "select " +
                "{[Measures].[Units In Stock] " +
                "} " +
                "on columns, " +
                "NON EMPTY(crossjoin ( " +
                "{[Products].[Product Name].Children " +
                "}, " +
                "{[Supplier].[Company Name].Children})) on rows " +
                "from[CubeProducts] " +
                "where[Products].[Discontinued].[";
            if (radioWycofane.IsChecked == true)
            {
                zmienna = "True";
                query = query + zmienna + "]";
                zapytanieDoTabeli(query);
            }
            if (radioProdukowane.IsChecked == true)
            {
                zmienna = "False";
                query = query + zmienna + "]";
                zapytanieDoTabeli(query);
            }
            connection.Close();
        }

        private void comboQuarter_DropDownClosed(object sender, EventArgs e)
        {
            if (comboQuarter.Text == "Wszystkie kwartaly")
            {
                comboMonth.Items.Clear();
                comboMonth.Items.Add(allMonths);

                allMonths.IsSelected = true;
            }
            if (comboQuarter.Text == "Quarter 1")
            {
                comboMonth.Items.Clear();
                comboMonth.Items.Add(allMonthsQuarter);
                comboMonth.Items.Add(january);
                comboMonth.Items.Add(february);
                comboMonth.Items.Add(march);

                allMonthsQuarter.IsSelected = true;
            }
            if (comboQuarter.Text == "Quarter 2")
            {
                comboMonth.Items.Clear();
                comboMonth.Items.Add(allMonthsQuarter);
                comboMonth.Items.Add(april);
                comboMonth.Items.Add(may);
                comboMonth.Items.Add(june);

                allMonthsQuarter.IsSelected = true;
            }
            if (comboQuarter.Text == "Quarter 3")
            {
                comboMonth.Items.Clear();
                comboMonth.Items.Add(allMonthsQuarter);
                comboMonth.Items.Add(july);
                comboMonth.Items.Add(august);
                comboMonth.Items.Add(september);

                allMonthsQuarter.IsSelected = true;
            }
            if (comboQuarter.Text == "Quarter 4")
            {
                comboMonth.Items.Clear();
                comboMonth.Items.Add(allMonthsQuarter);
                comboMonth.Items.Add(october);
                comboMonth.Items.Add(november);
                comboMonth.Items.Add(december);

                allMonthsQuarter.IsSelected = true;
            }
        }

        private void comboYear_DropDownClosed(object sender, EventArgs e)
        {
            if (comboYear.Text == "Wszystkie lata")
            {
                comboQuarter.Items.Clear();
                comboQuarter.Items.Add(allQuarters);

                allQuarters.IsSelected = true;

                comboMonth.Items.Clear();
                comboMonth.Items.Add(allMonths);

                allMonths.IsSelected = true;
            }
            else
            {
                comboQuarter.Items.Clear();
                comboQuarter.Items.Add(allQuarters);
                comboQuarter.Items.Add(quarter1);
                comboQuarter.Items.Add(quarter2);
                comboQuarter.Items.Add(quarter3);
                comboQuarter.Items.Add(quarter4);

                allQuarters.IsSelected = true;
            }
        }
    }
}
