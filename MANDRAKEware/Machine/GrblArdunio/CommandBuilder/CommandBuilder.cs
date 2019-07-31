using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MANDRAKEware.Machine.GrblArdunio.CommandBuilder
{
    /// <summary>
    /// This class builds, figures out distance, and executes the commands for grbl
    /// </summary>
    class CommandBuilder
    {
        #region Properties
        private string command; //completed command
        private const char Macro = 'M'; //const char of M for marco commands
        private const char gcommand = 'G'; //const char for G commands
       
        private const char xAxis = 'X';
        private const char yAxis = 'Y';
        private CommandCalc calculator; //command calculator

        public string Command { get => command; set => command = value; }
        #endregion

        #region Constructors
        public CommandBuilder()
        {

        }
        #endregion

        #region Methods
        public string CommandBuild(int distance, char axis)
        {
            StringBuilder sb = new StringBuilder();

            //TODO: rest of function where appends stuff

            return null;
        }
        #endregion

    }
}
