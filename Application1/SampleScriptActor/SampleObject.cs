using System;
using System.Collections.Generic;
using System.Text;

namespace SampleScriptActor
{
    public class SampleObject
    {
        static int a = 1;
        static int b = 2;
        static int c = 3;

        public static int GetValue(string name)
        {
            if(name == "a")
            {
                return a;
            }
            else if (name == "b")
            {
                return b;
            }
            else if (name == "c")
            {
                return c;
            }               
            return 0;
        }

        public static void SetValue(string name, int value)
        {
            if (name == "a")
            {
                a = value;
            }
            else if (name == "b")
            {
                b = value;
            }
            else if (name == "c")
            {
                c = value;
            }
        }
    }
}
