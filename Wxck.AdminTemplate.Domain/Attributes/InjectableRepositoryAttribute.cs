using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Wxck.AdminTemplate.Domain.Attributes {

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InjectableRepositoryAttribute : Attribute {
        public Type? ServiceType { get; }

        public InjectableRepositoryAttribute(Type? serviceType = null) {
            ServiceType = serviceType;
        }
    }
}