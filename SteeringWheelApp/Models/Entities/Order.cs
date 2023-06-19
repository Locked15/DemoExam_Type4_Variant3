using System;
using System.Collections.Generic;
using System.Linq;

namespace SteeringWheelApp.Models.Entities;

public partial class Order
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int StatusId { get; set; }

    public int PickupPointId { get; set; }

    public DateOnly OrderDate { get; set; }

    public DateOnly DeliveryDate { get; set; }

    public int TakeCode { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual PickupPoint PickupPoint { get; set; } = null!;

    public virtual OrderStatus Status { get; set; } = null!;

    public virtual User? User { get; set; }

    public decimal FinalOrderDiscount
    {
        get => OrderProducts.Sum(op => op.Product.DiscountAmount * op.Count);
    }

    public decimal FinalOrderCost
    {
        get => OrderProducts.Sum(op => op.Product.FinalCost * op.Count);
    }

    public bool TryToAddNewProduct(Product product)
    {
        var foundOne = OrderProducts.FirstOrDefault(op => op.ProductId == product.Id);
        if (foundOne != null)
        {
            foundOne.Count++;
            return true;
        }
        else
        {
            OrderProducts.Add(new()
            {
                Count = 1,
                Product = product,
                ProductId = product.Id,
                Order = this,
                OrderId = Id
            });
            return false;
        }
    }

    public bool SetProductCount(Product product, int newCount)
    {
        var foundOne = OrderProducts.FirstOrDefault(op => op.ProductId == product.Id);
        if (foundOne != null)
        {
            if (newCount <= 0) OrderProducts.Remove(foundOne);
            else foundOne.Count = newCount;

            return true;
        }

        return false;
    }

    public static Order GenerateNewOrder(User? user)
    {
        var result = new Order()
        {
            OrderDate = DateOnly.FromDateTime(DateTime.Now),
            User = user,
            UserId = user?.Id,
            TakeCode = new Random().Next(100, 1000),
            StatusId = 1,
            PickupPointId = -1,
        };
        return result;
    }
}
