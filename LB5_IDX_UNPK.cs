using System;
using System.Collections.Generic;
using System.IO;

namespace LB5_UNPK
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string extension = Path.GetExtension(args[0]);
                string lbFile = "";
                string idxFile = "";
                
                if (extension.ToLower() == ".lb5")
                {
                    lbFile = args[0];
                    idxFile = Path.ChangeExtension(args[0], ".idx");
                }  
                else if (extension.ToLower() == ".idx")
                {
                    idxFile = args[0];
                    lbFile = Path.ChangeExtension(args[0], ".lb5");
                }
                else
                    Environment.Exit(0);

                using (FileStream fsIDX = new FileStream(idxFile, FileMode.Open))
                {
                    using (BinaryReader rIDX = new BinaryReader(fsIDX))
                    {
                        int numFile = rIDX.ReadInt32();
                        using (FileStream fsLB = new FileStream(lbFile, FileMode.Open))
                        {
                            using (BinaryReader rLB = new BinaryReader(fsLB))
                            {
                                for (int i = 0; i < numFile; i++)
                                {
                                    int offset = rIDX.ReadInt32();
                                    int size = rIDX.ReadInt32();

                                    string name = new string(rIDX.ReadChars(16), 1, 15).Replace("\0", string.Empty);

                                    if (Path.GetExtension(name).ToLower() == ".bmp")
                                        name = Path.ChangeExtension(name, ".bp3");

                                    if (Path.GetExtension(name).ToLower() == ".txt")
                                        name = Path.ChangeExtension(name, ".tx3");

                                    Console.WriteLine(name + " offset:" + offset.ToString() + " size:" + size.ToString());

                                    fsLB.Seek(offset, SeekOrigin.Begin);
                                    byte[] Buffer = rLB.ReadBytes(size);
                                    File.WriteAllBytes(name, Buffer);
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("Done");
            }
        }
    }
}
