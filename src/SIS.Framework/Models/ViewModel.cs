using System.Collections.Generic;

namespace SIS.Framework.Models
{
    public class ViewModel
    {
        public ViewModel()
        {
            this.Data = new Dictionary<string, object>();
        }

        public IDictionary<string, object> Data { get; }

        public object this[string key]
        {
            get => this.Data[key];
            set => this.Data[key] = value;
        }
    }
}
