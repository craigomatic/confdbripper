using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfDbRipper.Model
{
    internal class Content
    {
        public string Id { get; set; }

        public string AuthorId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

    }
}
