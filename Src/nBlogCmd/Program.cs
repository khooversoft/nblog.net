using nBlogCmd.Application;
using System;

namespace nBlogCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var arguments = new[]
            {
                "help"
            };

            Option option = new OptionBuilder()
                .SetArgs(arguments)
                .Build();
        }
    }
}
