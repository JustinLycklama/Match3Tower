using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public struct Item : IEquatable<Item>, IComparable<Item>
{
    public enum Type { 
        one,
        two
    }


    public string id { get; private set; }
    public string title { get; private set; }

    public Type type;

    public Item(Type type)
    {
        id = Guid.NewGuid().ToString();
        title = id;

        this.type = type;
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
        Item?[,] data = new Item?[numberOfColumns, numberOfRows];

        data[0, 5] = new Item(Item.Type.one);
        data[1, 4] = new Item(Item.Type.one);
        data[1, 5] = new Item(Item.Type.one);
        data[2, 3] = new Item(Item.Type.one);

        setData(data);
        gravityAction();
        StartCoroutine(WaitForChange());
    }

    // Update is called once per frame
    void Update()
    {
        //dataChangeDelegate.Invoke(data);
    }

    Dictionary<Item, Coordinate> dataMap = new Dictionary<Item, Coordinate>();


    void setData(Item?[,] data)
    {
        this.data = data;
        rebuildDataMap();
    }

    void rebuildDataMap()
    {
        dataMap.Clear();

        for (int row = 0; row < numberOfRows; row++)
        {
            for (int column = 0; column < numberOfColumns; column++)
            {
                if (data[column, row] != null)
                {
                    dataMap[data[column, row].Value] = new Coordinate(column, row);
                }

            }
        }

        if (dataChangeDelegate == null || data == null)
        {
            return;
        }

        dataChangeDelegate.Invoke(dataMap);
    }

    private void gravityAction()
    {
        print("Gravity Action");
        Item?[,] newData = new Item?[numberOfColumns, numberOfRows];

        for (int column = 0; column < numberOfColumns; column++)
        {
            List<Item> items = new List<Item>();

            for (int row = 0; row < numberOfRows; row++)
            {
                if (data[column, row] != null)
                {
                    items.Add(data[column, row].Value);
                }
            }

            for (int row = 0; row < items.Count; row++)
            {
                print($"Gravity Action ROW {row}");
                newData[column, row] = items[row];
            }
        }

        setData(newData);
    }

    private void checkForMatches()
    {
        Item.Type? lastMatch = null;
        List<Coordinate> matchingIndicies = new List<Coordinate>();

        List<Coordinate> coordinatesToDelete = new List<Coordinate>();

        Action clearMatches = () =>
        {
            if (matchingIndicies.Count > 2)
            {
                matchingIndicies.ForEach(x =>
                {
                    coordinatesToDelete.Add(x);
                    print($"To Delete: {x}");
                });
            }

            matchingIndicies.Clear();
        };

        for (int row = 0; row < numberOfRows; row++)
        {
            for (int column = 0; column < numberOfColumns; column++)
            {
                Item? cell = data[column, row];
                if (cell == null)
                {
                    lastMatch = null;
                    clearMatches();
                    continue;
                }

                Item item = cell.Value;

                if (lastMatch == null || lastMatch == item.type)
                {
                    matchingIndicies.Add(new Coordinate(column, row));
                } else
                {
                    clearMatches();
                }


                lastMatch = item.type;
            }
        }

        Item?[,] newData = data;
        coordinatesToDelete.ForEach(coordinate => newData[coordinate.x, coordinate.y] = null);
        setData(newData);
    }

    public void subscribe(OnDataChange f)
    {
        dataChangeDelegate += f;
        f.Invoke(dataMap);
    }

    public bool dataUpdated = false;
    IEnumerator WaitForChange()
    {
        yield return new WaitUntil(() => { return dataUpdated; });
        dataUpdated = false;

        checkForMatches();
        StartCoroutine(WaitForChange());
    }
}
