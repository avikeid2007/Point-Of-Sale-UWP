using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DataAdminSettings : Page
    {
        public DataAdminSettings()
        {
            this.InitializeComponent();
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile newFile = await folder.GetFileAsync("logo.png");
                var bitmapImage = new BitmapImage();

                bitmapImage.SetSource(await newFile.OpenAsync(FileAccessMode.Read));
                ImageViewer.Source = bitmapImage;
            }
            catch
            {
                removeLogo.Visibility = Visibility.Collapsed;
            }

            
            
            


        }


        private async void dataFilePicker_Click(object sender, RoutedEventArgs e)// adds loctaion link to textbox
        {

            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                this.dataLocationLink.Text = folder.Path;
            }

        }

        private async void logoOpen_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                var storageFile = file as StorageFile;
                var contentType = storageFile.ContentType;
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                if (contentType == "image/png" || contentType == "image/jpeg" || contentType == "image/jpg")

                {

                    StorageFile newFile = await storageFile.CopyAsync(folder,"logo.png" , NameCollisionOption.ReplaceExisting);

                    var bitmapImage = new BitmapImage();

                    bitmapImage.SetSource(await storageFile.OpenAsync(FileAccessMode.Read));
                    ImageViewer.Source = bitmapImage;
                    removeLogo.Visibility = Visibility.Visible;
                }
            }
            else
            {
                //this.textBlock.Text = "Operation cancelled.";
            }
            

        }

        private async void removeLogo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile image = await folder.GetFileAsync("logo.png");
            await image.DeleteAsync();
            removeLogo.Visibility = Visibility.Collapsed;

            var bitmapImage = new BitmapImage();
            ImageViewer.Source = bitmapImage;

        }
    }
}
