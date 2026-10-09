using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Domain.Entities
{
    public abstract class BaseEntity<TKey>
    {


        public TKey ID { get; set; } = default!;


    }
}
