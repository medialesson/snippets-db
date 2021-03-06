﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Common
{
    public class AppSettings
    { 
        /// <summary>
        /// Secret key for use with hash algorithms
        /// </summary>
        public string Secret { get; set; }
        public string AccentColors { get; set; }

        public IEnumerable<string> AccentColorsList { get => AccentColors.Split(';').ToList(); }
    }
}
