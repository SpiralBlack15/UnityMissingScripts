using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spiral.EditorTools.DeadScriptsSearcher
{
    // SUPER EASY LOCALIZATION TIME

    public enum Local { RU, ENG }
    public struct LocalString
    {
        public string RU  { get; private set; }
        public string ENG { get; private set; }

        public LocalString(string RU, string ENG)
        {
            this.RU = RU;
            this.ENG = ENG;
        }
        public string Read(Local local)
        {
            switch (local)
            {
                case Local.RU:  return RU;
                case Local.ENG: return ENG;
             
                default: throw new ArgumentException("Sorry, this local is unsupported yet :(");
            }
        }
    }
}

