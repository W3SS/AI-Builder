﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    private enum States
    {
        Hidden, Visible
    }
    private States state;
    
    private Tile tile;

    [SerializeField]
    //private bool isEnabled;   // TODO use this to enable / disable tiles/cells to be loaded in the level
    private List<PlaceableObject> inhabitingObjects = new List<PlaceableObject>();

    private static GameObject tilePrefab;
    private static Transform tilesParent;

    #region lifecycle

    public void Init(int row, int col, LevelGrid grid)
    {
        if (tilePrefab == null)
        {
            tilePrefab = Resources.Load<GameObject>("Prefabs/Tile");
            tilesParent = new GameObject("Tiles").transform;
        }
        
        tile = Object.Instantiate(tilePrefab).GetComponent<Tile>();
        Vector3 tilePos = grid.GetFloorPosition() + new Vector3(col * GridProperties.TILE_SIZE, 0f, row * GridProperties.TILE_SIZE);
        tile.SetupTile(this, tilesParent, tilePos);
        
        state = States.Hidden;
    }

    #endregion lifecycle

    public List<PlaceableObject> InhabitingObjects
    {
        get { return inhabitingObjects; }
    }
    
    public bool IsEmpty()
    {
        return inhabitingObjects == null || inhabitingObjects.Count == 0;
    }

    public bool HasWall()
    {
        bool hasWall = false;
        foreach (PlaceableObject obj in inhabitingObjects)
        {
            if (obj.IsType<Wall>())
            {
                hasWall = true;
                break;
            }
        }
        return hasWall;
    }

    public Wall GetWall()
    {
        Wall wall = null;
        foreach (PlaceableObject obj in inhabitingObjects)
        {
            if (obj.IsType<Wall>())
            {
                wall = (Wall)obj;
                break;
            }
        }
        return wall;
    }

    public void RemoveInhabitingObject(PlaceableObject obj)
    {
        obj.SetSize(1f);
        inhabitingObjects.Remove(obj);
    }

    public void AddInhabitingObject(PlaceableObject obj)
    {
        if (inhabitingObjects.Contains(obj)) return;
        inhabitingObjects.Add(obj);

        if (state == States.Hidden)
        {
            obj.SetSize(0.01f);
        }
    }

    public void Show()
    {
        if (state == States.Visible) return;
        
        // TODO fix coupling issue here - Cell and Tile are strongly coupled
        SetObjectSize(0f);
        tile.Show();

        state = States.Visible;
    }

    public void Hide()
    {
        if (state == States.Hidden) return;

        tile.Hide();
        state = States.Hidden;
    }

    public void SetObjectSize(float sizeRatio)
    {
        tile.SetSize(0f);

        if (inhabitingObjects == null) return;

        foreach (SelectableObject obj in inhabitingObjects)
        {
            obj.SetSize(sizeRatio);
        }
    }
}
