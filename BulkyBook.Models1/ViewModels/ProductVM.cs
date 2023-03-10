﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models1.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; } = null!;
        public IEnumerable<SelectListItem> CategoryList { get; set; } = null!;
        public IEnumerable<SelectListItem> CoverTypeList { get; set; } = null!;
    }

}
