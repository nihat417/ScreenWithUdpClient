using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace Client;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private IPAddress ip;
    private int port;
    UdpClient udpClient;
    IPEndPoint remoteEp;
 


    public MainWindow()
    {
        InitializeComponent();
        ClientThings();
    }


    public void ClientThings()
    {
        udpClient = new UdpClient();
        ip = IPAddress.Parse("127.0.0.1");
        port = 45678;
        remoteEp = new IPEndPoint(ip, port);
    }

    private async void Screen_Click(object sender, RoutedEventArgs e)
    {
        var size = ushort.MaxValue - 29;
        var buffer = new byte[size];
        await udpClient.SendAsync(buffer, buffer.Length,remoteEp);
        List<byte> list = new List<byte>();

        var len = 0;



        do
        {
            try
            {
                var receiveResult = await udpClient.ReceiveAsync();
                buffer = receiveResult.Buffer;
                len = buffer.Length;
                list.AddRange(buffer);

                Imagephto.Source = GetImage(list.ToArray());
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        } while (len == buffer.Length);
        
        list.Clear();
    }


    private static BitmapImage GetImage(byte[] imageInfo)
    {
        var image = new BitmapImage();

        using (var memoryStream = new MemoryStream(imageInfo))
        {
            memoryStream.Position = 0;

            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = memoryStream;
            image.EndInit();
        }

        image.Freeze();
        return image;
    }
}
