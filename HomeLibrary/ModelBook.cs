using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeLibrary
{
    class ModelBook
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public StatusReading Status { get; set; }
        public int Grade { get; set; }
    }
}
