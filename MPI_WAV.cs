using System;
using System.IO;

namespace MPIandWAV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                byte[] wavHeader = { 82, 73, 70, 70, 151, 121, 2, 0, 87, 65, 86, 69 };
                byte[] mpiHeader = { 70, 79, 80, 73, 151, 121, 2, 0, 71, 69, 80, 79 };
                byte[] Buffer = File.ReadAllBytes(args[0]);

                string extension = Path.GetExtension(args[0]);

                if (extension.ToLower() == ".mpi")
                {
                    for (int x = 0; x < wavHeader.Length; x++)
                        Buffer[x] = wavHeader[x];
                    
                    extension = ".wav";
                }
                else if (extension.ToLower() == ".wav")
                {
                    for (int x = 0; x < mpiHeader.Length; x++)
                        Buffer[x] = mpiHeader[x];

                    extension = ".mpi";
                }
                else
                    Environment.Exit(0);

                File.WriteAllBytes(Path.ChangeExtension(args[0], extension), Buffer);
                Console.WriteLine("Done");
            }
        }
    }
}
