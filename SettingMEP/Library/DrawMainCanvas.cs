
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SettingMEP
{
    public class DrawMainCanvas
    {
        public static void DrawSlanted(Canvas canvas)
        {
            Line l1 = new Line() { X1 = 30, X2 =130, Y1 = 200, Y2 = 200 };
            l1.Stroke = Brushes.Black;
            l1.StrokeThickness = 4;
            canvas.Children.Add(l1);
            Line l2 = new Line() { X1 = 130, X2 = 230, Y1 = 200, Y2 = 100 };
            l2.Stroke = Brushes.Black;
            l2.StrokeThickness = 4;
            canvas.Children.Add(l2);
            Line l3 = new Line() { X1 = 230, X2 = 230, Y1 = 100, Y2 = 10 };
            l3.Stroke = Brushes.Black;
            l3.StrokeThickness = 4;
            canvas.Children.Add(l3);
            Line l4 = new Line() { X1 = 110, X2 = 110, Y1 = 180, Y2 = 220 };
            l4.Stroke = Brushes.Black;
            l4.StrokeThickness = 4;
            canvas.Children.Add(l4);
            Line l5 = new Line() { X1 = 130, X2 = 160, Y1 = 170, Y2 = 200 };
            l5.Stroke = Brushes.Black;
            l5.StrokeThickness = 4;
            canvas.Children.Add(l5);
             Line l6 = new Line() { X1 = 200, X2 = 230, Y1 = 100, Y2 = 130 };
            l6.Stroke = Brushes.Black;
            l6.StrokeThickness = 4;
            canvas.Children.Add(l6);
            Line l7 = new Line() { X1 = 210, X2 = 250, Y1 = 80, Y2 = 80 };
            l7.Stroke = Brushes.Black;
            l7.StrokeThickness = 4;
            canvas.Children.Add(l7);
            Line l8 = new Line() { X1 = 210, X2 = 250, Y1 = 10, Y2 = 10 };
            l8.Stroke = Brushes.Black;
            l8.StrokeThickness = 4;
            canvas.Children.Add(l8);
            DimHorizontalText(canvas, 30, 210, 1, 100, 15, -20, -5, "S2");
            DimHorizontalText(canvas, 130, 210, 1, 100, 15, -20,-5, "S1");
            DimVerticalText(canvas, 250, 10, 1, 90, 15, -20,-5, "S2");
            DimVerticalText(canvas, 250, 100, 1, 100, 15, -20,-5, "S1");
        }
        public static void DrawTeeElbow(Canvas canvas)
        {
            Line l1 = new Line() { X1 = 30, X2 = 270, Y1 = 70, Y2 = 70 };
            l1.Stroke = Brushes.Black;
            l1.StrokeThickness = 4;
            canvas.Children.Add(l1);
            Line l2 = new Line() { X1 = 50, X2 = 100, Y1 = 20, Y2 = 70 };
            l2.Stroke = Brushes.Black;
            l2.StrokeThickness = 4;
            canvas.Children.Add(l2);
            Line l3 = new Line() { X1 = 150, X2 = 200, Y1 = 20, Y2 = 70 };
            l3.Stroke = Brushes.Black;
            l3.StrokeThickness = 4;
            canvas.Children.Add(l3);

            Line l4 = new Line() { X1 = 80, X2 = 80, Y1 = 60, Y2 = 80 };
            l4.Stroke = Brushes.Black;
            l4.StrokeThickness = 4;
            canvas.Children.Add(l4);
            Line l5 = new Line() { X1 = 120, X2 = 120, Y1 = 60, Y2 = 80 };
            l5.Stroke = Brushes.Black;
            l5.StrokeThickness = 4;
            canvas.Children.Add(l5);
            Line l6 = new Line() { X1 = 180, X2 = 180, Y1 = 60, Y2 = 80 };
            l6.Stroke = Brushes.Black;
            l6.StrokeThickness = 4;
            canvas.Children.Add(l6);
            Line l7 = new Line() { X1 = 220, X2 = 220, Y1 = 60, Y2 = 80 };
            l7.Stroke = Brushes.Black;
            l7.StrokeThickness = 4;
            canvas.Children.Add(l7);
            Line l8 = new Line() { X1 = 60, X2 = 80, Y1 = 50, Y2 = 30 };
            l8.Stroke = Brushes.Black;
            l8.StrokeThickness = 4;
            canvas.Children.Add(l8);
            Line l9 = new Line() { X1 = 160, X2 = 180, Y1 = 50, Y2 = 30 };
            l9.Stroke = Brushes.Black;
            l9.StrokeThickness = 4;
            canvas.Children.Add(l9);
            DimHorizontalText(canvas, 100, 80, 1, 100, 15, -20, -5, "D1");
            DimHorizontalText(canvas, 100, 200, 1, 100, 15, -20, -5, "D2");

            Line l10 = new Line() { X1 = 100, X2 = 200, Y1 = 190, Y2 = 190 };
            l10.Stroke = Brushes.Black;
            l10.StrokeThickness = 4;
            canvas.Children.Add(l10);
            Line l11 = new Line() { X1 = 50, X2 = 100, Y1 = 140, Y2 = 190 };
            l11.Stroke = Brushes.Black;
            l11.StrokeThickness = 4;
            canvas.Children.Add(l11);
            Line l12 = new Line() { X1 = 250, X2 = 200, Y1 = 140, Y2 = 190 };
            l12.Stroke = Brushes.Black;
            l12.StrokeThickness = 4;
            canvas.Children.Add(l12);
            Line l13 = new Line() { X1 = 120, X2 = 120, Y1 = 180, Y2 = 200 };
            l13.Stroke = Brushes.Black;
            l13.StrokeThickness = 4;
            canvas.Children.Add(l13);

            Line l14 = new Line() { X1 = 180, X2 = 180, Y1 = 180, Y2 = 200 };
            l14.Stroke = Brushes.Black;
            l14.StrokeThickness = 4;
            canvas.Children.Add(l14);
            Line l15 = new Line() { X1 = 70, X2 = 90, Y1 = 180, Y2 = 160 };
            l15.Stroke = Brushes.Black;
            l15.StrokeThickness = 4;
            canvas.Children.Add(l15);
            Line l16 = new Line() { X1 = 210, X2 = 230, Y1 = 160, Y2 = 180 };
            l16.Stroke = Brushes.Black;
            l16.StrokeThickness = 4;
            canvas.Children.Add(l16);
        }
        private static void DimHorizontalText(Canvas canvas, double left, double top, double scale, double l, int font, double offset, double extend, string t)
        {
            Line l1 = new Line() { X1 = left, X2 = left + l / scale, Y1 = top - offset, Y2 = top - offset };
            l1.Stroke = Brushes.Black;
            l1.StrokeThickness = 0.5;
            Line l2 = new Line() { X1 = left - extend, X2 = left + extend, Y1 = top - offset + extend, Y2 = top - offset - extend };
            l2.Stroke = Brushes.Black;
            l2.StrokeThickness = 0.5;
            Line l3 = new Line() { X1 = left - extend + l / scale, X2 = left + extend + l / scale, Y1 = top - offset + extend, Y2 = top - offset - extend };
            l3.Stroke = Brushes.Black;
            l3.StrokeThickness = 0.5;
            Line l4 = new Line() { X1 = left, X2 = left, Y1 = top - offset - extend, Y2 = top - extend };
            l4.Stroke = Brushes.Black;
            l4.StrokeThickness = 0.5;
            Line l5 = new Line() { X1 = left + l / scale, X2 = left + l / scale, Y1 = top - offset - extend, Y2 = top - extend };
            l5.Stroke = Brushes.Black;
            l5.StrokeThickness = 0.5;
            TextBlock text = new TextBlock();
            text.Text = t;
            text.FontSize = font;
            text.Foreground = Brushes.Black;
            text.FontFamily = new FontFamily("Tahoma");
            text.Measure(new Size(System.Double.PositiveInfinity, System.Double.PositiveInfinity));
            text.Arrange(new Rect(text.DesiredSize));
            Canvas.SetTop(text, top - offset - 2 * font);
            Canvas.SetLeft(text, left + l / (2 * scale) - text.ActualWidth / 2);
            canvas.Children.Add(l1);
            canvas.Children.Add(l2);
            canvas.Children.Add(l3);
            canvas.Children.Add(l4);
            canvas.Children.Add(l5);
            canvas.Children.Add(text);

        }
        private static void DimVerticalText(Canvas canvas, double left, double top, double scale, double l, int font, double offset, double extend, string t)
        {
            Line l1 = new Line() { X1 = left - offset, X2 = left - offset, Y1 = top, Y2 = top + l / scale };
            l1.Stroke = Brushes.Black;
            l1.StrokeThickness = 0.5;
            Line l2 = new Line() { X1 = left - extend - offset, X2 = left + extend - offset, Y1 = top + extend, Y2 = top - extend };
            l2.Stroke = Brushes.Black;
            l2.StrokeThickness = 0.5;
            Line l3 = new Line() { X1 = left - extend - offset, X2 = left + extend - offset, Y1 = top + l / scale + extend, Y2 = top + l / scale - extend };
            l3.Stroke = Brushes.Black;
            l3.StrokeThickness = 0.5;
            Line l4 = new Line() { X1 = left - offset - extend, X2 = left - extend, Y1 = top, Y2 = top };
            l4.Stroke = Brushes.Black;
            l4.StrokeThickness = 0.5;
            Line l5 = new Line() { X1 = left - offset - extend, X2 = left - extend, Y1 = top + l / scale, Y2 = top + l / scale };
            l5.Stroke = Brushes.Black;
            l5.StrokeThickness = 0.5;
            TextBlock text = new TextBlock();
            text.Text = t;
            text.FontSize = font;
            text.Foreground = Brushes.Black;
            text.LayoutTransform = new RotateTransform(90, 25, 25);
            Canvas.SetTop(text, top + l / (2 * scale) - font);
            Canvas.SetLeft(text, left - offset);
            canvas.Children.Add(l1);
            canvas.Children.Add(l2);
            canvas.Children.Add(l3);
            canvas.Children.Add(l4);
            canvas.Children.Add(l5);
            canvas.Children.Add(text);

        }
    }
}
