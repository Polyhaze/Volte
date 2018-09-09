using System;

namespace SIVA.Core.Exceptions {
    public class WhatException : Exception {
        public WhatException() 
            : base("How did this exception occur? It's only placed in areas that literally cannot be reached.") { }
    }
}