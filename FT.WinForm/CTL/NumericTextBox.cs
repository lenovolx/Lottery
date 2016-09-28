using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows;

namespace FT.WinForm.CTL
{
    public class NumericTextBox : TextBox
    {
        public NumericTextBox()
        {
            //必须禁用输入法, 否则TextChanged事件中修改由输入法输入的信息时会出现Cannot close undo unit because no opened unit exists的异常
            InputMethod.SetIsInputMethodEnabled(this, false);

            //防止黏贴剪切
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, null, CancelCommand));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, null, CancelCommand));
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);

            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                if (DataType == "int")
                {
                    int result;
                    if (!int.TryParse(this.Text.Replace(",", ""), out result) && this.Text != "-")
                    {
                        this.Text = this.Text.Remove(offset, change[0].AddedLength);
                        this.Select(offset, 0);
                    }
                    else
                    {
                        //转成有千位分隔符
                        this.Text = string.Format("{0:#,#}", result);
                        this.Select(this.Text.Length, 0);
                    }
                }
                else
                {
                    decimal result;
                    if (!decimal.TryParse(this.Text, out result) && this.Text != "-")
                    {
                        this.Text = this.Text.Remove(offset, change[0].AddedLength);
                        this.Select(offset, 0);
                    }
                    else
                    {
                    }
                }
            }
            base.OnTextChanged(e);
        }

        ////cancel the command
        private static void CancelCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            e.Handled = true;
        }


        /// <summary>
        /// 面板Code
        /// </summary>
        public static readonly DependencyProperty DataTypePropery = DependencyProperty.Register("DataType", typeof(string), typeof(NumericTextBox), new PropertyMetadata("int", new PropertyChangedCallback(OnDataTypeProperyChanged)));
        public string DataType
        {
            get { return GetValue(DataTypePropery).ToString(); }
            set { SetValue(DataTypePropery, value); }
        }
        private static void OnDataTypeProperyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //CustomDataGrid cdg = d as CustomDataGrid;
        }


        //protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        //{
        //    int result;
        //    if (!int.TryParse(e.Text, out result))
        //    {
        //        e.Handled = true;
        //        //this.Text = e.ControlText.Remove(e.ControlText.Length - e.Text.Length, e.Text.Length);
        //    }

        //    base.OnPreviewTextInput(e);

        //}

    }
}
