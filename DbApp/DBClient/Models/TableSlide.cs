using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DBClient.Models;
using DBClient.ViewModels;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;

using System.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace DBClient.Models
{
    public class TableSlide : TabItem, INotifyPropertyChanged
    {
        // link to ViewModel
        ViewModel viewModel;

        // StackPanel that contains all in the TabItem content
        private Grid itemGrid = new Grid();
        public Grid ItemGrid
        {
            get { return itemGrid; }
            set
            {
                itemGrid = value;
                OnPropertyChanged("ItemGrid");
            }
        }
        // ScrollViewer
        private ScrollViewer scrollViewer = new ScrollViewer() {};               
        public ScrollViewer ScrollViewer
        { 
            get { return scrollViewer; }
            set 
            { 
                scrollViewer = value;
                OnPropertyChanged("ScrollViewer");
            }
        }
        // StackPanel in scroll
        private StackPanel stackViewer = new StackPanel();
        public StackPanel StackViewer
        {
            get { return stackViewer; }
            set
            {
                stackViewer = value;
                OnPropertyChanged("StackViewer");
            }
        }
        // StackPanel in header
        private StackPanel headerStack = new StackPanel() { Orientation = Orientation.Horizontal};
        public StackPanel HeaderStack
        {
            get { return headerStack; }
            set
            {
                headerStack = value;
                OnPropertyChanged("HeaderStack");
            }
        }
        // StackPanel in header
        private UniformGrid columnsGrid = new UniformGrid();
        public UniformGrid ColumnsGrid
        {
            get { return columnsGrid; }
            set
            {
                columnsGrid = value;
                OnPropertyChanged("ColumnsStack");
            }
        }
        // Name of this table which was given by server
        private string tableName;
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; OnPropertyChanged(nameof(TableName)); }
        }

        // Count of columns in current table
        private int columnsCount;
        // Contains name of each column which was given by server
        private string[] columnsNames;

        // Propeties of of origin table, it needs to compare the changes before save it to server
        // Origin count of columns in table 
        private int oldColumnsCount;
        // Old version of columns names, compare to new if new column has been added 
        private string[] oldColumnsNames;
        // Old version of table that used for cheking changes
        private string[][] oldTable;

        // Constructor
        public TableSlide(Packet packet, ViewModel model)
        {
            // Set ViewModel
            viewModel = model;

            // Set count of columns
            if (packet.columns_name != null)
            {
                columnsCount = packet.columns_name.Count();
                oldColumnsCount = packet.columns_name.Count();
            }

            // Fill the bar with names of columns
            FillColumns(packet.columns_name);

            // Fill the name of table
            TableName = packet.table_name;

            // Containig header
            Label label = new Label()
            { 
                Content = TableName,
                Height = 20,
                Padding = new Thickness(0, 0, 0, -4)
            };
            Button button0 = new Button()
            {
                Command = AddLineCommand,
                Content = "=",
                Height = 15,
                Width = 15,
                Padding = new Thickness(3, -2, 4, 0),
                Margin = new Thickness(5, 0, 0, 0),
                FontSize = 11,
            };
            Button button1 = new Button()
            { 
                Command= AddColumnCommand,
                Content= "+",
                Height = 15,
                Width = 15,
                Padding = new Thickness(3,-2,4,0),
                Margin = new Thickness(5, 0, 0, 0),
                FontSize = 11,
            };
            Button button2 = new Button()
            {
                Command = SaveCommand,
                Content = "˅",
                Height = 15,
                Width = 15,
                Padding = new Thickness(3, -2, 4, 0),
                Margin = new Thickness(5, 0, 0, 0),
                FontSize = 11,
            };
            Button button3 = new Button()
            {
                Command = CloseTableCommand,
                Content = "x",
                Height = 15,
                Width = 15,
                Padding = new Thickness(3, -2, 4, 0),
                Margin = new Thickness(5, 0, 0, 0),
                FontSize = 11,
            };
            HeaderStack.Children.Add(label);
            HeaderStack.Children.Add(button0);
            HeaderStack.Children.Add(button1);
            HeaderStack.Children.Add(button2);
            HeaderStack.Children.Add(button3);
            Header = HeaderStack;

            // Table name fill
            TableName = packet.table_name;

            // Create copy of table
            oldTable = packet.table;

            // Creating table
            foreach (string[] item in packet.table)
            {
                AddLine(item);
            }
            ScrollViewer.Content = StackViewer;

            // Preparing GridItem
            RowDefinition rd1 = new RowDefinition();
            rd1.Height = new GridLength(20, GridUnitType.Pixel);

            RowDefinition rd2 = new RowDefinition();
            rd2.Height = new GridLength(1, GridUnitType.Star);
            
            ItemGrid.RowDefinitions.Add(rd1);
            ItemGrid.RowDefinitions.Add(rd2);

            // Attaching elements to grid
            this.ColumnsGrid.SetValue(Grid.RowProperty, 0);
            this.ScrollViewer.SetValue(Grid.RowProperty, 1);

            // Fill all item by StackPanels
            ItemGrid.Children.Add(ColumnsGrid);
            ItemGrid.Children.Add(ScrollViewer);
            Content = ItemGrid;
        }

        // Commands which work with table
        public ICommand AddLineCommand
        {
            get
            {
                return new Command((obj) =>
                {
                    AddLineMethod();
                });
            }
        }
        public ICommand AddColumnCommand
        {
            get
            {
                return new Command((obj) =>
                {
                    AddColumnMethod();
                });
            }
        }
        public ICommand CloseTableCommand
        {
            get
            {
                return new Command((obj) =>
                {
                    CloseTableMethod();
                });
            }
        }
        public ICommand SaveCommand
        {
            get
            {
                return new Command((obj) =>
                {
                    SaveTableMethod();
                });
            }
        }

        // Method for buttons which calls by Commands
        public void AddLineMethod()
        {
            try
            {
                UniformGrid itemGrid = new UniformGrid()
                {
                    Columns = columnsCount
                };
                for (int i = 0; i < columnsCount; i++)
                {
                    TextBox textBox = new TextBox()
                    {
                        AcceptsReturn = true,
                        TextWrapping = TextWrapping.Wrap
                    };
                    itemGrid.Children.Add(textBox);
                }
                stackViewer.Children.Add(itemGrid);
            }
            catch
            {
                tableName = "error";
            }
        }
        public void AddColumnMethod()
        {
            try
            {
                // raise count of coulumns in table properties
                columnsCount++;

                foreach (UniformGrid grid in stackViewer.Children)
                {
                    // Change count of columns in grid to actual
                    grid.Columns = columnsCount;

                    // Add TextBox for each UniforGrid in table 
                    TextBox textBox = new TextBox()
                    {
                        AcceptsReturn = true,
                        TextWrapping = TextWrapping.Wrap
                    };
                    grid.Children.Add(textBox);
                }
                TextBox columnName = new TextBox()
                {
                    AcceptsReturn = true,
                    TextWrapping = TextWrapping.Wrap
                };
                ColumnsGrid.Columns++;
                ColumnsGrid.Children.Add(columnName);
            }
            catch
            {
                tableName = "error";
            }
        }
        public void CloseTableMethod()
        {
            try
            {
                foreach (TableSlide slide in viewModel.tableSlides)
                {
                    if (slide.TableName == tableName)
                    { 
                        viewModel.tableSlides.Remove(slide);
                    }
                }
            }
            catch
            {
                tableName = "error";
            }
        }
        public async void SaveTableMethod()
        {
            try
            {
                // Create Packet with base info
                Packet packet = new Packet()
                {
                    name = viewModel.viewData.Name,
                    password = viewModel.viewData.Password,

                    ip = viewModel.viewData.IP,
                    port = viewModel.viewData.Port,

                    type = "save",
                    table_name = TableName,

                    columns_name = GetActualColumns(),
                    table = GetChanges()
                };
                //viewModel.viewData.Console = $"{oldTable.Length.ToString()} ; {oldColumnsCount.ToString()}";
                if (packet.table != null)
                {
                    packet = await viewModel.ServerRequestAsync(packet);
                }
                
            }
            catch
            {
                viewModel.viewData.TableName = "error";
            }
        }

        // Method that adds line into table while table is creating
        public void AddLine(string[] item)
        {
            UniformGrid itemGrid = new UniformGrid()
            {
                Columns = item.Length
            };
            foreach (string str in item)
            {
                TextBox textBox = new TextBox()
                {
                    Text = str,
                    AcceptsReturn = true,
                    TextWrapping = TextWrapping.Wrap
                };
                itemGrid.Children.Add(textBox);
            }
            stackViewer.Children.Add(itemGrid);
        }
        public void FillColumns(string[] columns)
        {
            foreach (string str in columns)
            {
                TextBox textBox = new TextBox()
                {
                    Text = str,
                    AcceptsReturn = true,
                    TextWrapping = TextWrapping.Wrap,
                    IsEnabled = false,
                };
                ColumnsGrid.Children.Add(textBox);
            }
            ColumnsGrid.Columns = columns.Length;
            ColumnsGrid.Margin = new Thickness(0, 0, 17, 0);
        }
        public string[] GetActualColumns()
        {
            List<string> actualColumns = new List<string>();
            foreach (TextBox tb in ColumnsGrid.Children)
            {
                actualColumns.Add(tb.Text);
            }
            return actualColumns.ToArray();
        }
        public string[][] GetChanges()
        {
            try
            {
                // Get actual columns names
                List<string> actualColumns = new List<string>();
                foreach (TextBox tb in ColumnsGrid.Children)
                {
                    actualColumns.Add(tb.Text);
                }

                // Set table answer
                List<string[]> answer = new List<string[]>();
                int i = 0;
                foreach (UniformGrid line in stackViewer.Children)
                {
                    // Checking old lines
                    if (i < oldTable.Length)
                    {
                        int j = 0;
                        foreach (TextBox item in line.Children)
                        {
                            string firstColumn = "";
                            if (j == 0)
                                firstColumn = item.Text;

                            // Checking old columns
                            if (j < oldColumnsCount)
                            {
                                if (item.Text != oldTable[i][j])
                                {
                                    string[] anotherItem = new string[3];
                                    anotherItem[0] = item.Text;
                                    anotherItem[1] = actualColumns[j];
                                    anotherItem[2] = firstColumn;

                                    answer.Add(anotherItem);
                                }
                            }
                            // Checking new columns
                            else
                            {
                                if (item.Text != "")
                                {
                                    string[] anotherItem = new string[3];
                                    anotherItem[0] = item.Text;
                                    anotherItem[1] = actualColumns[j];
                                    anotherItem[2] = firstColumn;

                                    answer.Add(anotherItem);
                                }
                            }
                            j++;
                        }
                        i++;
                    }
                    // Checking new lines
                    else
                    {
                        int j = 0;
                        List<string> anotherItem = new List<string>();
                        anotherItem.Add("new");
                        foreach (TextBox item in line.Children)
                        {
                            anotherItem.Add(item.Text);
                        }
                        i++;
                    }
                }
                return answer.ToArray();
            }
            catch
            {
                return null;
            }
        }

        // INotifyPropertyChanged realization / MVVM realization
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
