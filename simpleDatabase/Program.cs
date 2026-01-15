using System;
using System.IO;
using System.Text;

string path = "test1.txt";
 string isiFile = "Halo, ini isi file!\nBaris kedua.";

 FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);;

byte[] data = new byte[fs.Length];

fs.Read(data, 0, data.Length);

Console.WriteLine($"panjang byte {data.Length}");

        // Tutup file
fs.Close();

string outputFile = Encoding.UTF8.GetString(data);

Console.WriteLine($"isi file adalah : {outputFile}");
