using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightTable.Model
{
    public class Track
    {
        public string Title { get; set; }
        public string StorageName { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
