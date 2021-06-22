using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using ReactiveUI;

namespace WolvenManager.App.Editors
{
    /// <summary>
    /// Interaction logic for PathEditorView.xaml
    /// </summary>
    public partial class PathEditorView : UserControl
    {
        private readonly bool _isFolderPicker;
        private readonly bool _multiselect;

        public PathEditorView(bool isFolderPicker, bool multiselect)
        {
            InitializeComponent();

            _isFolderPicker = isFolderPicker;
            _multiselect = multiselect;
        }


        public string Text
        {
            get => (string)this.GetValue(TextProperty);
            set => this.SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(PathEditorView), new PropertyMetadata(""));


        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog
            {
                AllowNonFileSystemItems = true, 
                Multiselect = _multiselect, 
                IsFolderPicker = _isFolderPicker, 
                
                Title = "Select files or folders"
            };
            if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            var results = dlg.FileNames;
            if (results == null)
            {
                return;
            }

            Text = "";
            foreach (var s in results)
            {
                Text += $"\"{s}\"";
            }

        }
    }
}
