using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiPaginatedCrud.Entities
{
    public interface ITimestampedEntity
    {
        DateTime? CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}