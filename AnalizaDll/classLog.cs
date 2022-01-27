using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizaDll
{
    public class classLog
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #region Nlog function
        public static void LogInfo(params string[] list)
        {
            foreach (var item in list)
            {
               
                logger.Info(item);
            }
        }

        public static void LogWarn(params string[] list)
        {
            try
            {
                foreach (var item in list)
                {
                    logger.Warn(item);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
           
        }
        public static void LogError(params string[] list)
        {
            foreach (var item in list)
            {
                logger.Error(item);
            }
        }
        public static string LogException(Exception e)
        {
            string res = GetExceptionMessage(e);
            logger.Error(res);
#if DEBUG
            System.Windows.Forms.MessageBox.Show(res);
#endif

            return res;

        }
        public static string GetExceptionMessage(Exception ex)
        {
            if (ex.InnerException == null)
            {
                return string.Concat(ex.Message, System.Environment.NewLine, ex.StackTrace);
            }
            else
            {
                if (ex.InnerException.InnerException == null)
                    return ex.InnerException.Message;
                else
                    return string.Concat(string.Concat(ex.InnerException.Message, System.Environment.NewLine, ex.StackTrace), System.Environment.NewLine, GetExceptionMessage(ex.InnerException));
            }
        }
        #endregion
    }
}
