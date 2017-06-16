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
using System.Globalization;

namespace Metro_Navigation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty zoomScaleProperty = DependencyProperty.Register("scale", typeof(double), typeof(MainWindow));
        public static readonly DependencyProperty zoomCentreProperty = DependencyProperty.Register("center", typeof(Point), typeof(MainWindow));
        public static readonly DependencyProperty xTransformProperty = DependencyProperty.Register("x", typeof(double), typeof(MainWindow));
        public static readonly DependencyProperty yTransformProperty = DependencyProperty.Register("y", typeof(double), typeof(MainWindow));
        public static readonly DependencyProperty moveProperty = DependencyProperty.Register("moveVector",typeof(Vector),typeof(MainWindow)); 
        public double scale
        {
            get { return (double)GetValue(zoomScaleProperty); }
            set { SetValue(zoomScaleProperty, value); }
        }

        public Point center
        {
            get { return (Point) GetValue(zoomCentreProperty); }
            set { SetValue(zoomCentreProperty, value);}
        }

        public double x
        {
            get { return (double) GetValue(xTransformProperty); }
            set { SetValue(xTransformProperty, value);}
        }

        public double y
        {
            get { return (double)GetValue(yTransformProperty); }
            set { SetValue(yTransformProperty, value); }
        }

        public Vector moveVector
        {
            get { return (Vector) GetValue(moveProperty); }
            set { SetValue(moveProperty, value);}
        }



        public bool IsMouseDown;
        public bool IsStationOneChosen;
        public bool IsStationTwoChosen;
        public bool IsClick;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            center = new Point(0.5, 0.5);
            scale = 0.9;
            x = 50;
            y = 0;
            IsMouseDown = false;
            IsStationOneChosen = false;
            IsStationTwoChosen = false;
            IsClick = false;
        }



        private void Grid1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((scale > 0.5 && scale < 2.5) || (scale < 0.5 && e.Delta > 0) || (scale > 2.5 && e.Delta < 0))
            {
                center = new Point(Mouse.GetPosition(Canvas1).X / Canvas1.ActualWidth, Mouse.GetPosition(Canvas1).Y / Canvas1.ActualHeight);
                double delta = e.Delta > 0 ? 1.1 : 1 / 1.1;
                scale *= delta;
                x = Mouse.GetPosition(Grid1).X - center.X * Grid1.ActualWidth;
                y = Mouse.GetPosition(Grid1).Y - center.Y * Grid1.ActualHeight;
            }
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = true;
            IsClick = true;
            moveVector = -(Vector)Mouse.GetPosition(this);
        }
        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
            {
                moveVector += (Vector) Mouse.GetPosition(this);
                x += moveVector.X;
                y += moveVector.Y;
                moveVector = -(Vector) Mouse.GetPosition(this);
                IsClick = false;
            }

        }
        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseDown)
            {
                IsMouseDown = false;
                if (IsStationTwoChosen&&IsClick)
                {
                        IsStationOneChosen = false;
                        IsStationTwoChosen = false;
                        TextBox1.Text = "";
                        TextBox2.Text = "";
                    ReturnOpacity();
                }
            }
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (e.PreviousSize.Height != 0)
            {
                x += (e.NewSize.Width - e.PreviousSize.Width)/2*scale;
            y += (e.NewSize.Height - e.PreviousSize.Height)/2*scale;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            
            if (!IsStationOneChosen)
            {
                TextBox1.Text = (string)btn.Content;
                TextBox1.Tag = btn.Tag;
                IsStationOneChosen = true;
                btn.BorderThickness = new Thickness(1);

            }
            else if (!IsStationTwoChosen)
            {
                if (TextBox1.Text != (string) btn.Content)
                {
                    TextBox2.Text = (string) btn.Content;
                    TextBox2.Tag = btn.Tag;
                    btn.BorderThickness = new Thickness(1);
                    IsStationTwoChosen = true;
                    searchPath(Int32.Parse(TextBox1.Tag.ToString()), Int32.Parse(TextBox2.Tag.ToString()));

                }
            }
            else
            {
                IsStationOneChosen = false;
                IsStationTwoChosen = false;
                TextBox1.Text = "";
                TextBox2.Text = "";
                ReturnOpacity();
            }

        }

        private void searchPath(int a, int b)
        {
            int[][] M = new int[3][];
            M[0] = new int[18];
            M[1] = new int[18];
            M[2] = new int[16];
            int[,] P = { { 0, 111, 110}, { 208, 0, 209}, { 304, 305, 0}};
            for (int i = 0; i < 18; i++)
            {
                M[0][i] = 100 + i + 1;
                M[1][i] = 200 + i + 1;
                if (i < 16) M[2][i] = 300 + i + 1;
            }

            if (a > b)
            {
                int t = a;
                a = b;
                b = t;
            }

            int M1 = (int) a/100 - 1;
            int M2 = (int) b/100 - 1;

            int P1=P[M1,M2];
            int P2=P[M2, M1];

            List<int> Path = new List<int>();

            int metro = a;
            int finish = P1;
            int j = a%100-1;
            int line = M1;
            while (metro != b)
            {
                if (P1 == 0)
                {
                    metro = M[M1][j];
                    if(Path.Count!=0)
                        Path.Add(1000+metro+Path[Path.Count-1]);
                    Path.Add(metro);
                    j++;
                }
                else
                {
                    if (finish != b)
                    {
                        metro = M[line][j];
                        if (metro != finish)
                        {
                            if (Path.Count != 0)
                                Path.Add(1000 + metro + Path[Path.Count - 1]);
                            Path.Add(metro);
                        }
                        
                        else
                        {
                            if (Path.Count != 0)
                                Path.Add(1000 + metro + Path[Path.Count - 1]);
                            Path.Add(metro);
                            finish = b; 
                            line = M2;
                            j = P2%100 - 1;
                            metro = M[line][j];
                            Path.Add(M1+M2+2);
                            Path.Add(metro);
                        }
                        if (finish > metro)
                            j++;
                        else j--;
                    }
                    else
                    {
                        metro = M[line][j];
                        if (Path.Count != 0)
                            Path.Add(1000 + metro + Path[Path.Count - 1]);
                        Path.Add(metro);
                        if (finish > metro)
                            j++;
                        else j--;
                    }
                }

            }

            ShowPath(Path);



        }

        private void ShowPath(List<int> Path)
        {
            foreach (var elem in Canvas1.Children)
            {

                if (elem is Control)
                        
                        {
                            Button btn = elem as Button;
                            
                            
                    if (!Path.Contains(Int32.Parse(btn.Tag.ToString())))
                            btn.Opacity = 0.05;
                        }
                else
                {
                    Shape shape = elem as Shape;
                    if (!Path.Contains(Int32.Parse(shape.Tag.ToString())))
                        shape.Opacity = 0.05;
                }

            }
        }

        private void ReturnOpacity()
        {
            foreach (var elem in Canvas1.Children)
            {

                if (elem is Control)

                {
                    Button btn = elem as Button;
                        btn.Opacity = 1;
                    btn.BorderThickness = new Thickness(0);
                }
                else
                {
                    Shape shape = elem as Shape;
                    shape.Opacity = 1;
                }

            }
        }

    }


}
