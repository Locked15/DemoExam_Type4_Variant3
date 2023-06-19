using System.Collections.Generic;

namespace SteeringWheelApp.Models.Entities;

public partial class PickupPoint
{
    public int Id { get; set; }

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public int House { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public override string ToString() => $"г. {City}, ул. {Street}, д. {House}.";
}
