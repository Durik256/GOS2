using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BP3toBMP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
                Environment.Exit(0);
            
            Console.WriteLine(args[0]);
            string outFile = args[0].Replace(".bp3", ".bmp").Replace(".BP3", ".bmp");

            using (var stream = File.Open(args[0], FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    uint magic = reader.ReadUInt32();
                    if (magic != 2290649224)
                    {
                        Console.WriteLine("File not *.BP3! magic: [" + magic + "] need [2290649224]...");
                        Environment.Exit(0);
                    }

                    int width = reader.ReadInt32();
                    int height = reader.ReadInt32();
                    int sizeBMP = reader.ReadInt32();
                    stream.Seek(26, SeekOrigin.Begin);
                    int sizeHeader = reader.ReadInt32();
                    stream.Seek(16, SeekOrigin.Begin);
                    byte[] bmpHeader = reader.ReadBytes(sizeHeader);

                    int wt = Calc(width, 8);
                    int ht = Calc(height, 8);

                    int Size = ht * wt / 64;

                    byte[] lpMem = reader.ReadBytes(Size);
                    byte[] cmMem = reader.ReadBytes(Size*3);

                    byte[] pixelData = new byte[Size * 192];

                    for (int i = 0; i < Size; i++)
                    {
                        int chunkY = 8;
                        if (8 * (i / (wt / 8)) + 8 >= height)
                            chunkY = height + 8 - ht;

                        int chunkX = 8;
                        if (8 * (i % (wt / 8)) + 8 >= width)
                            chunkX = width + 8 - wt;

                        int bitCount = 0;
                        switch (lpMem[i])
                        {
                            case 0:
                                bitCount = 0;
                                break;
                            case 1:
                                bitCount = 8;
                                break;
                            case 2:
                                bitCount = 8;
                                break;
                            case 3:
                                bitCount = 8;
                                break;
                            case 4:
                                bitCount = 4;
                                break;
                            case 5:
                                bitCount = 8;
                                break;
                            case 6:
                                bitCount = 16;
                                break;
                            case 7:
                                bitCount = 24;
                                break;
                        }
                        int v24 = bitCount * chunkX / 8;

                        List<byte> chunkPixel = new List<byte>();
                        if (bitCount > 0)
                        {
                            for (int cY = 0; cY < chunkY; cY++) {
                                chunkPixel.AddRange(reader.ReadBytes((bitCount * chunkX) / 8));
                                chunkPixel.AddRange(new byte[24 - (bitCount * chunkX / 8)]);
                            }
                            chunkPixel.AddRange(new byte[192 - (chunkPixel.Count)]);
                        }
                        int v20 = 3 * wt;
                        int v23 = 3 * wt * 8 * (i / (wt / 8));
                        int v51 = 24 * (i % (wt / 8));
                        int v61 = bitCount / 8;
                        int v26 = 0;
                        for (int e = 0; e < 8; e++)
                        {
                            int v48 = v51 + v23;
                            int v56 = v26;
                            for (int m = 0; m < 8; m++)
                            {
                                switch (lpMem[i])
                                {
                                    case 0://0bit
                                        pixelData[v48] = cmMem[3 * i];
                                        pixelData[v48 + 1] = cmMem[3 * i + 1];
                                        pixelData[v48 + 2] = cmMem[3 * i + 2];
                                        break;
                                    case 1://8bit
                                        pixelData[v48] = (byte)((chunkPixel[v56] & 7) + cmMem[3 * i]);
                                        pixelData[v48 + 1] = (byte)((((int)chunkPixel[v56] >> 3) & 7) + cmMem[3 * i + 1]);
                                        pixelData[v48 + 2] = (byte)((((int)chunkPixel[v56] >> 6) &3) + cmMem[3 * i + 2]);
                                        break;
                                    case 2://8bit
                                        pixelData[v48] = (byte)((chunkPixel[v56] & 3) + cmMem[3 * i]);
                                        pixelData[v48 + 1] = (byte)((((int)chunkPixel[v56] >> 2) & 7) + cmMem[3 * i + 1]);
                                        pixelData[v48 + 2] = (byte)((((int)chunkPixel[v56] >> 5) &7) + cmMem[3 * i + 2]);
                                        break;
                                    case 3://8bit
                                        pixelData[v48] = (byte)((chunkPixel[v56] & 7) + cmMem[3 * i]);
                                        pixelData[v48 + 1] = (byte)((((int)chunkPixel[v56] >> 3) & 3) + cmMem[3 * i + 1]);
                                        pixelData[v48 + 2] = (byte)((((int)chunkPixel[v56] >> 5) &7) + cmMem[3 * i + 2]);
                                        break;
                                    case 4://4bit
                                        if (m % 2 > 0)
                                        {
                                            pixelData[v48] = (byte)((((int)chunkPixel[v56] >> 4) & 0xF) + cmMem[3 * i]);
                                            pixelData[v48 + 1] = (byte)((((int)chunkPixel[v56] >> 4) & 0xF) + cmMem[3 * i + 1]);
                                            pixelData[v48 + 2] = (byte)((((int)chunkPixel[v56] >> 4) & 0xF) + cmMem[3 * i + 2]);
                                        }
                                        else
                                        {
                                            pixelData[v48] = (byte)((chunkPixel[v56] & 0xF) + cmMem[3 * i]);
                                            pixelData[v48 + 1] = (byte)((chunkPixel[v56] & 0xF) + cmMem[3 * i + 1]);
                                            pixelData[v48 + 2] = (byte)((chunkPixel[v56] & 0xF) + cmMem[3 * i + 2]);
                                        }
                                        break;
                                    case 5://8bit
                                        pixelData[v48] = chunkPixel[v56];
                                        pixelData[v48 + 1] = chunkPixel[v56];
                                        pixelData[v48 + 2] = chunkPixel[v56];
                                        break;
                                    case 6://16bit
                                        pixelData[v48] = (byte)((chunkPixel[v56] & 0x1F) + cmMem[3 * i]);
                                        pixelData[v48 + 1] = (byte)(((chunkPixel[v56] & 0xE0) >> 5) + 8 * (chunkPixel[v56 + 1] & 3) + cmMem[3 * i + 1]);
                                        pixelData[v48 + 2] = (byte)(((chunkPixel[v56 + 1] & 0x7C) >> 2) + cmMem[3 * i + 2]);
                                        break;
                                    case 7://24bit
                                        pixelData[v48] = chunkPixel[v56];
                                        pixelData[v48 + 1] = chunkPixel[v56 + 1];
                                        pixelData[v48 + 2] = chunkPixel[v56 + 2];
                                        break;
                                    default:
                                        break;
                                }
                                v56 += v61;
                                v48 += 3;
                            }
                            v26 += bitCount;
                            v23 += v20;
                        }
                    }

                    using (FileStream OutStream = new FileStream(outFile, FileMode.Create))
                    {
                        using (BinaryWriter binaryWriter = new BinaryWriter(OutStream))
                        {
                            binaryWriter.Write(bmpHeader);
                            int v20 = 3 * wt;
                            int remainder = Calc(3 * width, 4) - 3 * width;
                            for (int n = 0; n < height; n++)
                            {
                                binaryWriter.Write(pixelData, v20 * n, 3 * width);
                                if (remainder > 0)
                                    binaryWriter.Write(new byte[remainder]);
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Done");
            //Console.ReadKey();
        }

        public static int Calc(int a1, int a2)
        {
            if (a1 % a2 != 0)
                return a2 + a1 - a1 % a2;
            else
                return a1;
        }
    }
}
