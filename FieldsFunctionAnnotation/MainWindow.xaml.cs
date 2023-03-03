using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FieldsFunctionAnnotation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Generate_Fields(int size)
        {
            Back.Children.Clear();
            Front.Children.Clear();
            Row.Items.Clear();
            Column.Items.Clear();
            for (int i = 0; i < size; i++)
            {
                var back_row = new Grid()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 80*i, 0, 0)
                };
                var front_row = new Grid()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 80 * i, 0, 0)
                };
                Row.Items.Add(i);
                Column.Items.Add(i);
                for (int j = 0; j < size; j++)
                {
                    
                    var back_column = new Grid()
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(80 * j, 0, 0, 0)
                    };
                    var back_ball = new ToggleButton()
                    {
                        Margin = new Thickness(0),
                        Style = FindResource("Ball") as Style
                    };
                    var back_road = new Button()
                    {
                        Margin = new Thickness(30,0,0,0),
                        Style = FindResource("Road") as Style
                    };
                    var verticalback_road = new Button()
                    {
                        Margin = new Thickness(0, 30, 0, 0),
                        Style = FindResource("VerticalRoad") as Style
                    };
                    back_column.Children.Add(back_ball);
                    back_column.Children.Add(back_road);
                    back_column.Children.Add(verticalback_road);
                    back_row.Children.Add(back_column);

                    var front_column = new Grid()
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(80 * j, 0, 0, 0)
                    };
                    var arrow = new ToggleButton()
                    {
                        Margin = new Thickness(15, 15, 0, 0),
                        Style = FindResource("Arrow") as Style
                    };
                    var front_ball = new ToggleButton()
                    {
                        Margin = new Thickness(50, 50, 0, 0),
                        Style = FindResource("Ball") as Style
                    };
                    var front_road = new Button()
                    {
                        Margin = new Thickness(80, 50, 0, 0),
                        Style = FindResource("Road") as Style
                    };
                    var verticalfront_road = new Button()
                    {
                        Margin = new Thickness(50, 80, 0, 0),
                        Style = FindResource("VerticalRoad") as Style
                    };
                    front_column.Children.Add(arrow);
                    front_column.Children.Add(front_ball);
                    front_column.Children.Add(front_road);
                    front_column.Children.Add(verticalfront_road);
                    front_row.Children.Add(front_column);
                }
                Back.Children.Add(back_row);
                Front.Children.Add(front_row);
            }
        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            int grid_row = Row.SelectedIndex;
            int grid_column = Column.SelectedIndex;
            int[,] array = new int[2, 2];
            var row = Back.Children[grid_row] as Grid;
            for (int i = 0; i < grid_column; i++)
            {
                var local_grid = row.Children[i] as Grid;
                //Красит горизонтальный путь
                var road = local_grid.Children[1] as Button;
                road.Background = new SolidColorBrush(Color.FromRgb(255, 150, 100));
            }
            for (int i = grid_row; i < Front.Children.Count; i++)
            {
                var local_grid = Front.Children[i] as Grid;
                var column_grid = local_grid.Children[grid_column] as Grid;
                //Красит стрелочку
                if (i == grid_row) //не красит, если стрелочка с другого ряда
                {
                    var arrow = column_grid.Children[0] as ToggleButton;
                    arrow.Background = new SolidColorBrush(Color.FromRgb(255, 150, 100));
                }
                //Красит вертикальный путь
                if (i != Front.Children.Count - 1) //не красит вертикально, если последний ряд
                {
                    var road = column_grid.Children[3] as Button;
                    road.Background = new SolidColorBrush(Color.FromRgb(255, 150, 100));
                }
            }

        }

        private void ChangeSize_Click(object sender, RoutedEventArgs e)
        {
            int size = Convert.ToInt32(Size.Text);
            Generate_Fields(size);
        }
    }
}
