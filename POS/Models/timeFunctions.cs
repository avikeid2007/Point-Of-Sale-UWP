using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    class timeFunctions
    {
        public static string time24to12(string time)
        {
            try
            {
                time = time.Substring(0, 2) + ":" + time.Substring(2, 2) + ":" + time.Substring(4, 2);

                if (Convert.ToInt16(time.Substring(0, 2)) > 12)
                {

                    time = time + " PM";
                    time = Convert.ToString(Convert.ToInt16(time.Substring(0, 2)) - 12) + time.Substring(2, time.Length - 2);
                }
                else if (Convert.ToInt16(time.Substring(0, 2)) < 10)
                {
                    time = time.Substring(1, time.Length - 1) + " AM";

                }
                else
                {
                    time = time + " AM";
                }
            }
            catch { }
            return time;
        }

        public static string formattingDate(string date)
        {
            string[] dateComp = date.Split('/');

            if (dateComp[1].Length < 2)
            {
                dateComp[1] = "0" + dateComp[1];
            }

            if (dateComp[0].Length < 2)
            {
                dateComp[0] = "0" + dateComp[0];
            }

            date = dateComp[2] + dateComp[0] + dateComp[1];

            return date;
        }

        public static string calcTimeSpan(string startingTime, string endingTime)
        {
            string timeSpan = "!";
            int seconds;
            int minutes;
            int hours;
            bool borrow = false;
            //seconds
            if (Convert.ToInt32(endingTime.Substring(4, 2)) - Convert.ToInt32(Convert.ToString(startingTime).Substring(4, 2)) < 0)
            {
                seconds = 60 + Convert.ToInt32(endingTime.Substring(4, 2)) - Convert.ToInt32(Convert.ToString(startingTime).Substring(4, 2));
                borrow = true;
            }
            else
            {
                seconds = Convert.ToInt32(endingTime.Substring(4, 2)) - Convert.ToInt32(Convert.ToString(startingTime).Substring(4, 2));


            }
            //minutes
            if (Convert.ToInt32(endingTime.Substring(2, 2)) - Convert.ToInt32(Convert.ToString(startingTime).Substring(2, 2)) < 0)
            {
                minutes = 60 + Convert.ToInt32(endingTime.Substring(2, 2)) - Convert.ToInt32(Convert.ToString(startingTime).Substring(2, 2));
                if (borrow)
                {
                    minutes = minutes - 1;
                    borrow = false;
                }
                borrow = true;
            }
            else
            {
                minutes = Convert.ToInt32(endingTime.Substring(2, 2)) - Convert.ToInt32(Convert.ToString(startingTime).Substring(2, 2));
                if (borrow)
                {
                    minutes = minutes - 1;
                    borrow = false;
                }
            }

            //hours
            if (Convert.ToInt32(endingTime.Substring(0, 2)) - Convert.ToInt32(Convert.ToString(startingTime).Substring(0, 2)) < 0)
            {
                hours = 24 + Convert.ToInt32(endingTime.Substring(0, 2)) - Convert.ToInt32(Convert.ToString(startingTime).Substring(0, 2));
                if (borrow)
                {
                    hours = hours - 1;
                    borrow = false;
                }
            }
            else
            {
                hours = Convert.ToInt32(endingTime.Substring(0, 2)) - Convert.ToInt32(Convert.ToString(startingTime).Substring(0, 2));
                if (borrow)
                {
                    hours = hours - 1;
                    borrow = false;
                }
            }



            timeSpan = hours + ":" + minutes + ":" + seconds;
            return timeSpan;
        }

        public static string formattingTimeBack(string time)
        {
            if (time.Contains(":"))
            {
                if (Convert.ToInt32(time.Substring(0, time.IndexOf(":"))) < 10)
                {
                    time = "0" + time;
                }
            }
            if (time.Contains("PM") && time.Substring(0, 2) != "12")
            {
                time = Convert.ToString(Convert.ToInt32(time.Substring(0, 2)) + 12) + time.Substring(3, 2) + time.Substring(6, 2);

            }
            else if (time.Contains("PM") || time.Contains("AM"))
            {
                time = time.Substring(0, 2) + time.Substring(3, 2) + time.Substring(6, 2);

            }


            //24 hour time
            return time;
        }
    }


        
    
}
