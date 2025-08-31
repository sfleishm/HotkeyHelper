using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotkeyHelper.Models
{
    class JsonModel
    {
        public List<Hotkey>? Hotkeys { get; set; }
    }

    class Hotkey
    {
        public string Action { get; set; }
        public string Description { get; set; }
    }
}
