using System.Drawing;
using System.Net.Sockets;

var server = new UdpClient(45678);

while (true)
{
    UdpReceiveResult udpReceiveResult = await server.ReceiveAsync();
    var remoteEP=udpReceiveResult.RemoteEndPoint;

    var image = TakeScreenshot();
    var imageBytes = ImageToByte(image);
    var size = ushort.MaxValue - 29;


    foreach (var buffer in imageBytes.Chunk(size))
        await server.SendAsync(buffer, buffer.Length, remoteEP);


}


System.Drawing.Image TakeScreenshot()
{
    Bitmap bitmap = new Bitmap(1920, 1080);

    Graphics graphics = Graphics.FromImage(bitmap);
    graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

    return bitmap;
}

byte[] ImageToByte(System.Drawing.Image image)
{
    using (var stream = new MemoryStream())
    {
        image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

        return stream.ToArray();
    }
}