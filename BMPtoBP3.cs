using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = System.IO.File;

namespace BMPtoBP3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine(args[0]);
                using (var stream = File.Open(args[0], FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        string magic = new string(reader.ReadChars(2));
                        if (magic != "BM")
                        {
                            Console.WriteLine("File not *.BMP! magic: [" + magic + "] need [BM]...");
                            Environment.Exit(0);
                        }
                        stream.Seek(2, SeekOrigin.Begin);
                        int sizeFile = reader.ReadInt32();
                        stream.Seek(10, SeekOrigin.Begin);
                        int sizeHeader = reader.ReadInt32();
                        stream.Seek(4, SeekOrigin.Current);
                        int width = reader.ReadInt32();
                        int height = reader.ReadInt32();
                        stream.Seek(2, SeekOrigin.Current);
                        int BitCount = reader.ReadInt32();

                        if (BitCount != 24)
                        {
                            Console.WriteLine("This BitCount dont support: [" + BitCount + "] need [24]...");
                            Environment.Exit(0);
                        }

                        string outFile = args[0].Replace(".bmp", ".bp3").Replace(".BMP", ".bp3");

                        stream.Seek(0, SeekOrigin.Begin);
                        byte[] bmpHeader = reader.ReadBytes(sizeHeader);

                        int wt = calc(width, 8);
                        int ht = calc(height, 8);
                        int Size = ht * wt / 64;

                        int v12 = 3 * wt;
                        int v14 = wt / 8;

                        List<byte> pixelData = new List<byte>();

                        int v8 = calc(3 * width, 4);
                        for (int i = 0; i < height; i++)
                        {
                            pixelData.AddRange(reader.ReadBytes(v8));
                            pixelData.AddRange(new byte[v12 - v8]);
                        }
                        
                        using (FileStream OutStream = new FileStream(outFile, FileMode.Create))
                        {
                            using (BinaryWriter binaryWriter = new BinaryWriter(OutStream))
                            {
                                binaryWriter.Write(-2004318072);//BP3 magic
                                binaryWriter.Write(width);
                                binaryWriter.Write(height);
                                binaryWriter.Write(sizeFile);
                                binaryWriter.Write(bmpHeader);
                                for (int y = 0; y < Size; y++)
                                    binaryWriter.Write((byte)7);
                                binaryWriter.Write(new byte[Size*3]);

                                for (int j = 0; j < Size; j++)
                                {
                                    int v34 = 8;
                                    if (8 * (j / (wt / 8)) + 8 >= height)
                                        v34 = height + 8 - ht;

                                    int v36 = 8;
                                    if (8 * (j % (wt / 8)) + 8 >= width)
                                        v36 = width + 8 - wt;

                                    int v13 = v12 * 8 * (j / v14);
                                    int v37 = 24 * (j % v14);
                                    List<byte> v41 = new List<byte>();
                                    for (int k = 0; k < 8; k++)
                                    {
                                        int v35 = v37 + v13;
                                        for (int m = 0; m < 8; m++)
                                        {
                                            if (k < v34 && m < v36)
                                            {
                                                v41.Add(pixelData[v35]);
                                                v41.Add(pixelData[v35 + 1]);
                                                v41.Add(pixelData[v35 + 2]);
                                            }

                                            v35 += 3;
                                        }
                                        v13 += v12;
                                     }
                                    binaryWriter.Write(v41.ToArray());
                                }
                            }
                        }

                    }
                }
            }
                
            Console.WriteLine("Done");
            //Console.ReadKey();
        }

        public static int calc(int a1, int a2)
        {
            if (a1 % a2 != 0)
                return a2 + a1 - a1 % a2;
            else
                return a1;
        }
    }
}
