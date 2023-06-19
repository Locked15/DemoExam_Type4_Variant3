using System;
using System.Collections.Generic;
using System.Dynamic;

namespace SteeringWheelApp.Models.Entities;

public partial class Product
{
    public int Id { get; set; }

    public int ManufacturerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Image { get; set; }

    public int Amount { get; set; }

    public sbyte? Discount { get; set; }

    public decimal Cost { get; set; }

    public virtual Manufacturer Manufacturer { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public decimal DiscountAmount
    {
        get => Cost * ((Discount ?? 0M) / 100M);
    }

    public decimal FinalCost
    {
        get => Cost - DiscountAmount;
    }

    /// <summary>
    /// Это свойство схоже с работой объектов в JavaScript.
    /// Все подсвойства определяются динамически и не ограничены классами.
    /// </summary>
    public dynamic BindProperties
    {
        get
        {
            // 'ExpandoObject' не поддерживает инициализатор, так что его свойства нужно определять вручную.
            dynamic result = new ExpandoObject();

            result.Name = $"Название: {Name}";
            result.Description = Description != null ? $"Описание: {Description}" : string.Empty;
            result.Manufacturer = $"Производитель: {Manufacturer.Name}";
            result.Image = $"/Assets/Products/{Image ?? string.Empty}";

            result.Amount = $"Количество: {Amount}";
            result.Discount = $"Скидка: {Discount}%";
            result.Cost = $"Стоимость: {Cost:0,00}Р";

            return result;
        }
    }
}
