﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model.QueryModel
{
    public class AmountWaterQueryModel:BaseQueryModel
    {
        public int? Type { get; set; }
        public string UserName { get; set; }
    }
}
