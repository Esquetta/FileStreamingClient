

using FileStreaming;
using Google.Protobuf;
using Grpc.Net.Client;
using FileInfo = FileStreaming.FileInfo;

var channel = GrpcChannel.ForAddress("https://localhost:7036");
var client = new FileService.FileServiceClient(channel);






string file = @"C:\Users\fb_52\Downloads\Furkan.Taşçı (2).pdf";

//Stream yapılacak dosya belirleniyor

using FileStream fileStream = new FileStream(file, FileMode.Open);

//Dosyanın tüm bilgileri ediniliyor. Bu nesne ile stream birlikte gönderilecektir.

var content = new BytesContent
{
    FileSize = fileStream.Length,
    ReadedByte = 0,
    Info = new FileStreaming.FileInfo { FileName = "Furkan.Taşçı (2)", FileExtension = ".pdf" }
};

//Stream için server'da ki FileUpload fonksiyonu çağrılıyor. 

var upload = client.FileUpload();

//Akışta ne kadar parça gideceği önceden ayarlanıyor. Burada 2048'lik bir alan tahsis edilmektedir. Gönderilecek dosyanın boyutu ne olursa olsun en fazla 2048'lik parça gönderilebileceğinden dolayı bu şekilde ayarlanmıştır.
byte[] buffer = new byte[2048];


//Her bir buffer, 0. byte'tan itibaren 2048 adet okunmakta ve sonuç 'content.ReadedByte'a atanmaktadır.
while ((content.ReadedByte = fileStream.Read(buffer, 0, buffer.Length)) > 0)
{
    //Okunan buffer'ın stream edilebilmesi için 'message.proto' dosyasındaki 'bytes' türüne dönüştürülüyor.
    content.Buffer = ByteString.CopyFrom(buffer);
    //'BytesContent' nesnesi stream olarak gönderiliyor.
    await upload.RequestStream.WriteAsync(content);
}

await upload.RequestStream.CompleteAsync();

fileStream.Close();



//File Download


//string downLoadPath = @"D:\\Workstation\\mvc\\LastDance\\C\\gRPC\\FileStreaming\\FileStreaming.csproj";

//var fileInfo = new FileInfo
//{
//    FileExtension = ".pdf",
//    FileName = "Furkan.Taşçı (2).pdf"
//};


//FileStream fileDownloadStream = null;


////Serverden ilgili FileInfo ile talep yapılıyor


//var download = client.FileDownload(fileInfo);

//CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

//int count = 0;

//decimal chuckSize = 0;

////Talep Neticesinde stream olarak gelen dosya parçaları okunmaya başlanıyor
//while (await download.ResponseStream.MoveNext(cancellationTokenSource.Token))
//{
//    //İlk gelen parçada transfer edilen dosyanın ana hatları belirleniyor.
//    if (count++ == 0)
//    {
//        //Transfer edilen dosyanın server'dan gelen bilgiler eşliğide belirtilen dizinde depolanması için konfigürasyon gerçekleştiriliyor.
//        fileDownloadStream = new FileStream(@$"{downLoadPath}/{download.ResponseStream.Current.Info.FileName}/{download.ResponseStream.Current.Info.FileExtension}", FileMode.CreateNew);

//        //Depolanacak yerde dosya boyutu kadar alan tahsis ediliyor.
//        fileDownloadStream.SetLength(download.ResponseStream.Current.FileSize);

//    }

//    //'message.proto' dosyasında belirtilen 'bytes' türüne karşılık olarak 'BytesString' türünde gelen bufferlar  byte dizisine dönüştürülüyor.
//    var downloadBuffer = download.ResponseStream.Current.Buffer.ToByteArray();


//    //İlgili file stream parçaları yazdırılıyor
//    await fileStream.WriteAsync(downloadBuffer, 0, download.ResponseStream.Current.ReadedByte);

//    Console.WriteLine($"{Math.Round(((chuckSize += download.ResponseStream.Current.ReadedByte) * 100) / download.ResponseStream.Current.FileSize)}");
//}
//Console.WriteLine("Yüklendi...");
//await fileDownloadStream.DisposeAsync();

//fileDownloadStream.Close();



