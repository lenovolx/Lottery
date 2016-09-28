using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FT.WinForm
{
    public class GridHelper
    {
        public static bool GetShowBorder(DependencyObject obj)  
        {  
            return (bool)obj.GetValue(ShowBorderProperty);  
        }  
  
        public static void SetShowBorder(DependencyObject obj, bool value)  
        {  
            obj.SetValue(ShowBorderProperty, value);  
        }  
  
        public static readonly DependencyProperty ShowBorderProperty =  
            DependencyProperty.RegisterAttached("ShowBorder", typeof(bool), typeof(GridHelper), new PropertyMetadata(OnShowBorderChanged));  
  
  
        //这是一个事件处理程序，需要手工编写，必须是静态方法  
        private static void OnShowBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)  
        {  
            var grid = d as Grid;  
            if((bool)e.OldValue)  
            {  
                grid.Loaded -= (s, arg) => { };  
            }  
            if((bool)e.NewValue)  
            {  
                grid.Loaded += (s, arg) =>  
                {  
                    //这种做法自动将控件移动到Border里面来  
                    var controls = grid.Children;  
                    var count = controls.Count;  
                      
                    for(int i = 0; i < count; i++)  
                    {  
                        var item = controls[i] as FrameworkElement;
                        if (item.Tag == "border_line") return;
                        var row = Grid.GetRow(item);  
                        var column = Grid.GetColumn(item);  
                        var rowspan = Grid.GetRowSpan(item);  
                        var columnspan = Grid.GetColumnSpan(item);  
  
                        Border border = new Border();
                        border.Tag = "border_line";
                        border.BorderBrush = new SolidColorBrush(Colors.LightGray);  
  
                        //保证边框线条宽度统一  
                        if ((0 == (int)row) && (0 == (int)column))  
                        {  
                            //第一行第一列  
                            border.BorderThickness = new Thickness(1, 0, 1, 1);  
                        }  
                        else if (0 == (int)row)  
                        {  
                            //第一行  
                            border.BorderThickness = new Thickness(0, 0, 1, 1);  
                        }  
                        else if (0 == (int)column)  
                        {  
                            //第一列  
                            border.BorderThickness = new Thickness(1, 0, 1, 1);  
                        }  
                        else  
                        {  
                            //其它行列,即除去第一行,第一列以为的所有单元格  
                            border.BorderThickness = new Thickness(0, 0, 1, 1);  
                        }  
                        border.Padding = new Thickness(0);  
                        //{  
                        //    BorderBrush = new SolidColorBrush(Colors.LightGray),  
                        //    BorderThickness = new Thickness(1),  
                        //    Padding = new Thickness(2)  
                        //};  
                          
                        Grid.SetRow(border, row);  
                        Grid.SetColumn(border, column);  
                        Grid.SetRowSpan(border, rowspan);  
                        Grid.SetColumnSpan(border, columnspan);
                        
                        grid.Children.RemoveAt(i);
                        border.Child = item;  
                        grid.Children.Insert(i, border);  
                    }
                    grid.Loaded -= (s1, arg1) => { };  
                };  
            }  
        }
    }
}
