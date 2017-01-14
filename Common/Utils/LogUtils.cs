using System.Diagnostics;

namespace Common.Utils
{
    public class LogUtils
    {
        private const string TAG = "YueBa";
        private static bool isEnable = true;

        public static void init(bool enableLog)
        {
            isEnable = enableLog;
        }
        public static void Log(string message)
        {
            if (isEnable)
            {
                Debug.WriteLine("{0}: {1}", TAG, message);
            }
        }
    }
}
