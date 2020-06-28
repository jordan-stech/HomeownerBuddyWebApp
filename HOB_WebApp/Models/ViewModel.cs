using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOB_WebApp.Models
{
    public class ViewModel
    {
        public MaintenanceReminders CreatedReminders { get; set; }
        public IEnumerable<ContentModel> ActionPlans { get; set; }
    }
}
