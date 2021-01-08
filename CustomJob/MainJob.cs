using System;

namespace CustomJob
{
    public class MainJob
    {
        public string SayHello(string name)
        {
            string upperName = Util.StringUtil.MakeUppercase(name);
            return $"Hello, {upperName}";
        }
    }
}
