﻿using System.ComponentModel.DataAnnotations;

namespace APBD_Zadanie_6.Models;

public class ProductWarehouseDto
{
    [Required] public int IdProduct { get; set; }

    [Required] public int IdWarehouse { get; set; }

    [Required]
    public int Amount { get; set; }

    [Required] public DateTime CreatedAt { get; set; }
}