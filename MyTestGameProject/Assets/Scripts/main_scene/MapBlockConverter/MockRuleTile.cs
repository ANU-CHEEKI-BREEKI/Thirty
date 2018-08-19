using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MockRuleTile : RuleTile
{
    public RuleTile RealTile { get; private set; }

    public event Action<Vector3Int, TileData> OnGetTileData;

    public MockRuleTile(RuleTile baseTile)
    {
        this.m_DefaultSprite = baseTile.m_DefaultSprite;
        this.m_DefaultColliderType = baseTile.m_DefaultColliderType;
        this.m_TilingRules = baseTile.m_TilingRules;
        
        RealTile = baseTile;
    }
       
    public override void GetTileData(Vector3Int position, ITilemap tileMap, ref TileData tileData)
    {
        base.GetTileData(position, tileMap, ref tileData);
        if (OnGetTileData != null)
            OnGetTileData(position, tileData);
    }

    public void ClearEvent()
    {
        OnGetTileData = null;
    }
}
