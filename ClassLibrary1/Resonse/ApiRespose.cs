using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Resonse
{
    public record ApiRespose(bool flags=false, string Message="", object? Data = null);
}
