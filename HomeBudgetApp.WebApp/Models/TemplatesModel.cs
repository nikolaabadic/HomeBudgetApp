using HomeBudgetApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBudgetApp.WebApp.Models
{
    public class TemplatesModel
    {
        public List<Template> Templates { get; set; }
        public List<Category> Categories { get; set; }
    }
}
