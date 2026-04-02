using System;
using System.Drawing;
using UnityEngine;

public class GridCell
{
    public byte Id { get; set; }
    public int GridX { get; set; }
    public int GridZ { get; set; }
    public int DistanceFromCenter => Math.Max(Math.Abs(GridX), Math.Abs(GridZ));

    public override string ToString() => $"Cell[{Id}] ({GridX},{GridZ}) Dist:{DistanceFromCenter}";
}
