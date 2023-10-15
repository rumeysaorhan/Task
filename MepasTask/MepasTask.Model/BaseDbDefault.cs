using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MepasTask.Models
{
    public class BaseDbDefault
    {
        public int id { get; set; }

        [DataType(DataType.Date)]
        public DateTime? created_date { get; set; }
        [DataType(DataType.Date)]
        public DateTime? updated_date { get; set; }

    }
}
