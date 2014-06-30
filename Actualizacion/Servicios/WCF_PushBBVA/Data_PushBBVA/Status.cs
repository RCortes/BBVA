using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data_PushBBVA
{
    public class Status
    {
        private string _status;
        public string status
        {
            get { return _status; }
            set { _status = value; }
        }

        private string _description;
        public string description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
