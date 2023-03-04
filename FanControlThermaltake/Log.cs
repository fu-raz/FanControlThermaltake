using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.Thermaltake
{
    public class Log
    {
        // Yea I know this is lame
        protected static bool Debug = false;
        public static void WriteToLog(String data)
        {
            if (Log.Debug)
            {
                string fileName = "Thermaltake.log";

                if (!String.IsNullOrWhiteSpace(fileName) && !String.IsNullOrWhiteSpace(data))
                {
                    try
                    {
                        System.IO.File.AppendAllText(fileName, $"{DateTime.UtcNow:R} {data}{Environment.NewLine}");
                    }
                    catch (System.Security.SecurityException)
                    {

                    }
                    catch (System.IO.IOException)
                    {

                    }
                    catch (NotSupportedException)
                    {

                    }
                    catch (UnauthorizedAccessException)
                    {

                    }
                    catch (ArgumentException)
                    {

                    }
                }
            }
        }
    }
}
