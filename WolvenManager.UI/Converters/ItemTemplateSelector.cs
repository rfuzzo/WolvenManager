using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.UI.Xaml.TreeView.Engine;

namespace WolvenManager.UI.Converters
{
    public class ItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RootTemplate { get; set; }
        public DataTemplate ChildTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is not TreeViewNode treeviewNode)
            {
                return null;
            }

            if (treeviewNode.Level == 0)
            {
                return RootTemplate;
            }
            else
            {
                return ChildTemplate;
            }
        }
    }
}
