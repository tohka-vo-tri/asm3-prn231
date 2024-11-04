using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Person
{
    public int PersonId { get; set; }

    public string Fullname { get; set; } = null!;

    public DateOnly BirthDay { get; set; }

    public string Phone { get; set; } = null!;

    public int? UserId { get; set; }

    public virtual ICollection<PersonVirus> PersonViruses { get; set; } = new List<PersonVirus>();

    public virtual ViroCureUser? User { get; set; }
}
