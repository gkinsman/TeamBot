using System;
using System.Collections.Generic;

namespace TeamBot.Models
{
    public class SettingModel
    {
        protected SettingModel()
        {   
        }

        public SettingModel(string company, IDictionary<string, object> settings = null)
        {
            if (company == null) 
                throw new ArgumentNullException("company");

            Company = company;
            Settings = settings ?? new Dictionary<string, object>();
        }

        public string Company { get; private set; }

        public IDictionary<string, object> Settings { get; private set; }
    }
}