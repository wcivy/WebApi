using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wcivy.Sample.Client;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new SignAuthClient("wc", "test", "http://localhost:4496/");
            var t1 = client.Get("api/Values");
            if(t1.IsSuccess) Console.WriteLine(t1.Result.ToString());
            var t2 = client.Get("api/Values/1");
            if (t2.IsSuccess) Console.WriteLine(t2.Result);
            Console.Read();
        }
    }
}
