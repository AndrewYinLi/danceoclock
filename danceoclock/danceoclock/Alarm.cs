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
        public string action { get; set; }
        public int snoozes { get; set; }

        public Alarm(string musicPath, string date, int hour, int minute, bool isAM, string action) {
            this.musicPath = musicPath;
            this.date = date;
            string[] dateSplit = date.Split('/');
            this.month = Int32.Parse(dateSplit[0]);
            this.day = Int32.Parse(dateSplit[1]);
            this.year = Int32.Parse(dateSplit[2]);
            this.hour = hour;
            this.minute = minute;
            this.isAM = isAM;
            this.action = action;
            this.snoozes = 0;
        }

        public string placeholderZero(int chron) {
            return (chron < 10) ? "0" : "";
        }

        public string getFiller() {
            return "Playing " + musicPath + " on " + date + " at " + hour + ":" + placeholderZero(minute) + minute + ((isAM) ? " AM" : " PM");
        }

        public int getChronologicalPriority() {
            String yearStr = (year+"").Substring(2);
            String monthStr = placeholderZero(month) + month;
            String dayStr = placeholderZero(day) + day;
            int armyHour = ((isAM) ? 0 : 12) + hour;
            String hourStr = placeholderZero(armyHour) + armyHour;
            String minuteStr = placeholderZero(minute) + minute;
            //System.Diagnostics.Debug.Write(yearStr + monthStr + dayStr + hourStr + minuteStr);
            return Int32.Parse(yearStr + monthStr + dayStr + hourStr + minuteStr);
        }
    }
}
