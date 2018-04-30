using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TenTenManager
{
    public BlockManager blockManager;
    private MapManager mapManager;

    public UnityAction<int, TableBlock> action_create_blocks;
    public UnityAction<int, TableTile[], int, int> action_tile_add_map;
    public UnityAction<int> action_remove_block;

    private RemoveLineData[] m_array_removeLine = new RemoveLineData[6];

    public void Init()
    {
        blockManager = new BlockManager();
        mapManager = new MapManager();

        blockManager.Init();
        mapManager.Init();
    }

    public void ResetMap()
    {
        mapManager.ReSetMap();
    }

    public void ResetBlock()
    {
        blockManager.ResetBlock();
    }

    public void LoadSavedGame()
    {
        for (int x = 0, xcount = DataManager.Instance.m_array_map_tile_data.Length; x < xcount; x++)
        {
            for (int y = 0, ycount = DataManager.Instance.m_array_map_tile_data[x].Length; y < ycount; y++)
            {
                if (DataManager.Instance.m_array_map_tile_data[x][y].isEnable)
                    mapManager.TileAddMap(x, y);
            }
        }
        
        for(int i=0,count = DataManager.Instance.m_array_wait_block_data.Length; i<count; i++)
        {
            if (DataManager.Instance.m_array_wait_block_data[i].isEnable)
                blockManager.LoadSavedBlock(i, DataManager.Instance.m_array_wait_block_data[i].index);
        }
    }

    public bool IsEnableMoreBlock()
    {
        return blockManager.IsEnableMoreBlock();
    }

    public void CheckEndCreateBlock()
    {
        for (int i = 0, count = 3; i < count; i++)
        {
            if (blockManager.IsEnableBlockByIndex(i) == false)
            {
                CreateNewBlock(i);
            }
        }
    }

    public void CreateNewBlock(int slotIndex)
    {
        TableBlock new_block = blockManager.CreateNewBlock(slotIndex);

        if (action_create_blocks != null)
            action_create_blocks(slotIndex, new_block);
    }

    public bool IsEnablePutBlock(int slotIndex)
    {
        TableBlock block = blockManager.GetBlockByNumber(slotIndex);
        if (block == null) return false;

        return mapManager.IsSetAnyBlock(block.tile);
    }

    public bool IsEnablePutAnyBlock()
    {
        bool isEnableAny = false;

        for(int i=0,count = 3;i<count; i++)
        {
            TableBlock block = blockManager.GetBlockByNumber(i);
            if (block == null) continue;

            if (mapManager.IsSetAnyBlock(block.tile))
            {
                isEnableAny = true;
                break;
            }
        }

        return isEnableAny;
    }

    public bool IsEnablePutBlock(int blockIndex, int x, int y)
    {
        return IsEnablePutBlock(blockManager.GetBlockByNumber(blockIndex).tile, x, y);
    }

    public bool IsEnablePutBlock(TableTile[] array_tiles, int x, int y)
    {
        return mapManager.IsEnableBlock(array_tiles, x, y);
    }

    public bool IsCheckOutOfMap(TableTile[] array_tiles, int x, int y)
    {
        return mapManager.IsCheckOutOfMap(array_tiles, x, y);
    }

    public void PutBlock(int blockIndex, int x, int y)
    {
        mapManager.TileAddMap(blockManager.GetBlockByNumber(blockIndex).tile, x, y);

        if (action_tile_add_map != null)
            action_tile_add_map(blockIndex, blockManager.GetBlockByNumber(blockIndex).tile, x, y);
    }

    public void RemoveBlock(int blockIndex)
    {
        blockManager.RemoveBlock(blockIndex);

        if (action_remove_block != null)
            action_remove_block(blockIndex);
    }

    public bool IsLineFull(LINE_DIRECTION direction, int posi)
    {
        return mapManager.IsLineFull(direction, posi);
    }

    public bool IsClear()
    {
        return mapManager.IsClear();
    }

    public void RemoveTile(TableTile[] tiles, int posiX, int posiY)
    {
        mapManager.RemoveTile(tiles, posiX, posiY);
    }

    public RemoveLineData[] GetEnableRemoveLine(int blockIndex, int posiX, int posiY)
    {
        mapManager.TileAddMap(blockManager.GetBlockByNumber(blockIndex).tile, posiX, posiY);

        int minX, maxX, minY, maxY;
        GetBlocksMinAndMax(blockManager.GetBlockByNumber(blockIndex).tile, posiX, posiY, out minX, out maxX, out minY, out maxY);

        for (int i = 0, count = m_array_removeLine.Length; i < count; i++)
            m_array_removeLine[i] = new RemoveLineData(false);

        int index = 0;

        for (int x = minX; x <= maxX; x++)
        {
            if (IsLineFull(LINE_DIRECTION.VERTICAL, x))
            {
                RemoveLineData removeData = new RemoveLineData(LINE_DIRECTION.VERTICAL, x, true);
                m_array_removeLine[index] = new RemoveLineData(removeData);
                index++;
            }
        }

        for (int y = minY; y <= maxY; y++)
        {
            if (IsLineFull(LINE_DIRECTION.HORIZONTAL, y))
            {
                RemoveLineData removeData = new RemoveLineData(LINE_DIRECTION.HORIZONTAL, y, true);
                m_array_removeLine[index] = new RemoveLineData(removeData);
                index++;
            }
        }

        mapManager.RemoveTile(blockManager.GetBlockByNumber(blockIndex).tile, posiX, posiY);

        return m_array_removeLine;
    }

    public RemoveLineData[] GetRemoveLine(TableTile[] tiles, int posiX, int posiY)
    {
        int minX, maxX, minY, maxY;
        GetBlocksMinAndMax(tiles, posiX, posiY, out minX, out maxX, out minY, out maxY);

        for (int i = 0, count = m_array_removeLine.Length; i < count; i++)
            m_array_removeLine[i] = new RemoveLineData(false);

        int index = 0;

        for (int x = minX; x <= maxX; x++)
        {
            if (IsLineFull(LINE_DIRECTION.VERTICAL, x))
            {
                RemoveLineData removeData = new RemoveLineData(LINE_DIRECTION.VERTICAL, x, true);
                m_array_removeLine[index] = new RemoveLineData(removeData);
                index++;
            }
        }

        for (int y = minY; y <= maxY; y++)
        {
            if (IsLineFull(LINE_DIRECTION.HORIZONTAL, y))
            {
                RemoveLineData removeData = new RemoveLineData(LINE_DIRECTION.HORIZONTAL, y, true);
                m_array_removeLine[index] = new RemoveLineData(removeData);
                index++;
            }
        }

        RemoveLine(m_array_removeLine);

        return m_array_removeLine;
    }

    void RemoveLine(RemoveLineData[] list_removeLine)
    {
        if (list_removeLine[0].isEnable == false) return;

        for (int i = 0, count = list_removeLine.Length; i < count; i++)
        {
            if (list_removeLine[i].isEnable == false) break;

            mapManager.RemoveLine(list_removeLine[i].direction, list_removeLine[i].posi);
        }
    }

    public void GetBlocksMinAndMax(TableTile[] tiles, int posiX, int posiY, out int minX, out int maxX, out int minY, out int maxY)
    {
        minX = tiles[0].x + posiX;
        maxX = tiles[0].x + posiX;
        minY = tiles[0].y + posiY;
        maxY = tiles[0].y + posiY;

        for (int i = 1, count = tiles.Length; i < count; i++)
        {
            if (minX > tiles[i].x + posiX)
                minX = tiles[i].x + posiX;
            if (maxX < tiles[i].x + posiX)
                maxX = tiles[i].x + posiX;
            if (minY > tiles[i].y + posiY)
                minY = tiles[i].y + posiY;
            if (maxY < tiles[i].y + posiY)
                maxY = tiles[i].y + posiY;
        }
    }

    public List<TableTile> GetLocatedTiles()
    {
        return mapManager.GetLocatedTiles();
    }
}
