using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Wxck.AdminTemplate.Domain.Attributes {

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InjectableServiceAttribute : Attribute {
        public Type? ServiceType { get; }

        public InjectableServiceAttribute(Type? serviceType = null) {
            ServiceType = serviceType;
        }
    }
}