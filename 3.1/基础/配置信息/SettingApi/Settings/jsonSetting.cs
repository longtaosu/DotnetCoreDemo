using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SettingApi.Settings
{
    public class jsonSetting
    {
        public string json1 { get; set; }

        public string json2 { get; set; }

        public JsonClass jsonClass { get; set; }
    }

    public class JsonClass
    {
        public string jsonc1 { get; set; }
        public string jsonc2 { get; set; }
    }
}
