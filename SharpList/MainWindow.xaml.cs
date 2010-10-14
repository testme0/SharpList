using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Business;

namespace SharpList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SharpListManager _manager;

        private Business.SharpList currentSharpList;

        public MainWindow()
        {
            InitializeComponent();

            _manager = new SharpListManager();
            sharpListList.ItemsSource = _manager.GetSharpLists();
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            _manager.CreateSharpList();

            sharpListList.ItemsSource = null;
            sharpListList.ItemsSource = _manager.GetSharpLists();
            sharpListList.SelectedIndex = _manager.GetSharpLists().Count - 1;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentSharpList != null)
            {
                _manager.DeleteSharpList(currentSharpList);

                sharpListList.ItemsSource = null;
                sharpListList.ItemsSource = _manager.GetSharpLists();
                itemsList.ItemsSource = null;
                nameTextBox.Text = "";
            }
        }

        private void sharpListList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentSharpList = ((sender as ListBox).SelectedItem as Business.SharpList);
            if (currentSharpList != null)
            {
                nameTextBox.Text = currentSharpList.Name;
                itemsList.ItemsSource = currentSharpList.Items;
            }
        }

        private void check_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Business.SharpItem item = (sender as Image).DataContext as Business.SharpItem;
            if (item != null)
            {
                item.Checked = !item.Checked;

                itemsList.ItemsSource = null;
                itemsList.ItemsSource = currentSharpList.Items;
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameTextBox.Text.Length > 0 && nameTextBox.Text.Length < 31)
            {
                currentSharpList.Name = nameTextBox.Text;
                int index = _manager.GetSharpLists().IndexOf(currentSharpList);

                sharpListList.ItemsSource = null;
                sharpListList.ItemsSource = _manager.GetSharpLists();
                sharpListList.SelectedIndex = index;
            }
            else
            {
                ErrorBox errorBox = new ErrorBox("Name length must be between 1 and 30 !");
                errorBox.ShowDialog();
            }
        }

        private void addItem(string name)
        {
            if (currentSharpList != null)
            {
                Business.SharpItem newItem = new Business.SharpItem(name);

                currentSharpList.Items.Add(newItem);

                itemsList.ItemsSource = null;
                itemsList.ItemsSource = currentSharpList.Items;
            }
        }

        private void deleteItem(Business.SharpItem item)
        {
            if (item != null)
            {
                currentSharpList.Items.Remove(item);

                itemsList.ItemsSource = null;
                itemsList.ItemsSource = currentSharpList.Items;
            }
        }

        private void MouseMoveSharpListItem(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && itemsList.SelectedItem != null)
            {
                DragDropEffects effects;
                DataObject obj = new DataObject();
                obj.SetData(typeof(Business.SharpItem), itemsList.SelectedItem as Business.SharpItem);
                effects = DragDrop.DoDragDrop(itemsList, obj, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        private void SharpListDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Business.SharpItem)))
            {
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri("pack://application:,,,/SharpList;component/Img/delete_2.png");
                logo.EndInit();
                deleteImage.Source = logo;
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void SharpListDragLeave(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Business.SharpItem)))
            {
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri("pack://application:,,,/SharpList;component/Img/delete.png");
                logo.EndInit();
                deleteImage.Source = logo;
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void SharpListDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Business.SharpItem)))
            {
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri("pack://application:,,,/SharpList;component/Img/delete.png");
                logo.EndInit();
                deleteImage.Source = logo;
                e.Effects = DragDropEffects.Copy;
                deleteItem(e.Data.GetData(typeof(Business.SharpItem)) as Business.SharpItem);
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void addButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sharpListList.SelectedItem != null)
            {
                DialogBox dialogBox = new DialogBox();
                if (dialogBox.ShowDialog() == false)
                {
                    if (dialogBox.validated)
                    {
                        addItem(dialogBox.nameTextBox.Text);
                    }
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _manager.Persist();
        }
    }
}
