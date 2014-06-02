using System;
using System.Collections.Generic;

namespace TeamBot.Models
{
    public class ViewBagModel
    {
        protected ViewBagModel()
        {   
        }

        public ViewBagModel(string company, string handlerName, IDictionary<string, object> cache = null)
        {
            if (company == null) 
                throw new ArgumentNullException("company");
            
            if (handlerName == null) 
                throw new ArgumentNullException("handlerName");

            Company = company;
            HandlerName = handlerName;
            ViewBag = cache ?? new Dictionary<string, object>();
        }

        public string Id { get { return string.Format("{0}.{1}", Company, HandlerName); } }

        public string Company { get; private set; }

        public string HandlerName { get; private set; }

        public IDictionary<string, object> ViewBag { get; private set; }

        public void UpdateValues(IDictionary<string, object> values)
        {
            if (values == null) 
                throw new ArgumentNullException("values");

            ViewBag = values;
        }
    }
}