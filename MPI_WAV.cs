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
                string wavHeader = "RIFF____WAVE";
                string mpiHeader = "MOPI____GEPO";
                byte[] Buffer = File.ReadAllBytes(args[0]);

                string extension = Path.GetExtension(args[0]);

                if (extension.ToLower() == ".mpi")
                {
                    for (int x = 0; x < wavHeader.Length; x++)
                    {
                        if (wavHeader[x] != '_')
                            Buffer[x] = (byte)wavHeader[x];
                    }
                    extension = ".wav";
                }
                else if (extension.ToLower() == ".wav")
                {
                    for (int x = 0; x < mpiHeader.Length; x++)
                    {
                        if (mpiHeader[x] != '_')
                            Buffer[x] = (byte)mpiHeader[x];
                    }
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
