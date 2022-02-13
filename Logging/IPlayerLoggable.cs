using System;
using System.Collections.Generic;

namespace cyberframe.Logging
{
    public interface IPlayerLoggable
    {
        /// <summary>
        /// Returns the names of the variables 
        /// </summary>
        /// <returns></returns>
        List<string> LogVariableNames();

        /// <summary>
        /// Returns strings with values of logged variable
        /// </summary>
        /// <returns></returns>
        List<string> LogVariableValues();

        /// <summary>
        /// Returns what keys were pressed this frame
        /// </summary>
        /// <returns></returns>
        List<string> PlayerInput();
    }

}
