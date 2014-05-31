using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data_PushBBVA
{
    public class NotificationType
    {
        private int _idNotificationType;
        public int idNotificationType
        {
            get { return _idNotificationType; }
            set { _idNotificationType = value; }
        }

        private string _status;
        public string status
        {
            get { return _status; }
            set { _status = value; }
        }

        private string _title;
        public string title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _start;
        public string start
        {
            get { return _start; }
            set { _start = value; }
        }

        private int _duration;
        public int duration
        {
            get { return _duration; }
            set { _duration = value; }
        }
    }
}
