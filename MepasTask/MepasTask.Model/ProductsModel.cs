using System.Drawing;

namespace MepasTask.Models
{
    public class ProductsModel : BaseDbDefault
    {

        
        public string name { get; set; }
        public int category_id { get; set; }
        public decimal price { get; set; }
        public string unit { get; set; }
        public int stock { get; set; }
        public string color { get; set; }
        public int? weight { get; set; }
        public int? width { get; set; }
        public int? heigth { get; set; }
        public int added_user_id { get; set; }
        public int? updated_user_id { get; set; }
      
    }
}

        
  