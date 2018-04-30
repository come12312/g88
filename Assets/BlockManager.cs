using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager
{
    private TableBlock[] array_blocks;
    private TableBlock[] array_createBlocks;

    public void Init()
    {
        array_blocks = TableManager.Instance.block.blocks;
        array_createBlocks = new TableBlock[3];
    }

    public void ResetBlock()
    {
        for (int i = 0, count = array_createBlocks.Length; i < count; i++)
            array_createBlocks[i] = null;
    }

    public TableBlock CreateNewBlock(int slotIndex)
    {
        if (array_createBlocks[slotIndex] == null)
            array_createBlocks[slotIndex] = GetBlockDataByIndex(GetCreateBlockIndex());

        return array_createBlocks[slotIndex];
    }

    public TableBlock GetBlockByNumber(int number)
    {
        return array_createBlocks[number];
    }

    public void LoadSavedBlock(int slotindex, int blockindex)
    {
        array_createBlocks[slotindex] = array_blocks[GetBlockIndexDataByIndex(blockindex)];
    }

    public void RemoveBlock(int number)
    {
        array_createBlocks[number] = null;
    }

    public bool IsEnableBlockByIndex(int slotIndex)
    {
        return array_createBlocks[slotIndex] != null;
    }

    public bool IsEnableMoreBlock()
    {
        bool isEnable = false;

        for(int i=0,count = array_createBlocks.Length; i<count; i++)
        {
            if(array_createBlocks[i] != null)
            {
                isEnable = true;
                break;
            }
        }

        return isEnable;
    }

    int GetCreateBlockIndex()
    {
        int currentScore = 0;
        int maxProbabillity = 0;

        for(int i=0,count = TableManager.Instance.block.spawnDatas.Length; i<count; i++)
        {
            if (TableManager.Instance.block.spawnDatas[i].score <= currentScore)
                maxProbabillity += TableManager.Instance.block.spawnDatas[i].probabillity;
        }

        int ranValue = Random.Range(1, maxProbabillity + 1);

        int sumValue = 0;
        int blockIndex = 0;
        int test = 0;
        for (int i = 0, count = TableManager.Instance.block.spawnDatas.Length; i < count; i++)
        {
            sumValue += TableManager.Instance.block.spawnDatas[i].probabillity;
           
            if (ranValue <= sumValue)
            {
                blockIndex = TableManager.Instance.block.spawnDatas[i].index;
                test = i;
                break;
            }
        }

#if ONLY_SQUARE
        blockIndex = 2;
#endif

        return blockIndex;
    }

    TableBlock GetBlockDataByIndex(int blockIndex)
    {
        TableBlock block = null;

        for (int i = 0, count = array_blocks.Length; i < count; i++)
        {
            if (array_blocks[i].index == blockIndex)
            {
                block = array_blocks[i];
                break;
            }
        }

        if (block == null)
            block = array_blocks[0];

        return block;
    }

    int GetBlockIndexDataByIndex(int blockIndex)
    {
        int slotIndex = 0;

        for(int i=0,count = array_blocks.Length; i<count; i++)
        {
            if(array_blocks[i].index == blockIndex)
            {
                slotIndex = i;
                break;
            }
        }

        return slotIndex;
    }
}
