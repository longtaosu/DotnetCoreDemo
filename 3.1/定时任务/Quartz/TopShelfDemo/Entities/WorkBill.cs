using Chloe.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace TopShelfDemo.Entities
{
    [Table("Dat_WorkBill")]
    public class WorkBill
    {

        public string WorkBillID { get; set; }

        public string BillDes { get; set; }
    }
}
