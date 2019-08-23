using NLua;
using System;

namespace LuaSampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Lua state = new Lua();
            var res = state.DoString("return 10 + 3*(5 + 2)")[0]; 
            Console.WriteLine("Result :" + res);
            state.LoadCLRPackage();
            state.DoString(@" import ('LuaSampleApp', 'LuaSampleApp') 
			   import ('System.Web') ");
            Console.WriteLine("Result before Script :" + SampleObject.GetValue("c"));
            state.DoString(@"
	        local res4 = SampleObject.GetValue('b')
            res4 = res4 * res4 * res4
            SampleObject.SetValue('c', res4);
	        ");
            Console.WriteLine("Result After Script :" + SampleObject.GetValue("c"));
            Console.ReadLine();
        }
    }
}
