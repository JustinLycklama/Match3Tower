using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;

public struct Item : IEquatable<Item>, IComparable<Item>
{
    public enum Type
    {
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
        return id.Equals(other.id);
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

class GameBoard {







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
        //Item?[,] data = new Item?[numberOfColumns, numberOfRows];

        setData(new Item?[numberOfColumns, numberOfRows]);

        for (int column = 0; column< numberOfColumns; column++)
        {
            itemQList.Add(new Queue<Item>());
        }

        itemQList[0].Enqueue(new Item(Item.Type.one));
        itemQList[0].Enqueue(new Item(Item.Type.one));
        itemQList[0].Enqueue(new Item(Item.Type.one));

        itemQList[1].Enqueue(new Item(Item.Type.two));
        itemQList[2].Enqueue(new Item(Item.Type.one));
        //itemQList[2].Enqueue(new Item(Item.Type.one));


        //data[0, 5] = new Item(Item.Type.one);
        //data[1, 4] = new Item(Item.Type.one);
        //data[1, 5] = new Item(Item.Type.one);
        //data[2, 3] = new Item(Item.Type.one);

        //setData(data);

        //actionList.Enqueue(gravityAction);
        //actionList.Enqueue(checkForMatches);


        //gravityAction();
        //StartCoroutine(Match3GameLoop());
    }

    // Update is called once per frame
    void Update()
    {
        //dataChangeDelegate.Invoke(data);
    }

    Dictionary<Item, Coordinate> dataMap = new Dictionary<Item, Coordinate>();


    void setData(Item?[,] newData)
    {
        bool dataChanged = false;

        if (data != null)
        {
            for (int x = 0; x < numberOfColumns; x++)
            {
                for (int y = 0; y < numberOfRows; y++)
                {
                    if ((!data[x, y].HasValue) && (!newData[x, y].HasValue))
                    {
                        continue;
                    }

                    if (!data[x, y].HasValue || !newData[x, y].HasValue)
                    {
                        dataChanged = true;
                        break;
                    }

                    Item oldItem = data[x, y].Value;
                    Item newItem = newData[x, y].Value;

                    if (!newItem.Equals(oldItem))
                    {
                        dataChanged = true;
                        break;
                    }
                }

                if (dataChanged)
                {
                    break;
                }
            }
        } else
        {
            dataChanged = true;
        }

        data = newData;

        if (dataChanged)
        {
            rebuildDataMap();
            actionList.Enqueue(gravityAction);
            actionList.Enqueue(checkForMatches);
        } else
        {
            //performNextAction();
        }
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

    List<Queue<Item>> itemQList = new List<Queue<Item>>();

    //private void addItemsAction()
    //{
    //    print("Add Items Action");
    //    Item?[,] newData = data;

    //    for (int column = 0; column < numberOfColumns; column++)
    //    {
    //        List<Item> items = new List<Item>();

    //        if (newData[column, numberOfRows - 1] == null &&
    //            itemQList[column].Count > 0)
    //        {
    //            newData[column, numberOfRows - 1] = itemQList[column].Dequeue();
    //        }
    //    }

    //    // DO NOT TRIGGER VIEW UPDATE
    //    data = newData;
    //    gravityAction();
    //}

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

            while (items.Count() < numberOfRows && itemQList[column].Count > 0)
            {
                items.Add(itemQList[column].Dequeue());
            }

            for (int row = 0; row < items.Count; row++)
            {
                //print($"Gravity Action ROW {row}");
                newData[column, row] = items[row];
            }
        }

        setData(newData);
    }

    private void checkForMatches()
    {
        print("Check for Matches Action");

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
                });
            }

            matchingIndicies.Clear();
        };

        // Row Matches
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
                }
                else
                {
                    clearMatches();
                }


                lastMatch = item.type;
            }
        }

        // Column Matches
        for (int column = 0; column < numberOfColumns; column++)
        {
            for (int row = 0; row < numberOfRows; row++)
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
                }
                else
                {
                    clearMatches();
                }


                lastMatch = item.type;
            }
        }

        Item?[,] newData = data.Clone() as Item?[,];

        coordinatesToDelete.ForEach(coordinate => newData[coordinate.x, coordinate.y] = null);
        setData(newData);
    }

    public void subscribe(OnDataChange f)
    {
        dataChangeDelegate += f;
        f.Invoke(dataMap);
    }

    public void performNextAction()
    {
        //print("Perform Action Called");
        getNextAction()?.Invoke();
    }

    Queue<Action> actionList = new Queue<Action>();

    private Action? getNextAction()
    {
        if (actionList.Count == 0)
        {
            return null;
        }

        return actionList.Dequeue();
    }
}
