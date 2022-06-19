using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.AnalysisServices.AdomdClient;
using System.Data;

namespace ApkaDoHurtowni
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AdomdConnection connection;

        private readonly ComboBoxItem allQuarters = new ComboBoxItem();
        private readonly ComboBoxItem quarter1 = new ComboBoxItem();
        private readonly ComboBoxItem quarter2 = new ComboBoxItem();
        private readonly ComboBoxItem quarter3 = new ComboBoxItem();
        private readonly ComboBoxItem quarter4 = new ComboBoxItem();

        private readonly ComboBoxItem allMonths = new ComboBoxItem();
        private readonly ComboBoxItem allMonthsQuarter = new ComboBoxItem();
        private readonly ComboBoxItem january = new ComboBoxItem();
        private readonly ComboBoxItem february = new ComboBoxItem();
        private readonly ComboBoxItem march = new ComboBoxItem();
        private readonly ComboBoxItem april = new ComboBoxItem();
        private readonly ComboBoxItem may = new ComboBoxItem();
        private readonly ComboBoxItem june = new ComboBoxItem();
        private readonly ComboBoxItem july = new ComboBoxItem();
        private readonly ComboBoxItem august = new ComboBoxItem();
        private readonly ComboBoxItem september = new ComboBoxItem();
        private readonly ComboBoxItem october = new ComboBoxItem();
        private readonly ComboBoxItem november = new ComboBoxItem();
        private readonly ComboBoxItem december = new ComboBoxItem();

        public MainWindow()
        {
            InitializeComponent();

            connection = ReadConnectionFromFile();

            allQuarters.Content = "Wszystkie kwartaly";
            allQuarters.IsSelected = true;
            quarter1.Content = "Quarter 1";
            quarter2.Content = "Quarter 2";
            quarter3.Content = "Quarter 3";
            quarter4.Content = "Quarter 4";

            QuarterComboBox.Items.Add(allQuarters);

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

            MonthComboBox.Items.Add(allMonths);
        }

        private AdomdConnection ReadConnectionFromFile()
        {
            string connectionString = "";
            AdomdConnection connection = null;
            try
            {
                string fileContent = System.IO.File.ReadAllText("Connection.txt");
                if (fileContent != null)
                {
                    connectionString = fileContent;
                }
                connection = new AdomdConnection(connectionString);
            }
            catch (Exception readException)
            {
                MessageBox.Show("Unable to load Connection.txt file, try again. Message: " + readException.Message, "Load connection file error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
            return connection;
        }

        private void QueryToTable(string query)
        {
            try
            {
                connection.Open();
                QueryInput.Text = query;
                AdomdCommand cmd = new AdomdCommand(query, connection);
                DataSet dataSet = new DataSet
                {
                    EnforceConstraints = true
                };
                DataTable dt = new DataTable();
                try
                {
                    dt.Load(cmd.ExecuteReader());
                }
                catch (Exception)
                {

                }
                dataSet.Tables.Add(dt);
                var table = dataSet.Tables[0];
                ResultDataGrid.Columns.Clear();
                foreach (DataColumn dataColumn in dataSet.Tables[0].Columns)
                {
                    ResultDataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = dataColumn.ColumnName,
                        Binding = new Binding("[" + dataColumn.ColumnName + "]")
                    });
                }
                ResultDataGrid.ItemsSource = table.DefaultView;
            }
            catch (Exception dbException)
            {
                MessageBox.Show("Database connection error. Message: " + dbException.Message, "Database connection error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connection.Close();
            }
        }

        private void ExecuteQueryBtn_Click(object sender, RoutedEventArgs e)
        {
            string query = QueryInput.Text;
            QueryToTable(query);
        }
        
        private void VendorBtn1_Click(object sender, RoutedEventArgs e)
        {
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
            QueryToTable(query);
        }

        private void VendorBtn2_Click(object sender, RoutedEventArgs e)
        {
            string query =
                "with member measures.TotalPrice as '[Measures].[Quantity] * [Measures].[Unit Price]' " +
                "select " +
                "{[Measures].[Quantity], [Measures].[Unit Price], measures.TotalPrice " +
                "} " +
                "on columns, " +
                "{Order([Hierarchy].[Category Name].Members, measures.TotalPrice, BDESC)} " +
                "on rows " +
                "from[Order_Products_Suppl]";
            QueryToTable(query);
        }

        private void VendorBtn3_Click(object sender, RoutedEventArgs e)
        {
            string category = CategoryComboBox.Text;
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
            QueryToTable(query);
        }

        private void VendorBtn4_Click(object sender, RoutedEventArgs e)
        {
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
            QueryToTable(query);
        }

        private void SaleBtn1_Click(object sender, RoutedEventArgs e)
        {
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
            QueryToTable(query);
        }

        private void SaleBtn4_Click(object sender, RoutedEventArgs e)
        {
            string greaterOrLessVariable;
            string quantity = QuantityTextBox.Text;

            if (GreaterOrLessComboBox.Text == "Powyżej")
            {
                greaterOrLessVariable = ">";
            }
            else
            {
                greaterOrLessVariable = "<";
            }

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
                greaterOrLessVariable + " " + quantity + "))" +
                "on rows " +
                "from[CubeCustomers]";
            QueryToTable(query);
        }

        private void SaleBtn5_Click(object sender, RoutedEventArgs e)
        {
            string topOrBottomVariable;
            string numberOfRecords = RecordsTextBox.Text;

            if (TopOrBottomComboBox.Text == "Najlepsi")
            {
                topOrBottomVariable = "TOPCOUNT";
            }
            else
            {
                topOrBottomVariable = "BOTTOMCOUNT";
            }

            string query =
                "select " +
                "{[Measures].[Quantity] " +
                "} " +
                "on columns, " +
                topOrBottomVariable + "(crossjoin( " +
                "{[Orders].[Employee ID].Children}, " +
                "{[Orders].[Last Name].Children}, " +
                "{[Orders].[Employees - Country].Children}, " +
                "{[Orders].[Employees - City].Children}, " +
                "{[Orders].[Employees - Address].Children}), " + numberOfRecords +
                ", ([Measures].[Quantity])) " +
                "on rows " +
                "from[CubeCustomers]";
            QueryToTable(query);
        }

        private void SaleBtn2_Click(object sender, RoutedEventArgs e)
        {
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
            QueryToTable(query);
        }

        private void SaleBtn6_Click(object sender, RoutedEventArgs e)
        {
            string topOrBottomVariable;
            string numberOfRecords = Records2TextBox.Text;

            if (TopOrBottom2ComboBox.Text == "Najwięcej")
            {
                topOrBottomVariable = "TOPCOUNT";
            }
            else
            {
                topOrBottomVariable = "BOTTOMCOUNT";
            }

            string query =
                "select " +
                "{[MEASURES].[Quantity] " +
                "} " +
                "on columns, " +
                topOrBottomVariable + "(crossjoin( " +
                "{([Orders].[Customers - Company Name].Children)}, " +
                "{([Orders].[Last Name].Children)}, " +
                "{([Orders].[Employee ID].Children)}), " + numberOfRecords +
                ", ([MEASURES].[Quantity])) on rows " +
                "from[CubeCustomers]";
            QueryToTable(query);
        }

        private void SaleBtn3_Click(object sender, RoutedEventArgs e)
        {
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
            QueryToTable(query);
        }

        private void VendorCostsBtn1_Click(object sender, RoutedEventArgs e)
        {
            string yearVariable;
            string quarterVariable;
            string monthVariable;

            if (YearComboBox.Text == "Wszystkie lata")
            {
                yearVariable = "Children";
            }
            else
            {
                yearVariable = "[Calendar " + YearComboBox.Text + "]";
            }

            if (QuarterComboBox.Text == "Wszystkie kwartaly")
            {
                quarterVariable = "Children";
            }
            else
            {
                quarterVariable = "[" + QuarterComboBox.Text + ", " + YearComboBox.Text + "]";
            }

            if (MonthComboBox.Text == "Wszystkie miesiace" || MonthComboBox.Text == "Caly kwartal")
            {
                monthVariable = "Children";
            }
            else
            {
                monthVariable = "[" + MonthComboBox.Text + " " + YearComboBox.Text + "]";
            }

            string query =
                "select " +
                "NON EMPTY( [Measures].[Freight])on columns, " +
                "NON EMPTY(crossjoin( " +
                "{ ([TimeOrder].[Year]." + yearVariable + ")}, " +
                "{ ([TimeOrder].[Quarter]." + quarterVariable + ")}, " +
                "{ ([TimeOrder].[Month]." + monthVariable + ")}, " +
                "{ ([Shippers].[Company Name].Children)}, " +
                "{ ([Customers].[Company Name].Children)})) on rows " +
                "from[CubeTime]";
            QueryToTable(query);
        }

        private void ProductsBtn1_Click(object sender, RoutedEventArgs e)
        {
            string variableInQuery;
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
            if (DiscontinuedRadioButton.IsChecked == true)
            {
                variableInQuery = "True";
                query = query + variableInQuery + "]";
            }
            if (ProducedRadioButton.IsChecked == true)
            {
                variableInQuery = "False";
                query = query + variableInQuery + "]";
            }
            QueryToTable(query);
        }

        private void QuarterComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (QuarterComboBox.Text == "Wszystkie kwartaly")
            {
                MonthComboBox.Items.Clear();
                MonthComboBox.Items.Add(allMonths);

                allMonths.IsSelected = true;
            }
            if (QuarterComboBox.Text == "Quarter 1")
            {
                MonthComboBox.Items.Clear();
                MonthComboBox.Items.Add(allMonthsQuarter);
                MonthComboBox.Items.Add(january);
                MonthComboBox.Items.Add(february);
                MonthComboBox.Items.Add(march);

                allMonthsQuarter.IsSelected = true;
            }
            if (QuarterComboBox.Text == "Quarter 2")
            {
                MonthComboBox.Items.Clear();
                MonthComboBox.Items.Add(allMonthsQuarter);
                MonthComboBox.Items.Add(april);
                MonthComboBox.Items.Add(may);
                MonthComboBox.Items.Add(june);

                allMonthsQuarter.IsSelected = true;
            }
            if (QuarterComboBox.Text == "Quarter 3")
            {
                MonthComboBox.Items.Clear();
                MonthComboBox.Items.Add(allMonthsQuarter);
                MonthComboBox.Items.Add(july);
                MonthComboBox.Items.Add(august);
                MonthComboBox.Items.Add(september);

                allMonthsQuarter.IsSelected = true;
            }
            if (QuarterComboBox.Text == "Quarter 4")
            {
                MonthComboBox.Items.Clear();
                MonthComboBox.Items.Add(allMonthsQuarter);
                MonthComboBox.Items.Add(october);
                MonthComboBox.Items.Add(november);
                MonthComboBox.Items.Add(december);

                allMonthsQuarter.IsSelected = true;
            }
        }

        private void YearComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (YearComboBox.Text == "Wszystkie lata")
            {
                QuarterComboBox.Items.Clear();
                QuarterComboBox.Items.Add(allQuarters);

                allQuarters.IsSelected = true;

                MonthComboBox.Items.Clear();
                MonthComboBox.Items.Add(allMonths);

                allMonths.IsSelected = true;
            }
            else
            {
                QuarterComboBox.Items.Clear();
                QuarterComboBox.Items.Add(allQuarters);
                QuarterComboBox.Items.Add(quarter1);
                QuarterComboBox.Items.Add(quarter2);
                QuarterComboBox.Items.Add(quarter3);
                QuarterComboBox.Items.Add(quarter4);

                allQuarters.IsSelected = true;
            }
        }
    }
}
