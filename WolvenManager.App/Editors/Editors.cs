using System;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using MS.WindowsAPICodePack.Internal;
using ReactiveUI;
using Splat;
using Syncfusion.Windows.Controls.Input;
using Syncfusion.Windows.PropertyGrid;
using Syncfusion.Windows.Tools.Controls;
using WolvenKit.Common;
using WolvenManager.App.ViewModels.PageViewModels;
using PropertyItem = Syncfusion.Windows.PropertyGrid.PropertyItem;

namespace WolvenManager.App.Editors
{
    //Custom Editor for folder type properties.
    public class EnumArrayEditor<T> : ITypeEditor where T : Enum
    {
        private ComboBoxAdv _wrappedControl;
        public void Attach(PropertyViewItem property, PropertyItem info)
        {
            if (info.CanWrite)
            {
                var binding = new Binding("Value")
                {
                    Mode = BindingMode.TwoWay,
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(_wrappedControl, ComboBoxAdv.SelectedItemsProperty, binding);
            }
            else
            {
                _wrappedControl.IsEnabled = false;
                var binding = new Binding("Value")
                {
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(_wrappedControl, ComboBoxAdv.SelectedItemsProperty, binding);
            }
        }

        public object Create(PropertyInfo propertyInfo)
        {
            var type = typeof(T);
            var vals = type.GetEnumValues();
            var values = vals.OfType<T>();

            _wrappedControl = new ComboBoxAdv()
            {
                AllowMultiSelect = true,
                ItemsSource = values.ToList()
            };
            return _wrappedControl;
        }
        public void Detach(PropertyViewItem property) { }
    }


    //Custom Editor for folder type properties.
    public abstract class FolderPathEditorBase : ITypeEditor
    {
        protected PathEditorView _wrappedControl;
        public void Attach(PropertyViewItem property, PropertyItem info)
        {
            if (info.CanWrite)
            {
                var binding = new Binding("Value")
                {
                    Mode = BindingMode.TwoWay,
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(_wrappedControl, PathEditorView.TextProperty, binding);
            }
            else
            {
                _wrappedControl.IsEnabled = false;
                var binding = new Binding("Value")
                {
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(_wrappedControl, PathEditorView.TextProperty, binding);
            }
        }

        public abstract object Create(PropertyInfo propertyInfo);
        public void Detach(PropertyViewItem property) { }
    }


    //Custom Editor for folder type properties.
    public class MultiFolderPathEditor : FolderPathEditorBase
    {
        public override object Create(PropertyInfo propertyInfo)
        {
            _wrappedControl = new PathEditorView(true, true);
            return _wrappedControl;
        }
    }

    //Custom Editor for folder type properties.
    public class SingleFolderPathEditor : FolderPathEditorBase
    {
        public override object Create(PropertyInfo propertyInfo)
        {
            _wrappedControl = new PathEditorView(true, false);
            return _wrappedControl;
        }
    }

    //Custom Editor for file type properties.
    public class MultiFilePathEditor : FolderPathEditorBase
    {
        public override object Create(PropertyInfo propertyInfo)
        {
            _wrappedControl = new PathEditorView(false, true);
            return _wrappedControl;
        }
    }

    //Custom Editor for file type properties.
    public class SingleFilePathEditor : FolderPathEditorBase
    {
        public override object Create(PropertyInfo propertyInfo)
        {
            _wrappedControl = new PathEditorView(false, false);
            return _wrappedControl;
        }
    }

}
