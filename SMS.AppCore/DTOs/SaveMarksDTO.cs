using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.DTOs
{
    public class SaveMarksDTO
    {
        public int ClassId { get; set; }
        public Dictionary<int, Dictionary<int, int>> Marks { get; set; } = new Dictionary<int, Dictionary<int, int>>();
    }
}
