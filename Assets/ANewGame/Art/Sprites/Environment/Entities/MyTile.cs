using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class MyTile : RuleTile<MyTile.Neighbor> {
    public List<TileBase> siblings = new List<TileBase>();
    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Sibling = 3;
    }
    public override bool RuleMatch(int neighbor, TileBase other) {


        // if (other is RuleOverrideTile)
        // {
        //     other = (other as RuleOverrideTile).m_InstanceTile;
        // }

        switch (neighbor) {
            case Neighbor.Sibling: return siblings.Contains(other);
            case Neighbor.This: return other == this || siblings.Contains(other);
            case Neighbor.NotThis: return other != this && !siblings.Contains(other);
        }

        return true;

    }
}