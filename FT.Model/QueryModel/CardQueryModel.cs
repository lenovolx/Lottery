﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model.QueryModel
{
    public class CardQueryModel:BaseQueryModel
    {
        public string CardNum { get; set; }
        public long? GroupId { get; set; }
    }
}
