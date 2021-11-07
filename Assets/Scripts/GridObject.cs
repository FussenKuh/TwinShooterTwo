using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{

    private Grid<GridObject> grid;
    private int x;
    private int y;

    private string letters;
    private string numbers;

    private bool walkable;
    public bool Walkable { get { return walkable; } set { walkable = value; grid.TriggerGridObjectChanged(x, y); } }

    private bool occupied;
    public bool Occupied { get { return occupied; } set { occupied = value; grid.TriggerGridObjectChanged(x, y); } }


    public GridObject(Grid<GridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        walkable = true;
        occupied = false;

    }

    public override string ToString()
    {
        return "Walkable: " + walkable + " Occupied: " + occupied;
    }

    public string AsciiMapCharacter
    {
        get
        {
            if (!walkable) { return "X"; }

            if (occupied) { return "O"; }

            return "_";
        }
    }

}
