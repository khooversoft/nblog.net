using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace Toolbox.Model
{
    public enum RunEnvironment
    {
        Local,
        Dev,
        PPE,
        Prod
    }

    public static class RunEnvironmentExtensions
    {
        public static RunEnvironment ToEnvironment(this string subject)
        {
            Enum.TryParse(subject, true, out RunEnvironment enviornment)
                .VerifyAssert(x => x == true, $"Invalid environment {subject.ToNullIfEmpty() ?? "<none>"}");

            return enviornment;
        }
    }
}
