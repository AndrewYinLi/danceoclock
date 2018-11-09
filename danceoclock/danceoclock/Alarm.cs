using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danceoclock {
    class Alarm {
        public string musicPath { get; set; }
        public string date { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public int year { get; set; }
        public int h { get; set; }
        public int m { get; set; }
        public bool isAM { get; set; }
        public string action { get; set; }

        public Alarm(string musicPath, string date, int h, int m, bool isAM, string action) {
            this.musicPath = musicPath;
            this.date = date;
            string[] dateSplit = date.Split('/');
            this.month = Int32.Parse(dateSplit[0]);
            this.day = Int32.Parse(dateSplit[1]);
            this.year = Int32.Parse(dateSplit[2]);
            this.h = h;
            this.m = m;
            this.isAM = isAM;
            this.action = action;
        }

        public string getFiller() {
            return "Playing " + musicPath + " on " + date + " at " + h + ":" + ((m < 9) ? "0" : "") + m + ((isAM) ? " AM" : " PM");
        }

        public int getPriority() {

        }
    }
}
