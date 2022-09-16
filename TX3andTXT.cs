using System;
using System.IO;

namespace TX3toTXT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                byte[] Buffer = File.ReadAllBytes(args[0]);

                string extension = Path.GetExtension(args[0]);

                if (extension.ToLower() == ".txt")
                {
                    for (int x = 0; x < Buffer.Length; x++)
                    {
                        if (Buffer[x] >= 0xF)
                            Buffer[x] = (byte)(14 + (256 - Buffer[x]));
                    }
                    extension = ".tx3";
                }
                else if (extension.ToLower() == ".tx3")
                {
                    for (int x = 0; x < Buffer.Length; x++)
                    {
                        if (Buffer[x] >= 0xF)
                            Buffer[x] = (byte)(14 - Buffer[x]);
                    }
                    extension = ".txt";
                }
                else
                    Environment.Exit(0);

                File.WriteAllBytes(Path.ChangeExtension(args[0], extension), Buffer);
                Console.WriteLine("Done");
            }
        }
    }
}
