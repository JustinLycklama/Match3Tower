using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public struct Item : IEquatable<Item>, IComparable<Item>
{
    public int id { get; private set; }
    public string title { get; private set; }

    public Item(int identifier)
    {
        id = identifier;
        title = id.ToString();
    }

    public bool Equals(Item other)
    {
        return id == other.id;
    }

    public int CompareTo(Item other)
    {
        return id.CompareTo(other.id);
    }
}

public struct Coordinate
{
    public readonly int x;
    public readonly int y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class Match3Model : MonoBehaviour
{
    Item?[,] data;

    public int numberOfRows;
    public int numberOfColumns;

    public delegate void OnDataChange(Dictionary<Item, Coordinate> dataMap);
    public OnDataChange dataChangeDelegate;



    // Start is called before the first frame update
    void Start()
    {
        data = new Item?[numberOfColumns, numberOfRows];

        data[0, 0] = new Item(1);

        data[1, 1] = new Item(2);

        //for (int col = 0; col < numberOfColumns; col++) {
        //    data[col] = new Item[numberOfRows];
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //dataChangeDelegate.Invoke(data);
    }

    void setData()
    {
        if (dataChangeDelegate == null) {
            return;
        }

        Dictionary<Item, Coordinate> dataMap = new Dictionary<Item, Coordinate>();

        for (int x = 0; x < numberOfRows; x++)
        {
            for (int y = 0; y < numberOfColumns; y++)
            {
                if (data[x, y] != null)
                {
                    dataMap[data[x, y].Value] = new Coordinate(x, y);
                }

            }
        }
        
        dataChangeDelegate.Invoke(dataMap);
    }

    private void OnMouseDown()
    {
        data[0, 1] = data[1, 1];
        data[1, 1] = null;
        setData();
    }

    public void subscribe(OnDataChange f)
    {
        dataChangeDelegate += f;
        setData();
    }
}
