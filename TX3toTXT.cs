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

                for (int x = 0; x < Buffer.Length; x++)
                {
                    if (Buffer[x] >= 0xF)
                        Buffer[x] = (byte)(14 - Buffer[x]);
                }
                File.WriteAllBytes(args[0].Replace(".tx3", ".txt").Replace(".TX3", ".txt"), Buffer);
                Console.WriteLine("Done");
            }
        }
    }
}
