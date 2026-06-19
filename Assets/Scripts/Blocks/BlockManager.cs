using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockManager : MonoBehaviour
{
    public Tilemap tilemap;

    [SerializeField] private float bumpHeight = 0.1f;
    [SerializeField] private float bumpDuration = 0.1f;

    //uses Vector3Int instead of Vector3 because tiles are hwole numbers

    public void HitBlock(Vector3 hitPosition)
    {
        Vector3Int tilePos = tilemap.WorldToCell(hitPosition);
        BlockTile tile = tilemap.GetTile<BlockTile>(tilePos);

        if (tile == null || tile.data == null)
        {
            return;
        }

        BlockData data = tile.data;

        switch (data.blockType)
        {
            case BlockData.BlockType.Normal:
                BumpTile(tilePos);
                break;

            case BlockData.BlockType.OneTime:
                BumpTile(tilePos);
                UseTile(tilePos, data);
                break;

            case BlockData.BlockType.Brick:
                BreakTile(tilePos);
                break;

            case BlockData.BlockType.Solid:
                //nothing
                break;

            case BlockData.BlockType.Dangerous:
                HurtPlayer();
                break;

            case BlockData.BlockType.Special:
                TriggerSpecial(tilePos, data); //just for now
                break;
        }
    }

    private void BumpTile(Vector3Int pos)
    {
        StartCoroutine(BumpCoroutine(pos));
        Debug.Log("RanMatrix");
    }

    private void UseTile(Vector3Int pos, BlockData data)
    {
        if (data.usedSprite != null)
        {
            tilemap.SetTile(pos, null); //remove old tile
            BlockTile newTile = ScriptableObject.CreateInstance<BlockTile>();
            newTile.sprite = data.usedSprite;
            newTile.data = data;
            tilemap.SetTile(pos, newTile);
        }
    }

    private void BreakTile(Vector3Int pos)
    {
        tilemap.SetTile(pos, null);
        //effect
    }

    private void HurtPlayer()
    {
        //hurt the homie
    }

    private void TriggerSpecial(Vector3Int pos, BlockData data)
    {
        //POW, powers, mods, allat
    }

    private IEnumerator BumpCoroutine(Vector3Int pos)
    {
        //tiles original transform matrix
        Matrix4x4 originalMatrix = tilemap.GetTransformMatrix(pos);

        //get original tile pos from matrix
        Vector3 originalPos = originalMatrix.GetColumn(3);

        //get bumped pos
        //everythinge else is zero bump is only vertical
        Vector3 bumpPos = originalPos + new Vector3(0, bumpHeight, 0);

        float x = 0f;
        
        //animate
        LeanTween.value(0f, 1f, bumpDuration).setOnUpdate((float value) =>
        {
            //anim

            x = value;

            Vector3 newPos = Vector3.Lerp(originalPos, bumpPos, x);

            //new matrix
            Matrix4x4 matrix4X4 = originalMatrix;
            matrix4X4.SetColumn(3, new Vector4(newPos.x, newPos.y, newPos.z, 1));

            tilemap.SetTransformMatrix(pos, matrix4X4);
        }
        ).setOnComplete(()=>
        {
            tilemap.SetTransformMatrix(pos, originalMatrix);
        }
        );

        yield return new WaitForSeconds(bumpDuration);
    }
}
