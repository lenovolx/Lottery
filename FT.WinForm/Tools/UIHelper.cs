using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace FT.WinForm
{
    public static class UIHelper
    {
        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static List<T> GetChildObjects<T>(DependencyObject obj, Type typename) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).GetType() == typename))
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects<T>(child, typename));
            }
            return childList;
        }

        public static List<T> GetChildObjectsLogic<T>(DependencyObject obj, Type typename) where T : FrameworkElement
        {
            //DependencyObject child = null;
            List<T> childList = new List<T>();

            foreach (object child in LogicalTreeHelper.GetChildren(obj))
            {
                if (child is T && (((T)child).GetType() == typename))
                {
                    childList.Add((T)child);
                }
                if (child is DependencyObject)
                {
                    childList.AddRange(GetChildObjectsLogic<T>((DependencyObject)child, typename));
                }
            }
            return childList;
        }

        public static ToggleButton GetCheckedChild(this Panel me)
        {
            foreach (var c in me.Children)
            {
                if (c is ToggleButton)
                {
                    if ((c as ToggleButton).IsChecked == true)
                        return c as ToggleButton;
                }
            }
            return null;
        }

        public static void CheckChild(this Panel me, Func<ToggleButton, bool> predicate)
        {
            foreach (var c in me.Children)
            {
                if (c is ToggleButton && predicate(c as ToggleButton))
                {
                    (c as ToggleButton).IsChecked = true;
                    return;
                }
            }
        }

        public static void ActLikeRadioButtonGroup(this Panel me)
        {
            RoutedEventHandler checkboxChecked = (RoutedEventHandler)((object sender, RoutedEventArgs e) =>
            {
                foreach (var c in me.Children)
                {
                    if (c == e.Source) continue;
                    if (c is CheckBox) (c as CheckBox).IsChecked = false;
                }
            });
            me.AddHandler(CheckBox.CheckedEvent, checkboxChecked);
        }

        public static T GetVisualParent<T>(this DependencyObject me) where T : Visual
        {
            DependencyObject p = VisualTreeHelper.GetParent(me);
            if (p == null) return null;
            while (!(p is T))
            {
                p = VisualTreeHelper.GetParent(p);
                if (p == null) return null;
            }
            return p as T;
        }
        public static void UnCheckToggleButton(DependencyObject obj)
        {
            List<ToggleButton> listButtons = UIHelper.GetChildObjects<ToggleButton>(obj, typeof(ToggleButton));
            foreach (ToggleButton tbtn in listButtons)
            {
                //排除掉Expander内置的Toggle
                if (tbtn.IsChecked == true && tbtn.Name != "ToggleSite")
                    tbtn.IsChecked = false;
            }
        }
    }
}
