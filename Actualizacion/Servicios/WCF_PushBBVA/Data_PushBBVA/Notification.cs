using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data_PushBBVA
{
    public class Notification
    {
        private int _idNotification;
        public int idNotification
        {
            get { return _idNotification; }
            set { _idNotification = value; }
        }

        private string _name;
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _rut;
        public string rut
        {
            get { return _rut; }
            set { _rut = value; }
        }

        private DateTime _delivery;
        public DateTime delivery
        {
            get { return _delivery; }
            set { _delivery = value; }
        }

        private DateTime _create;
        public DateTime create
        {
            get { return _create; }
            set { _create = value; }
        }

        private string _device;
        public string device
        {
            get { return _device; }
            set { _device = value; }
        }

        private string _plataform;
        public string plataform
        {
            get { return _plataform; }
            set { _plataform = value; }
        }

        private string _tipo;
        public string tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }

        private string _title;
        public string title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _text;
        public string text
        {
            get { return _text; }
            set { _text = value; }
        }

        private bool _status;
        public bool status 
        {
            get { return _status;}
            set { _status = value; }
        }


    }
}
