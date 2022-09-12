using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WolvenManager.UI.Editors
{
  /// <summary>
  /// Interaction logic for EnumArrayEditor.xaml
  /// </summary>
  public partial class EnumArrayEditor : UserControl
  {
    public EnumArrayEditor()
    {
      InitializeComponent();
    }

    public IEnumerable ItemsSource
    {
      get => (IEnumerable)this.GetValue(ItemsSourceProperty);
      set => this.SetValue(ItemsSourceProperty, value);
    }
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource), typeof(IEnumerable), typeof(PathEditorView), new PropertyMetadata(null));

    public ObservableCollection<object> SelectedItems
    {
      get => (ObservableCollection<object>)this.GetValue(SelectedItemsProperty);
      set => this.SetValue(SelectedItemsProperty, value);
    }
    public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
        nameof(SelectedItems), typeof(ObservableCollection<object>), typeof(PathEditorView), new PropertyMetadata(null));


  }
}
