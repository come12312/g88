using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    public bool[][] array_maps;
    private int maxX, maxY;

    private List<TableTile> list_removeTiles = new List<TableTile>();
    private List<TableTile> list_located_tiles = new List<TableTile>();

    public void Init()
    {
        maxX = TableManager.Instance.map_max_X;
        maxY = TableManager.Instance.map_max_Y;

        array_maps = new bool[maxX][];

        for (int x = 0, xcount = maxX; x < xcount; x++)
        {
            array_maps[x] = new bool[maxY];
        }
    }

    public void ReSetMap()
    {
        for (int x = 0, xcount = maxX; x < xcount; x++)
        {
            for (int y = 0, ycount = maxY; y < ycount; y++)
            {
                array_maps[x][y] = false;
            }
        }
    }

    public bool IsSetAnyBlock(TableTile[] tileData)
    {
        for (int x = 0, xcount = maxX; x < xcount; x++)
        {
            for (int y = 0, ycount = maxY; y < ycount; y++)
            {
                //기준 값이 비어있는 경우가 있어 일단 검사 제외
                //if(array_maps[x][y] == false)
                {
                    if (IsEnableBlock(tileData, x, y))
                        return true;
                }
            }
        }

        return false;
    }

    public bool IsEnableBlock(TableTile[] tileData, int x, int y)
    {
        for (int i = 0, count = tileData.Length; i < count; i++)
        {
            if (IsCheckOutOfMap(tileData, x, y))
                return false;

            if (array_maps[tileData[i].x + x][tileData[i].y + y])
                return false;
        }

        return true;
    }

    //범위 밖 체크
    public bool IsCheckOutOfMap(TableTile[] tileData, int x, int y)
    {
        for (int i = 0, count = tileData.Length; i < count; i++)
        {
            if ((tileData[i].y + y < 0)
                || (tileData[i].x + x < 0)
                || (tileData[i].y + y >= maxY)
                || (tileData[i].x + x >= maxX))
                return true;
        }

        return false;
    }

    public void TileAddMap(TableTile[] tileData, int x, int y)
    {
        for (int i = 0, count = tileData.Length; i < count; i++)
        {
            TileAddMap(tileData[i].x + x, tileData[i].y + y);
        }
    }

    public void TileAddMap(int x, int y)
    {
        if (array_maps[x][y] == true)
            Debug.LogError("Already Added");

        array_maps[x][y] = true;
    }

    public bool IsClear()
    {
        for (int x = 0, xcount = maxX; x < xcount; x++)
        {
            for (int y = 0, ycount = maxY; y < ycount; y++)
            {
                if(array_maps[x][y] == true)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool IsLineFull(LINE_DIRECTION direction, int posi)
    {
        switch (direction)
        {
            case LINE_DIRECTION.VERTICAL:
                for (int y = 0, ycount = maxY; y < ycount; y++)
                {
                    if (array_maps[posi][y] == false)
                        return false;
                }
                break;
            case LINE_DIRECTION.HORIZONTAL:
                for (int x = 0, xcount = maxX; x < xcount; x++)
                {
                    if (array_maps[x][posi] == false)
                        return false;
                }
                break;
        }

        return true;
    }

    public void RemoveLine(LINE_DIRECTION direction, int posi)
    {
        switch (direction)
        {
            case LINE_DIRECTION.VERTICAL:
                for (int y = 0, ycount = maxY; y < ycount; y++)
                {
                    RemoveTile(posi, y);
                }
                break;
            case LINE_DIRECTION.HORIZONTAL:
                for (int x = 0, xcount = maxX; x < xcount; x++)
                {
                    RemoveTile(x, posi);
                }
                break;
        }
    }

    public void RemoveTile(TableTile[] tileData, int x, int y)
    {
        for (int i=0,count = tileData.Length; i<count; i++)
        {
            RemoveTile(tileData[i].x + x, tileData[i].y + y);
        }
    }

    void RemoveTile(int x, int y)
    {
        array_maps[x][y] = false;
    }

    public List<TableTile> GetLocatedTiles()
    {
        list_located_tiles.Clear();

        for (int x = 0, xcount = array_maps.Length; x<xcount; x++)
        {
            for(int y=0,ycout = array_maps[x].Length; y<ycout; y++)
            {
                if(array_maps[x][y] == true)
                {
                    list_located_tiles.Add(new TableTile(x, y));
                }
            }
        }

        return list_located_tiles;
    }
}
