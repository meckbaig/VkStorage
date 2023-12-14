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

    public class Globals : IGlobals
    {
        public string group_id => "223718377";
        public string access_token => "vk1.a.QnS1oUnJqcloxYoHycP0erD-G3Va9U2YNF3zs693MSGn4KeQBA8V0xnLCSOWbF9EVMZWMt-_gDRqUa_0bUfVg7dKVNFvXPkkXOeEwpL5O_wR6LACwF7tpci62NPw4lo9Ru6mMsHG3j1iKKOhjpTt_-GBSrmJ7BO6vA-Jx9LOfGb83su0DWIqw1qETORD0p3bv4vElkSmqlQBUnc9HaTpCA";
        public string files_folder => "TmpFiles";
    }
}
