using System;
using System.Collections.Generic;

namespace cyberframe.Logging
{
    public interface IPlayerLoggable
    {
        string HeaderLine();
        List<string>  PlayerInformation();
    }

}
