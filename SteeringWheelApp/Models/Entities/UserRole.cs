﻿using System;
using System.Collections.Generic;

namespace SteeringWheelApp.Models.Entities;

public partial class UserRole
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
