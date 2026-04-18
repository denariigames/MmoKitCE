using System;
using System.Drawing;
using UnityEngine;

namespace MultiplayerARPG
{
    public struct GridCell
    {
        public byte Id { get; set; }
        public int GridX { get; set; }
        public int GridZ { get; set; }
    }
}
