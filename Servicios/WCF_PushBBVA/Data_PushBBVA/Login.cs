using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data_PushBBVA
{
    public class Login
    {
        public string _status;

        public string status 
        {
            get { return _status; }
            set { _status = value; }
        }

        private string _user;

        public string user
        {
            get { return _user; }
            set { _user = value; }
        }

        private string _name;
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
