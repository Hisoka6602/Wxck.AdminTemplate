﻿namespace Wxck.AdminTemplate.Domain.Entities {

    public interface IEntity<TPrimaryKey> {
        TPrimaryKey Id { get; set; }
    }
}