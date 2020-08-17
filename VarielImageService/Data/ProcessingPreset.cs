using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Variel.ImageService.Data
{
    public class ProcessingPreset
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float Quality { get; set; }
        public ResizeOption ResizeOption { get; set; }
    }

    public enum ResizeOption
    {
        StretchToFill,
        AspectFill,
        AspectFit,
        Center
    }
}
