﻿using System;
using System.Collections.Generic;

namespace LSC.SmartCartHub.Entities;

public partial class Cart
{
    public int CartId { get; set; }

    public int? UserId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual Product? Product { get; set; }

    public virtual UserProfile? User { get; set; }
}
