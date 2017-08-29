using System.Diagnostics;
using System.Threading;

namespace FileSoakTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TraceSource source = new TraceSource("mySource");
            while (true)
            {
                Thread.Sleep(10);
                source.TraceInformation("Test message");
            }
        }
    }
}
