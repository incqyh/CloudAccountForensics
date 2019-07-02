using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Model.Common
{
    public class TimeConverter
    {
        /// <summary>
        /// 输入的整形数据为毫秒数
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        static public DateTime UInt64ToDateTime(UInt64 timeStamp)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime datatime = startTime.AddMilliseconds(timeStamp);
            return datatime;
        }

        /// <summary>
        /// 返回当前unix毫秒时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        static public string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string currentTimeStamp = Convert.ToInt64(ts.TotalMilliseconds).ToString();
            return currentTimeStamp;
        }

        /// <summary>
        /// 返回特定时间点的unix时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        static public string DateTimeToString(DateTime time)
        {
            TimeSpan ts = time - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string currentTimeStamp = Convert.ToInt64(ts.TotalMilliseconds).ToString();
            return currentTimeStamp;
        }

        /// <summary>
        /// 整型数据转为时间跨度，单位为秒
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        static public TimeSpan ToTimeSpan(UInt32 seconds)
        {
            TimeSpan interval = TimeSpan.FromSeconds(seconds);
            return interval;
        }
        static public TimeSpan ToTimeSpan(UInt64 seconds)
        {
            TimeSpan interval = TimeSpan.FromSeconds(seconds);
            return interval;
        }

        /// <summary>
        /// 时间跨度转为整型数据，单位为秒
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        static public UInt32 TimeSpanToUInt32(TimeSpan interval)
        {
            UInt32 seconds = Convert.ToUInt32(interval.TotalSeconds);
            return seconds;
        }
    }
}
