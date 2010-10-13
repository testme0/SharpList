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
using System.Xml;
using System.IO;
using System.Globalization;
using System.ComponentModel;

namespace SharpList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string path = "C:\\SharpList\\data.xml";
        private List<List> sharpLists;
        private List currentSharpList;
        private XmlDocument data;

        public MainWindow()
        {
            InitializeComponent();

            GetAllSharpLists();
        }

        public void GetAllSharpLists()
        {
            FileStream fileStream = null;
            data = new XmlDocument();

            try
            {
                fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                data.Load(fileStream);
            }
            catch (Exception e)
            {
                System.IO.Directory.CreateDirectory("C:\\SharpList");

                XmlDeclaration xmlDeclaration = data.CreateXmlDeclaration("1.0", "utf-8", null);

                // Create the root element
                XmlElement rootNode = data.CreateElement("Lists");
                data.InsertBefore(xmlDeclaration, data.DocumentElement);
                data.AppendChild(rootNode);
                fileStream = File.Create(path);
                data.Save(fileStream);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Flush();
                    fileStream.Close();
                    XmlNodeList xmlSharpLists = data.GetElementsByTagName("SharpList");
                    sharpLists = new List<List>();
                    for (int i = 0; i < xmlSharpLists.Count; i++)
                    {
                        XmlNode xmlSharpList = xmlSharpLists[i];

                        List sharpList = new List(xmlSharpList.Attributes[0].Value);

                        IFormatProvider culture = new CultureInfo("fr-FR", true);
                        sharpList.Date = DateTime.Parse(xmlSharpList.Attributes[1].Value,
                                           culture,
                                           DateTimeStyles.NoCurrentDateDefault);

                        XmlNodeList xmlItems = xmlSharpList.ChildNodes;

                        for (int j = 0; j < xmlItems.Count; j++)
                        {
                            XmlNode xmlItem = xmlItems[j];

                            Item item = new Item(xmlItem.Attributes[0].Value);
                            item.Checked = Boolean.Parse(xmlItem.InnerText);

                            sharpList.Items.Add(item);
                        }
                        sharpLists.Add(sharpList);
                    }
                    sharpListList.ItemsSource = sharpLists;
                }
            }
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            List newSharpList = new List("New");
            sharpLists.Add(newSharpList);

            sharpListList.ItemsSource = null;
            sharpListList.ItemsSource = sharpLists;
            sharpListList.SelectedIndex = sharpLists.IndexOf(newSharpList);
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentSharpList != null)
            {
                sharpLists.Remove(currentSharpList);

                sharpListList.ItemsSource = null;
                sharpListList.ItemsSource = sharpLists;
                itemsList.ItemsSource = null;
                nameTextBox.Text = "";
            }
        }

        private void sharpListList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentSharpList = ((sender as ListBox).SelectedItem as List);
            if (currentSharpList != null)
            {
                nameTextBox.Text = currentSharpList.Name;
                itemsList.ItemsSource = currentSharpList.Items;
            }
        }

        private void check_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Item item = (sender as Image).DataContext as Item;
            if (item != null)
            {
                item.Checked = !item.Checked;
                itemsList.ItemsSource = null;
                itemsList.ItemsSource = currentSharpList.Items;
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameTextBox.Text.Length < 31)
            {
                currentSharpList.Name = nameTextBox.Text;
                int index = sharpLists.IndexOf(currentSharpList);

                sharpListList.ItemsSource = null;
                sharpListList.ItemsSource = sharpLists;
                sharpListList.SelectedIndex = index;
            }
            else
            {
                MessageBox.Show(this, "Name length must be under 30 !");
            }
        }

        private void persist()
        {
            data = new XmlDocument();
            XmlDeclaration xmlDeclaration = data.CreateXmlDeclaration("1.0", "utf-8", null);

            XmlElement rootNode = data.CreateElement("Lists");
            data.InsertBefore(xmlDeclaration, data.DocumentElement);

            for (int i = 0; i < sharpLists.Count; i++)
            {
                List sharpList = sharpLists[i];
                XmlElement xmlSharpList = data.CreateElement("SharpList");

                XmlAttribute xmlSharpListName = data.CreateAttribute("name");
                xmlSharpListName.Value = sharpList.Name;
                XmlAttribute xmlSharpListDate = data.CreateAttribute("date");
                xmlSharpListDate.Value = String.Format("{0:dd/MM/yyyy HH:mm:ss}", sharpList.Date);

                xmlSharpList.SetAttributeNode(xmlSharpListName);
                xmlSharpList.SetAttributeNode(xmlSharpListDate);

                for (int j = 0; j < sharpList.Items.Count; j++)
                {
                    Item item = sharpList.Items[j];
                    XmlElement xmlItem = data.CreateElement("Item");

                    XmlAttribute xmlItemName = data.CreateAttribute("name");
                    xmlItemName.Value = item.Name;

                    xmlItem.SetAttributeNode(xmlItemName);
                    xmlItem.InnerText = item.Checked.ToString();

                    xmlSharpList.AppendChild(xmlItem);
                }

                rootNode.AppendChild(xmlSharpList);
            }

            data.AppendChild(rootNode);
            FileStream fileStream = File.Create(path);
            data.Save(fileStream);
            fileStream.Flush();
            fileStream.Close();
        }

        private void addItem(string name)
        {
            if (currentSharpList != null)
            {
                Item newItem = new Item(name);

                currentSharpList.Items.Add(newItem);

                itemsList.ItemsSource = null;
                itemsList.ItemsSource = currentSharpList.Items;
            }
        }

        private void deleteItem(Item item)
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
                obj.SetData(typeof(Item), itemsList.SelectedItem as Item);
                effects = DragDrop.DoDragDrop(itemsList, obj, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        private void SharpListDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Item)))
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
            if (e.Data.GetDataPresent(typeof(Item)))
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
            if (e.Data.GetDataPresent(typeof(Item)))
            {
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri("pack://application:,,,/SharpList;component/Img/delete.png");
                logo.EndInit();
                deleteImage.Source = logo;
                e.Effects = DragDropEffects.Copy;
                deleteItem(e.Data.GetData(typeof(Item)) as Item);
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
            persist();
        }
    }
}
