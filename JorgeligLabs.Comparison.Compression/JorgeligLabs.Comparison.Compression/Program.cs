using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Ionic.Zlib;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using CompressionLevel = Ionic.Zlib.CompressionLevel;
using CompressionMode = Ionic.Zlib.CompressionMode;

namespace JorgeligLabs.Comparison.Compression
{
  class Program
  {
    //private static readonly string filePath = "http://ftp.esrf.eu/pub/dinosaur/VIDEOS/VIDEOS%20MP4/ESRF_Fossil_Report.mp4";
    //private static readonly string filePath = "http://ftp.esrf.eu/pub/dinosaur/VIDEOS/VIDEOS%20MP4/Dennis%20VOETEN_Dutch.mp4";
    private static readonly string filePath = "https://ftp3.syscom.mx/usuarios/ftp/single_page/rfelements/q4bPfwUR-23004299.mp4";

    static void Main(string[] args)
    {
      Console.WriteLine("Downloading test file...");
      WebClient wc = new WebClient();
      byte[] initialData = wc.DownloadData(filePath);
      wc.Dispose();
      Console.WriteLine("Initial size: " + initialData.Length.ToString());
      Console.WriteLine("Times are in ms. Sizes are in bytes.");
      



      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      MemoryStream ms1 = new MemoryStream();
      using ZlibStream iStream = new ZlibStream(ms1, CompressionMode.Compress, CompressionLevel.Level7);
      iStream.Write(initialData, 0, initialData.Length);
      byte[] result1 = ms1.ToArray();
      stopwatch.Stop();
      Console.WriteLine("\n");
      
      Console.WriteLine("=====================================");
      Console.WriteLine($"Compress with DotNetZip: {stopwatch.ElapsedMilliseconds} milliseconds");
      Console.WriteLine($"Compressed size: {result1.Length}");
      Console.WriteLine($"Ratio: {(float)result1.Length / (float)initialData.Length}");
      Console.WriteLine("=====================================\n\n");
      stopwatch.Reset();
      

      //Tutorial: https://www.carlrippon.com/zipping-up-files-from-a-memorystream/
      stopwatch.Start();
      MemoryStream ms2 = new MemoryStream();
      var archive = new ZipArchive(ms2, ZipArchiveMode.Create, true);
      var zipArchiveEntry = archive.CreateEntry("file", System.IO.Compression.CompressionLevel.Optimal);
      var originalFileMemory = new MemoryStream(initialData);
      var entryStream = zipArchiveEntry.Open();
      originalFileMemory.CopyTo(entryStream);
      var result2 = ms2.ToArray();
      stopwatch.Stop();
      
      Console.WriteLine("======================================");
      Console.WriteLine($"Compress with IO.Compression: {stopwatch.ElapsedMilliseconds} milliseconds");
      Console.WriteLine($"Compressed size: {result2.Length}");
      Console.WriteLine($"Ratio: {(float)result2.Length / (float)initialData.Length}");
      Console.WriteLine("=====================================\n\n");
      stopwatch.Reset();

      stopwatch.Start();
      var result3 = new MemoryStream();
      var zipStream = new ZipOutputStream(result3);
      zipStream.SetLevel(7);
      var zipEntry = new ZipEntry("foo");
      zipStream.PutNextEntry(zipEntry);

      StreamUtils.Copy(new MemoryStream(initialData),zipStream, new byte[4096]);
      zipStream.CloseEntry();
      zipStream.IsStreamOwner = false;
      result3.Position = 0;
      stopwatch.Stop();
      
      Console.WriteLine("=====================================");
      Console.WriteLine($"Compress with SharpZipLib: {stopwatch.ElapsedMilliseconds} milliseconds");
      Console.WriteLine($"Compressed size: {result3.Length}");
      Console.WriteLine($"Ratio: {(float)result3.Length / (float)initialData.Length}");
      Console.WriteLine("=====================================\n\n");
      stopwatch.Reset();


    }
  }
}
