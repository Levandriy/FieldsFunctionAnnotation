using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;

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
using static FieldsFunctionAnnotation.MainWindow;

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
        public class BrokenKey
        {
            public Nullable<int> Row { get; set; } = null!;
            public Nullable<int> Column { get; set; } = null!;
            public ToggleButton Toggle { get; set; } = null!;
        }
        public class Snake
        {
            public int Start { get; set; }
            public int End { get; set; }
            public int Destination_column { get; set; }
            public int Destination_row { get; set; }
            public SolidColorBrush Snake_Color { get; set; } = new();
            public int Current_Row { get; set; }
            public int Current_Column { get; set; } = 0;
            public bool Snake_on_Front = false;
            public bool? win = null;
            public void Reset(int size)
            {
                var snake = this;
                snake.Current_Row = snake.Start;
                snake.Current_Column = 0;
                snake.Destination_column = snake.End;
                snake.Destination_row = size;
                snake.win = null;
                snake.Snake_on_Front = false;
            }
        }
        public List<Snake> Snakes = new();
        public List<BrokenKey> BrokenKeys = new();
        private void Delete_Snake(Snake snake)
        {
            snake.Snake_Color = new SolidColorBrush(Colors.Black);
            Execute_Snake(snake);
            Snakes.Remove(snake);
        }
        private void Key_broken(object sender, RoutedEventArgs e)
        {
            var clicked = sender as ToggleButton;
            for (int i = 0; i < Front.Children.Count; i++)
            {
                var grid = Front.Children[i] as Grid;
                for (int j = 0; j < grid.Children.Count; j++)
                {
                    var child_grid = grid.Children[j] as Grid;
                    var arrow = child_grid.Children[0] as ToggleButton;
                    if (arrow == clicked)
                    {
                        var query = from snakes in Snakes select snakes;
                        Snake snake = query.FirstOrDefault();
                        if (snake != null)
                        {
                            snake.Reset(grid.Children.Count - 1);
                            Delete_Snake(snake);
                            Debug.WriteLine($"Удалена змейка {snake.Start}:{snake.End} ; Может дойти {snake.win}");
                        }
                        BrokenKey brokenKey = new BrokenKey()
                        {
                            Row = i,
                            Column = j,
                            Toggle = arrow
                        };
                        if (arrow.IsChecked == true)
                        {
                            BrokenKeys.Add(brokenKey);
                            Debug.WriteLine($"Добавлен сломанный ключ {brokenKey.Row} : {brokenKey.Column}");
                        }
                        else
                        {
                            var key = from keys in BrokenKeys where keys.Row == brokenKey.Row && keys.Column == brokenKey.Column select keys;
                            Debug.WriteLine($"Удалён сломанный ключ {brokenKey.Row} : {brokenKey.Column} : {BrokenKeys.Remove(key.FirstOrDefault())}");
                        }
                        if (snake != null)
                        {
                            snake.Reset(grid.Children.Count - 1);
                            snake.Snake_Color = new SolidColorBrush(Color.FromRgb(145, 135, 165)); 
                            Snakes.Add(snake);
                            Execute_Snake(snake);
                            Debug.WriteLine($"Добавлена змейка {snake.Start}:{snake.End} ; Может дойти {snake.win}");
                        }
                    }
                }
            }
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
                    Margin = new Thickness(0, 80 * i, 0, 0)
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
                    var back_road = new Button()
                    {
                        Margin = new Thickness(0, 0, 0, 0),
                        Style = FindResource("Road") as Style
                    };
                    var back_ball = new ToggleButton()
                    {
                        Margin = new Thickness(50, 0, 0, 0),
                        Style = FindResource("Ball") as Style
                    };
                    back_column.Children.Add(back_road);
                    back_column.Children.Add(back_ball);
                    back_row.Children.Add(back_column);

                    var front_column = new Grid()
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(80 * j, 0, 0, 0)
                    };
                    var arrow = new ToggleButton()
                    {
                        Margin = new Thickness(65, 15, 0, 0),
                        Style = FindResource("Arrow") as Style
                    };
                    arrow.Checked += Key_broken;
                    arrow.Unchecked += Key_broken;
                    var front_ball = new ToggleButton()
                    {
                        Margin = new Thickness(95, 50, 0, 0),
                        Style = FindResource("Ball") as Style
                    };
                    var verticalfront_road = new Button()
                    {
                        Margin = new Thickness(95, 80, 0, 0),
                        Style = FindResource("VerticalRoad") as Style
                    };
                    front_column.Children.Add(arrow);
                    front_column.Children.Add(front_ball);
                    front_column.Children.Add(verticalfront_road);
                    front_row.Children.Add(front_column);
                }
                Back.Children.Add(back_row);
                Front.Children.Add(front_row);
            }
        }
        private Grid Get_Back_Grid(int row, int column)
        {
            var grid = Back.Children[row] as Grid;
            var ret = grid.Children[column] as Grid;
            return ret;
        }
        private Grid Get_Front_Grid(int row, int column)
        {
            var grid = Front.Children[row] as Grid;
            var ret = grid.Children[column] as Grid;
            return ret;
        }
        public void Change_grid(Snake snake)
        {
            if (snake.Snake_on_Front)
            {
                snake.Snake_on_Front = false;
            }
            else
            {
                snake.Snake_on_Front = true;
            }
            var back_column = Get_Back_Grid(snake.Current_Row, snake.Current_Column);
            var back_ball = back_column.Children[1] as ToggleButton;
            var front_column = Get_Front_Grid(snake.Current_Row, snake.Current_Column);
            var arrow = front_column.Children[0] as ToggleButton;
            var ball = front_column.Children[1] as ToggleButton;
            if (arrow.Background == snake.Snake_Color)
            {
                arrow.Background = new SolidColorBrush(Colors.Black);
                ball.Background = new SolidColorBrush(Colors.Black);
                back_ball.Background = new SolidColorBrush(Colors.Black);
            }
            else
            {
                arrow.Background = snake.Snake_Color;
                ball.Background = snake.Snake_Color;
                back_ball.Background = snake.Snake_Color;
                Debug.WriteLine("Покрасил Стрелочку");
            }
        }
        private bool Move_vertically(Snake snake, int step)
        {
            int size = Convert.ToInt32(Size.Text) - 1;
            if (snake.Current_Row >= 0 && snake.Current_Row < size)
            {
                var front_column = Get_Front_Grid(snake.Current_Row, snake.Current_Column);
                var road = front_column.Children[2] as Button;
                Debug.WriteLine($"Буду красить вертикальный путь в точке {snake.Current_Row}; {snake.Current_Column}");
                if (road.Background == snake.Snake_Color)
                {
                    road.Background = new SolidColorBrush(Colors.Black);
                    Debug.WriteLine($"Покрасил вертикальный путь в чёрный, иду назад");
                }
                else
                {
                    road.Background = snake.Snake_Color;
                    Debug.WriteLine($"Покрасил вертикальный путь в {snake.Snake_Color}");
                }
                Debug.WriteLine("Иду на " + step + " по вертикали");
                snake.Current_Row += step;
                Debug.WriteLine("Перешёл на новый ряд, поэтому ресет колонки назначения");
                snake.Destination_column = snake.End;
                return true;
            }
            else
            {
                Debug.WriteLine("По вертикали не выходит");
                return false;
            }
        }
        private bool Move_horizontally(Snake snake, int step)
        {
            int size = Convert.ToInt32(Size.Text) - 1;
            if(snake.Current_Column == snake.End && snake.Current_Row == size)
            {
                snake.win = true;
                Change_grid(snake);
                return false;
            }
            else
            {
                if (snake.Current_Column >= 0 && snake.Current_Column <= size)
                {
                    if (step < 0)
                    {
                        var back_column = Get_Back_Grid(snake.Current_Row, snake.Current_Column);
                        var road = back_column.Children[0] as Button;
                        Debug.WriteLine($"Буду красить горизонтальный путь в точке {snake.Current_Row}; {snake.Current_Column}");
                        if (road.Background == snake.Snake_Color)
                        {
                            road.Background = new SolidColorBrush(Colors.Black);
                            Debug.WriteLine("Покрасил горизонтальный путь в чёрный, вернулся назад");
                        }
                        else
                        {
                            road.Background = snake.Snake_Color;
                            Debug.WriteLine($"Покрасил горизонтальный путь в {snake.Snake_Color}");
                        }
                        Debug.WriteLine("Иду на " + step + " по горизонтали");
                        snake.Current_Column += step;
                    }
                    else
                    {
                        Debug.WriteLine("Иду на " + step + " по горизонтали");
                        snake.Current_Column += step;
                        var back_column = Get_Back_Grid(snake.Current_Row, snake.Current_Column);
                        var road = back_column.Children[0] as Button;
                        Debug.WriteLine($"Буду красить горизонтальный путь в точке {snake.Current_Row}; {snake.Current_Column}");
                        if (road.Background == snake.Snake_Color)
                        {
                            road.Background = new SolidColorBrush(Colors.Black);
                            Debug.WriteLine("Покрасил горизонтальный путь в чёрный, вернулся назад");
                        }
                        else
                        {
                            road.Background = snake.Snake_Color;
                            Debug.WriteLine($"Покрасил горизонтальный путь в {snake.Snake_Color}");
                        }
                    }
                    Debug.WriteLine("Перешёл на новую колонку, поэтому ресет ряда назначения");
                    snake.Destination_row = size;
                    return true;
                }
                else
                {
                    Debug.WriteLine("По горизонтали не выходит");
                    return false;
                }
            }
        }
        private void Move(Snake snake)
        {
            int size = Convert.ToInt32(Size.Text) - 1;
            if (snake.Snake_on_Front)
            {
                Debug.WriteLine("Нахожусь впереди, иду по вертикали");
                int diff = snake.Destination_row - snake.Current_Row;
                Debug.WriteLine($"Ряд-назначение: {snake.Destination_row}|| Ряд текущий: {snake.Current_Row}");
                if (diff > 0)
                {
                    Debug.WriteLine($"Разница: {diff}");
                    int step = diff / Math.Abs(diff);
                    var vertical_query = from keys in BrokenKeys
                                         where keys.Column == snake.Current_Column
                                         && keys.Row == snake.Current_Row + step
                                         select keys;
                    if (vertical_query.FirstOrDefault() == null)
                    {
                        if (Move_vertically(snake, step))
                        {
                            Debug.WriteLine("Moved_Vertically " + step);
                            Debug.WriteLine("Змейка на " + snake.Current_Row + " ряду и " + snake.Current_Column + " колонке");
                            Debug.WriteLine("Змейка должна идти к" + size + " ряду и " + snake.End + " колонке");
                            if (snake.Current_Column != snake.End || snake.Current_Row != size)
                            {
                                Change_grid(snake);
                            }
                            else
                            {
                                snake.win = true;
                            }
                            
                        }
                        else
                        {
                            Debug.WriteLine("Проиграл на вертикале");
                            snake.win = false;
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"Ключ по вертикали {vertical_query.FirstOrDefault().Row}:{vertical_query.FirstOrDefault().Column}");
                        snake.Destination_row -= step;
                    }
                }
                else if (diff == 0)
                {
                    int change = snake.Current_Column - snake.End;
                    if (change != 0)
                    {
                        int step = change / Math.Abs(change);
                        snake.Destination_column += step;
                        Debug.WriteLine("Текущая колонка - это не тот ряд, поэтому меняю колонку, куда нужно идти");
                    }
                    else
                    {
                        Debug.WriteLine("Текущая колонка - нужный ряд, но внизу сломан ключ");
                        if (snake.Current_Column > 0)
                        {
                            snake.Destination_column--;
                            Debug.WriteLine("Слева есть место, уменьшаю цель-колонку");
                        }
                        else
                        {
                            snake.Destination_column++;
                            Debug.WriteLine("Слева места нет, увеличиваю цель-колонку");
                        }
                    }
                    Change_grid(snake);
                    Debug.WriteLine("Иду на задний ряд");
                }
            }
            else
            {
                int diff = snake.Destination_column - snake.Current_Column;
                Debug.WriteLine("Это разница между текущей колонкой и колонкой назначения: " + diff);
                if (diff != 0)
                {
                    int step = diff / Math.Abs(diff);
                    Debug.WriteLine("Нужно пошагать горизонтально на: " + step);
                    var horizontal_query = from keys in BrokenKeys
                                           where keys.Row == snake.Current_Row
                                           && keys.Column == snake.Current_Column + step
                                           select keys;
                    if (horizontal_query.FirstOrDefault() == null)
                    {
                        if (Move_horizontally(snake, step))
                        {
                            Debug.WriteLine("Moved_Horizontally " + step);
                        }
                        else
                        {
                            Debug.WriteLine("Проиграл на горизонтале");
                            snake.win = false;
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"Ключ по горизонтали {horizontal_query.FirstOrDefault().Row}:{horizontal_query.FirstOrDefault().Column}");
                        snake.Destination_column -= step;
                        Debug.WriteLine("Уменьшил колонку назначения. Теперь: " +  snake.Destination_column);
                    }
                }
                if(snake.Destination_column == snake.Current_Column)
                {
                    try
                    {
                        Change_grid(snake);
                    }
                    catch
                    {
                        snake.win = false;
                    }
                }
            }
        }
        private void Execute_Snake(Snake snake)
        {
            int size = Convert.ToInt32(Size.Text) - 1;
            {
                var back_column = Get_Back_Grid(snake.Current_Row, snake.Current_Column);
                var road = back_column.Children[0] as Button;
                if (road.Background != snake.Snake_Color)
                {
                    road.Background = snake.Snake_Color;
                }
                else
                {
                    road.Background = new SolidColorBrush(Colors.Black);
                }
            }
            while (snake.win == null)
            {
                Move(snake);
                Debug.WriteLine(snake.Current_Row);
                Debug.WriteLine(snake.Current_Column);
                if (snake.Current_Row == size && snake.Current_Column == snake.End)
                {
                    snake.win = true;
                }
            }
            if (snake.win != null)
            {
                if ((bool)snake.win)
                {
                    Debug.WriteLine("Победа");
                    var front_column = Get_Front_Grid(snake.Current_Row, snake.Current_Column);
                    var road = front_column.Children[2] as Button;
                    if (road.Background != snake.Snake_Color)
                    {
                        road.Background = snake.Snake_Color;
                    }
                    else
                    {
                        road.Background = new SolidColorBrush(Colors.Black);
                    }
                }
                else
                {
                    MessageBox.Show("Невозможно достичь цели");
                }
            }
        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            int size = Convert.ToInt32(Size.Text) - 1;
            int row = Row.SelectedIndex;
            int column = Column.SelectedIndex;
            Snake snake = new Snake()
            {
                Start = row,
                End = column,
                Snake_Color = new SolidColorBrush(Color.FromRgb(145, 135, 165)),
                Current_Row = row,
                Destination_row = size,
                Destination_column = column
            };
            Snakes.Add(snake);
            Debug.WriteLine($"Добавлена змейка {snake.Start}:{snake.End} ; Может дойти {snake.win}");
            Execute_Snake(snake);
        }

        private void ChangeSize_Click(object sender, RoutedEventArgs e)
        {
            int size = Convert.ToInt32(Size.Text);
            BrokenKeys.Clear();
            Generate_Fields(size);
        }
    }
}
