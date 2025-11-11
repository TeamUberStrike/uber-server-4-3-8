using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.ViewModel
{
    public class PromotionContentViewModel
    {
        public int PromotionContentId { get; set; }

        public string Name { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }

        public bool IsPermanent { get; set; }

        public List<PromotionContentElementViewModel> PromotionContentElements { get; set; }
    }
}
    