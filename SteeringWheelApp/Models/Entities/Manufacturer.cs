﻿using System;
using System.Collections.Generic;

namespace SteeringWheelApp.Models.Entities;

public partial class Manufacturer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
