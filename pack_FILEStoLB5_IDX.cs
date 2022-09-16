using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace TX3toTXT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                using (FileStream fsIDX = new FileStream("out.idx", FileMode.Create))
                {
                    using (BinaryWriter rIDX = new BinaryWriter(fsIDX))
                    {
                        rIDX.Write(args.Length);
                        using (FileStream fsLB = new FileStream("out.lb5", FileMode.Create))
                        {
                            using (BinaryWriter rLB = new BinaryWriter(fsLB))
                            {
                                int offset = 0;
                                for (int i = 0; i < args.Length; i++)
                                {
                                    byte[] Buffer = File.ReadAllBytes(args[i]);

                                    string name = Path.GetFileName(args[i]);
                                    string ext = Path.GetExtension(name).ToLower();
                                    Console.WriteLine(ext);

                                    if (ext == ".bp3")
                                        ext = ".bmp";

                                    if (ext == ".tx3")
                                        ext = ".txt";

                                    name = Path.ChangeExtension(name, ext);
                                    if (name.Length > 15)
                                        name = Path.GetFileNameWithoutExtension(name).Substring(0, 15 - ext.Length) + ext;

                                    rIDX.Write(offset);
                                    rIDX.Write(Buffer.Length);
                                    offset += Buffer.Length;

                                    rIDX.Write((byte)0);
                                    rIDX.Write(name.ToCharArray());
                                    if (name.Length < 15)
                                        rIDX.Write(new byte[15 - name.Length]);

                                    Console.WriteLine(name);

                                    rLB.Write(Buffer);
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
