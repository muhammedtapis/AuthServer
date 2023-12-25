﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Configuration
{
    //appsettingsteki TokenOptions alanını karşılamak için eşleştirmek için oluşturduk OPTIONS pattern ile.
    public class CustomTokenOptions
    {
        public List<string> Audience { get; set; }
        public string Issuer { get; set; }
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }
        public string SecurityKey { get; set; }
    }
}