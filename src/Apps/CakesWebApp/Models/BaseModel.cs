using System;
using System.Collections.Generic;
using System.Text;

namespace CakesWebApp.Models
{
    public abstract class BaseModel<T>
    {
        public T Id { get; set; }
    }
}
