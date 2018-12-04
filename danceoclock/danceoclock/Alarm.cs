using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danceoclock {
    public class Alarm {
        public string musicPath { get; set; }
        public string date { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public int year { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
        public bool isAM { get; set; }
        public string actionPath { get; set; }
        public int snoozes { get; set; }
        public int armyHour { get; set; }

        // kinectwindow settings
        public int numrepeats;
        public int tolerance;
        public int timeout;

        public Alarm(string musicPath, string date, int hour, int minute, bool isAM, string actionPath, int numrepeats, int tolerance, int timeout) {
            this.numrepeats = numrepeats;
            this.tolerance = tolerance;
            this.timeout = timeout;
            this.musicPath = musicPath;
            this.date = date;
            string[] dateSplit = date.Split('/');
            this.month = Int32.Parse(dateSplit[0]);
            this.day = Int32.Parse(dateSplit[1]);
            this.year = Int32.Parse(dateSplit[2]);
            this.hour = hour;
            this.minute = minute;
            this.isAM = isAM;
            this.actionPath = actionPath;
            this.snoozes = 0;
            if (isAM)
            {
                if(hour == 12)
                {
                    this.armyHour = 0;
                }
                else
                {
                    this.armyHour = hour;
                }
            }
            else
            {
                this.armyHour = 11 + hour;
            }
        }

        public string placeholderZero(int chron) {
            return (chron < 10) ? "0" : "";
        }

        public string getFiller() {
            return "Playing " + musicPath + " on " + date + " at " + hour + ":" + placeholderZero(minute) + minute + ((isAM) ? " AM" : " PM");
        }

        public int getChronologicalPriority() {
            string yearStr = (year+"").Substring(2);
            string monthStr = placeholderZero(month) + month;
            string dayStr = placeholderZero(day) + day;
           
            string hourStr = placeholderZero(armyHour) + armyHour;
            string minuteStr = placeholderZero(minute) + minute;
            //System.Diagnostics.Debug.Write(yearStr + monthStr + dayStr + hourStr + minuteStr);
            return Int32.Parse(yearStr + monthStr + dayStr + hourStr + minuteStr);
        }
    }
}
