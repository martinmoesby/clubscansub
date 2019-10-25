using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubScansub.Models
{
    public enum EntityState
    {
        Added,
        Modified,
        Deleted,
        Unchanged
    }

    public interface IEntity
    {
        EntityState EntityState {get;set;}
    }
}
