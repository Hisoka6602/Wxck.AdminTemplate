using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Wxck.AdminTemplate.Domain.Aggregates {

    public abstract class AggregateRoot<TEntity> where TEntity : class {
        public TEntity Entity { get; protected set; }

        protected AggregateRoot(TEntity entity) {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
        }
    }
}