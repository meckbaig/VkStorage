using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkStorage.Application
{
    public interface IGlobals
    {
        public string group_id { get; }
        public string access_token { get; }
        public string files_folder { get; }
    }
}
