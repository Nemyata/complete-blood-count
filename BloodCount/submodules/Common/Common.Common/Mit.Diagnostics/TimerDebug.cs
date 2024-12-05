using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;

namespace Common.Diagnostics
{
    /// <summary>
    /// Замер времени
    /// </summary>
    public class TimerLog
    {
        private Stopwatch sw;
        private readonly CultureInfo cult = CultureInfo.CreateSpecificCulture("ru-RU");
        private ILogger _log;

        /// <summary>
        /// Считать последнее время работы
        /// </summary>
        public double WorkingHours
        {
            get; private set;
        }

        public TimerLog()
        {
            sw = Stopwatch.StartNew();
        }

        public TimerLog(ILogger log)
        {
            sw = Stopwatch.StartNew();
            _log = log;
        }

        /// <summary>
        /// Замер в милисикундах
        /// </summary>
        /// <returns></returns>
        public void Start()
        {
            sw = Stopwatch.StartNew();
        }

        /// <summary>
        /// Замер в секундах
        /// </summary>
        /// <returns></returns>
        public double StopSec(string text)
        {
            double f = -1;
            if (sw != null)
            {
                sw.Stop();
                f = (sw.ElapsedMilliseconds) / 1000;
            }
            else
            {
                f = -1;
            }

            WorkingHours = f;
            if (_log != null)
            {
                _log.LogInformation($"{text} {WorkingHours} sec", text, WorkingHours);
            }
            return f;
        }

        /// <summary>
        /// Замер в секундах
        /// </summary>
        /// <returns></returns>
        public string StopSecStr()
        {
            return StopSecStr("");
        }

        /// <summary>
        /// Замер в секундах
        /// </summary>
        /// <returns></returns>
        public string StopSecStr(string str)
        {
            double f = -1;
            if (sw != null)
            {
                sw.Stop();
                f = (sw.ElapsedMilliseconds) / 1000;
            }
            else
            {
                f = -1;
            }
            WorkingHours = f;
            Start();

            string alert = GetAlert(f);

            var rest = ($"..{str}\t finish in\t{(f).ToString("N0", cult)} s\t{alert} Process [{Process.GetCurrentProcess().Id}]");
            if (_log != null)
            {
                _log.LogInformation(rest);
            }

            return rest;
        }

        private static string GetAlert(double f)
        {
            var alert = "";

            if (f > 2)
            {
                alert = "Более 2 секунд";
            }
            else if (f > 5)
            {
                alert = "Более 5 секунд";
            }
            else if (f > 10)
            {
                alert = "Более 10 секунд";
            }

            return alert;
        }
    }
}
