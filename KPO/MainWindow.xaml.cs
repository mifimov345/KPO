using AvalonDock.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.Toolkit.Core;

namespace KPO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Figures currentFigure = new Figures();
        enum Figures
        {
            Pen,
            Line,
            Ellipse,
            Eraser,
            Star
        }
        Point startCoordinate = new Point();
        Brush currentColor = Brushes.Black;
        double thickness = 1;
        int anglesNum = 3;
        bool isDelete = false;

        bool DrawAvaliable = false;

        public MainWindow()
        {
            InitializeComponent();
            SaveAlthough.IsHitTestVisible= false;
            SaveCurrent.IsHitTestVisible = false;
            
        }

        static List<Button> Canvases = new List<Button>();

        static List<Canvas> CanvasEnumerable = new List<Canvas>();

        static List<LayoutAnchorablePane> ChildrenCvs = new List<LayoutAnchorablePane>();

        static List<LayoutAnchorable> ChildrenAnchorable = new List<LayoutAnchorable>();

        static Canvas LastChosenCanvas = new Canvas();

        public static int counter = 0;

        private void MenuItem_OpenFile_Click(object sender, RoutedEventArgs e)
        {


            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "BMP or PNG(*.png or *.bmp)|*.png;*.bmp|PNG (*.png)|*.png|BMP (*.bmp)|*bmp";
            BitmapCache bmc = new BitmapCache();
            if (ofd.ShowDialog() == true)
            {
                foreach (string fileName in ofd.FileNames)
                {
                    Image con = CreateImage(fileName);
                    NewFile(con);
                    isSaved= true;
                    filePath = fileName;
                }
            }
        }

        public Image CreateImage(string fileName)
        {
            Image con = new Image();
            BitmapImage com = new BitmapImage();
            Uri uri = new Uri(fileName);
            com.BeginInit();
            com.UriSource = uri;
            com.CacheOption = BitmapCacheOption.OnLoad;
            com.EndInit();
            WIDTH = (int)com.Width;
            HEIGHT = (int)com.Height;
            con.Source = com;
            return con;
        }

        private void ChangeColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (ChangeColor.SelectedColor != null) currentColor = new SolidColorBrush((Color)ChangeColor.SelectedColor);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem currentItem = (ComboBoxItem)comboBox.SelectedItem;
            TextBlock selectedText = (TextBlock)currentItem.Content;
            if (selectedText != null) thickness = Convert.ToDouble(selectedText.Text);
        }
        
        private void ComboBox_CornerChange(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem currentItem = (ComboBoxItem)comboBox.SelectedItem;
            TextBlock selectedText = (TextBlock)currentItem.Content;
            if (selectedText != null) anglesNum = Convert.ToInt32(selectedText.Text);
        }
        private void ButtonEllipse_Click(object sender, RoutedEventArgs e)
        {
            currentFigure = Figures.Ellipse;
        }

        private void ButtonLine_Click(object sender, RoutedEventArgs e)
        {
            currentFigure = Figures.Line;
        }

        private void ButtonStar_Click(object sender, RoutedEventArgs e)
        {
            currentFigure = Figures.Star;
        }

        private void ButtonErase_Click(object sender, RoutedEventArgs e)
        {
            currentFigure = Figures.Eraser;
        }

        private void ButtonPen_Click(object sender, RoutedEventArgs e)
        {
            currentFigure = Figures.Pen;
        }

        private void InkCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DrawAvaliable = true;
        }

        private void InkCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isSaved = false;

            Canvas canv = (Canvas)sender;
            startCoordinate.X = e.GetPosition(canv).X;
            startCoordinate.Y = e.GetPosition(canv).Y;

            isDelete = false;
            DrawAvaliable = true;

        }

        private void InkCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DrawAvaliable= false;
            }
            else
            {
                DrawAvaliable= true;
            }
        }

        private void DrawPen(object sender, MouseEventArgs e, Canvas es)
        {
            Line line1 = new Line();
            line1.X1 = startCoordinate.X;
            line1.Y1 = startCoordinate.Y;
            line1.X2 = e.GetPosition(es).X;
            line1.Y2 = e.GetPosition(es).Y;
            line1.Stroke = currentColor;
            line1.StrokeThickness = thickness;
            //this.AddLogicalChild(line1);
            es.Children.Add(line1);
            startCoordinate.X = e.GetPosition(es).X;
            startCoordinate.Y = e.GetPosition(es).Y;

            // рисуем кружочками
            Ellipse el1 = new Ellipse();
            el1.Width = thickness;
            el1.Height = thickness;
            el1.Stroke = currentColor;
            el1.Fill = currentColor;
            Canvas.SetTop(el1, e.GetPosition(es).Y - thickness / 2);
            Canvas.SetLeft(el1, e.GetPosition(es).X - thickness / 2);

            es.Children.Add(el1);
            
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                es.Children.Remove(el1);
                es.Children.Remove(line1);
                /*this.RemoveLogicalChild(el1);
                this.RemoveLogicalChild(line1);*/
            }
        }

        private void DrawLine(object sender, MouseEventArgs e, Canvas es)
        {
            Line l = new Line();
            l.X1 = startCoordinate.X;
            l.Y1 = startCoordinate.Y;

            l.X2 = e.GetPosition(es).X;
            l.Y2 = e.GetPosition(es).Y;

            l.Stroke = currentColor;
            l.StrokeThickness = thickness;
            /*if (isDelete) this.RemoveLogicalChild(this.VisualChildrenCount - 1);
            this.AddLogicalChild(l);*/
            if (isDelete) es.Children.RemoveAt(es.Children.Count - 1);
            es.Children.Add(l);
            isDelete = true;
        }

        private void DrawEllipse(object sender, MouseEventArgs e, Canvas es)
        {
            Ellipse el = new Ellipse();


            Canvas.SetLeft(el, startCoordinate.X);
            if (startCoordinate.X > e.GetPosition(es).X)
                Canvas.SetLeft(el, e.GetPosition(es).X);

            Canvas.SetTop(el, startCoordinate.Y);
            if (startCoordinate.Y > e.GetPosition(es).Y)
                Canvas.SetTop(el, e.GetPosition(es).Y);


            el.Width = Math.Abs(startCoordinate.X - e.GetPosition(es).X);
            el.Height = Math.Abs(startCoordinate.Y - e.GetPosition(es).Y);
            el.Stroke = currentColor;

            /*if (isDelete) this.RemoveLogicalChild(this.VisualChildrenCount - 1);
            this.AddLogicalChild(el);*/
            if (isDelete) es.Children.RemoveAt(es.Children.Count - 1);
            es.Children.Add(el);
            isDelete = true;
        }

        private void DrawEraser(object sender, MouseEventArgs e, Canvas es)
        {
            Rectangle p = new Rectangle();
            p.Height = thickness;
            p.Width = thickness;
            p.Fill = es.Background;
            p.Stroke = es.Background;
            Canvas.SetLeft(p, e.GetPosition(es).X - thickness / 2);
            Canvas.SetTop(p, e.GetPosition(es).Y - thickness / 2);

            Line line1 = new Line();
            line1.X1 = startCoordinate.X;
            line1.Y1 = startCoordinate.Y;
            line1.X2 = e.GetPosition(es).X;
            line1.Y2 = e.GetPosition(es).Y;
            line1.Stroke = es.Background;
            line1.StrokeThickness = thickness;
            /*this.AddLogicalChild(line1);*/
            es.Children.Add(line1);
            startCoordinate.X = e.GetPosition(es).X;
            startCoordinate.Y = e.GetPosition(es).Y;

            /*this.AddLogicalChild(p);*/
            es.Children.Add(p);
        }

        private void DrawStar(object sender, MouseEventArgs e, Canvas es)
        {
            Point currentPosition = e.GetPosition(es);

            double width = Math.Abs(currentPosition.X - startCoordinate.X);
            double height = Math.Abs(currentPosition.Y - startCoordinate.Y);

            double x0 = width / 2;
            double y0 = height / 2;

            // кол-во вершин 
            int n = anglesNum;

            // R/r = ratio
            double ratio = 2;

            // радиусы
            double R = Math.Min(width, height) / 2;
            double r = R / ratio;

            Point[] points = new Point[2 * n + 1];
            double rotateValue = Math.PI / n;

            // текущий угол + определение начального
            double angle;
            if (n % 4 == 0)
                angle = 0;
            else if (n % 4 == 1)
                angle = 1.5 * rotateValue;
            else if (n % 4 == 2)
                angle = rotateValue;
            else
                angle = 1.5 * rotateValue - Math.PI;

            double currRadius;
            for (int k = 0; k < 2 * n + 1; k++)
            {
                if (k % 2 == 0)
                    currRadius = R;
                else
                    currRadius = r / 2;
                points[k] = new Point((float)(x0 + currRadius * Math.Cos(angle)), (float)(y0 + currRadius * Math.Sin(angle)));
                angle += rotateValue;
            }

            Polygon star = new Polygon();
            star.Stroke = currentColor;
            star.Fill = currentColor;
            star.StrokeThickness = thickness;
            star.Points = new PointCollection(points);

            Canvas.SetTop(star, Math.Min(currentPosition.Y, startCoordinate.Y) - thickness / 2);
            Canvas.SetLeft(star, Math.Min(currentPosition.X, startCoordinate.X) - thickness / 2);
            if (isDelete)
            {
                /*this.RemoveLogicalChild(this.VisualChildrenCount - 1);*/
                es.Children.RemoveAt(es.Children.Count - 1);
            }

            /*this.AddLogicalChild(star);*/
            es.Children.Add(star);
            isDelete = true;
        }

        private void InkCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            DrawAvaliable = false;
        }

        private void InkCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Canvas es = (Canvas)sender;
            DrawAvaliable &= (e.GetPosition(es).X <= es.ActualWidth - 20 || e.GetPosition(es).Y <= es.ActualHeight - 20);
            if (e.LeftButton == MouseButtonState.Pressed && DrawAvaliable)
            {
                switch (currentFigure)
                {
                    case Figures.Pen:
                        DrawPen(sender, e, es);
                        break;

                    case Figures.Line:
                        DrawLine(sender, e, es);
                        break;

                    case Figures.Ellipse:
                        DrawEllipse(sender, e, es);
                        break;

                    case Figures.Eraser:
                        DrawEraser(sender, e, es);
                        break;

                    case Figures.Star:
                        DrawStar(sender, e, es);
                        break;
                }
            }
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        public Double FixedCanvasWidth = new Double();

        public Double FixedCanvasHeight = new Double();

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e, Canvas canv)
        {
            DrawAvaliable = false;
            if (canv.Width + e.HorizontalChange > 0 && canv.Height + e.VerticalChange > 0)
            {
                canv.Width += e.HorizontalChange;
                canv.Height += e.VerticalChange;
            }
            List<UIElement> childrenToRemove = new List<UIElement>();
            foreach (UIElement child in canv.Children)
            {
                double left = (double)child.GetValue(Canvas.LeftProperty);
                double top = (double)child.GetValue(Canvas.TopProperty);
                double width = child.RenderSize.Width;
                double height = child.RenderSize.Height;

                if (child is Line)
                {
                    Line line = (Line)child;
                    if (line.X1 > canv.ActualWidth || line.Y1 > canv.ActualHeight || line.X2 > canv.ActualWidth || line.Y2 > canv.ActualHeight)
                    {
                        childrenToRemove.Add(child);
                    }
                }
                else if (left + width < 0 || top + height < 0 || left + width > canv.ActualWidth || top + height > canv.ActualHeight)
                {
                    childrenToRemove.Add(child);
                }

            }
            foreach (UIElement child in childrenToRemove)
            {
                canv.Children.Remove(child);
            }
        }

        private void KeepThumbInRightBottomCorner(Thumb thumb, Canvas canvas)
        {
            thumb.SizeChanged += (sender, e) =>
            {
                Canvas.SetLeft(thumb, canvas.ActualWidth - thumb.ActualWidth);
                Canvas.SetTop(thumb, canvas.ActualHeight - thumb.ActualHeight);
            };

            canvas.SizeChanged += (sender, e) =>
            {
                Canvas.SetLeft(thumb, canvas.ActualWidth - thumb.ActualWidth);
                Canvas.SetTop(thumb, canvas.ActualHeight - thumb.ActualHeight);
            };
        }

        private void Button_ChoiceSav(object sender, RoutedEventArgs e)
        {

        }

        bool IsClosing;

        private void tab_Closure(CancelEventArgs e , LayoutAnchorablePane pane, MenuItem dynamicItem)
        {
            DialogBoxWithResult dialogBoxWithResult = new DialogBoxWithResult();
            // Open dialog box and retrieve dialog result when ShowDialog returns
            bool? dialogResult = dialogBoxWithResult.ShowDialog();
            switch (dialogResult)
            {
                case true:
                    // save
                    DynDirSave(dynamicItem ,LastChosenCanvas);
                    FileDialogueSaver.Items.Remove(dynamicItem);

                    Panel.Children.Remove(pane);
                    counter--;
                    if (FileDialogueSaver.Items.Count == 0)
                    {
                        SaveAlthough.IsHitTestVisible = false;
                        SaveCurrent.IsHitTestVisible = false;
                    }
                    break;
                case false:
                    e.Cancel= true;
                    
                    break;
                default:
                    // return
                    break;
            }

            /*MessageBox messageBox = null;
            //BtnSav.Click += (sender, e) => Button_ChoiceSav;
            string caption = "";
            MessageBoxButton Unclose = MessageBoxButton.OK;
            MessageBoxButton Save = MessageBoxButton.OK;
            MessageBox.Show("Вы забыли сохранить изображение",caption, Unclose);*/
        }

        bool isSaved = false;
        string filePath;

        private void DynDirSave(object sender, Canvas n1)
        {
            isSaved = true;
            MenuItem MIT = (MenuItem)sender;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = MIT.Header.ToString(); // Default file name
            saveFileDialog.DefaultExt = ".jpg|.bmp"; // Default file extension
            saveFileDialog.Filter = "Image (.jpg)|*.jpg | (.bmp)|*.bmp"; // Filter files by extension
            // Show save file dialog box
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                // Save document
                string filename = saveFileDialog.FileName;
                filePath = filename;
                //get the dimensions of the ink control
                int margin = (int)LastChosenCanvas.Margin.Left;
                int width = (int)((LastChosenCanvas.ActualWidth - margin)* scaling);
                int height = (int)((LastChosenCanvas.Height - margin) * scaling);
                //render ink to bitmap
                RenderTargetBitmap rtb =
                new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Default);
                rtb.Render(LastChosenCanvas);

                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    encoder.Save(fs);
                }
            }
        }

        int WIDTH = 1000;
        int HEIGHT = 1000;

        string CurrentBtn;

        double scaling = 1;

        private void IsChosen(object sender, EventArgs e)
        {
            LayoutAnchorable la = (LayoutAnchorable)sender;
            ScrollViewer content = (ScrollViewer)la.Content;
            Canvas c = (Canvas)content.Content;
            LastChosenCanvas = c;
            SaveCurrent.IsHitTestVisible= true;
        }

        private void OG_Click(object sender, RoutedEventArgs e)
        {
            Canvas s = new Canvas();
            if (isSaved)
            {
                int margin = (int)LastChosenCanvas.Margin.Left;
                int width = (int)LastChosenCanvas.ActualWidth - margin;
                int height = (int)LastChosenCanvas.ActualHeight - margin;
                //render ink to bitmap
                RenderTargetBitmap rtb =
                new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Default);
                rtb.Render(LastChosenCanvas);

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    encoder.Save(fs);
                }
            }
            else
            {
                DynDirSave(sender, s);
            }

        }

        private void NewFile(Image i = null)
        {
            string name = "Tab " + (counter++).ToString();

            Canvas n1 = new Canvas();

            if (i != null)
            {
                n1.Children.Add(i);
            }

            n1.Height = HEIGHT;
            n1.Width = WIDTH;

            Thumb thumb = new Thumb();
            thumb.Background = Brushes.Transparent;
            thumb.Width = 20;
            thumb.Height = 20;
            thumb.DragDelta += (sender, e) => Thumb_DragDelta(sender, e, n1);
            thumb.Cursor = Cursors.SizeAll;
            KeepThumbInRightBottomCorner(thumb, n1);
            n1.Loaded += (sender, e) => KeepThumbInRightBottomCorner(thumb, n1);
            //thumb.DragCompleted += (sender, e) => Thumb_DragCompleted(sender, e, canv);
            //thumb.DragStarted += (sender, e) => Thumb_DragStarted(sender, e, canv);
            n1.Children.Add(thumb);
            Canvas.SetLeft(thumb, n1.ActualWidth - thumb.Width);
            Canvas.SetTop(thumb, n1.ActualHeight - thumb.Height);


            n1.SetValue(Canvas.LeftProperty, 0.0); // sets left to zero
            n1.SetValue(Canvas.TopProperty, 0.0); // sets top to zero
            n1.VerticalAlignment = VerticalAlignment.Top;
            n1.HorizontalAlignment = HorizontalAlignment.Left;


            LayoutAnchorable tab = new LayoutAnchorable();

            LayoutAnchorablePane pane = new LayoutAnchorablePane();

            Binding binding= new Binding();

            binding.ElementName = tab.ToString();

            tab.Title = name;

            tab.IsActiveChanged += IsChosen;

            var st = new ScaleTransform();

            n1.RenderTransform = st;
            //n1.Name = name;


            n1.MouseLeftButtonUp += InkCanvas_MouseLeftButtonUp;

            n1.MouseLeftButtonDown += InkCanvas_MouseLeftButtonDown;

            n1.MouseEnter += InkCanvas_MouseEnter;

            n1.MouseLeave += InkCanvas_MouseLeave;

            n1.MouseMove += InkCanvas_MouseMove;


            
            //VerticalScrollBarVisibility="Auto" HorizontalScrollBarVisibility="Auto">

            ScrollViewer scrollViewer = new ScrollViewer();

            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

            n1.MouseWheel += (sender, e) =>
            {
                if (e.Delta > 0)
                {
                    st.ScaleX *= 2;
                    st.ScaleY *= 2;

                    scaling *= 2;
                }
                else
                {
                    st.ScaleX /= 2;
                    st.ScaleY /= 2;

                    scaling /= 2;
                }
            };

            FixedCanvasHeight = n1.Height;

            FixedCanvasWidth = n1.Width;

            /*Size ds = new Size(FixedCanvasWidth, FixedCanvasHeight);

            n1.DesiredSize= ds*/
            ;



            n1.Background = Brushes.White;



            scrollViewer.Content = n1;

            //scrollViewer.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            tab.Content = scrollViewer;

            pane.Children.Add(tab);

            CanvasEnumerable.Add(n1);

            /*pane.DockMinWidth = 100;

            pane.DockMinWidth = 100;*/
            MenuItem dynamicItem = new MenuItem();

            dynamicItem.Header = tab.Title;

            dynamicItem.Click += (sender, e) => DynDirSave(sender, n1);

            //tab.IsSelectedChanged += 

            tab.Hiding += (sender, e) => tab_Closure(e, pane, dynamicItem);

            ChildrenAnchorable.Add(tab);

            ChildrenCvs.Add(pane);

            

            FileDialogueSaver.Items.Add(dynamicItem);

            Panel.Children.Add(pane);

            SaveAlthough.IsHitTestVisible= true;
        }


        /*private void TempSave()
        {
        }*/
        private void MenuItem_NewFile_Click(object sender, RoutedEventArgs e)
        {
            /*LayoutAnchorable tab = new LayoutAnchorable();
            LayoutAnchorablePane pane = new LayoutAnchorablePane();
            tab.Title = "Tab " + (counter++).ToString();
            //tab.Content = "Tab " + (counter++).ToString();
            pane.Children.Add(tab);
            ChildrenCvs.Add(pane);
            Panel.Children.Add(pane);*/

            NewFile();
        }

        private void OpenAbout(object sender, RoutedEventArgs e)
        {
            var AboutPaint = new AboutPaint();
            AboutPaint.Show();
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
