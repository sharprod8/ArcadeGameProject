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

    [SerializeField] private TileBase solidBlockTile;

    //uses Vector3Int instead of Vector3 because tiles are hwole numbers

    public void HitBlock(Vector3 hitPosition)
    {
        Vector3 slightlyHigherHitPosition = hitPosition + new Vector3(0f, tilemap.cellSize.y * 0.5f, 0f);
        Vector3Int tilePos = tilemap.WorldToCell(slightlyHigherHitPosition);

        BlockTile tile = tilemap.GetTile<BlockTile>(tilePos);

        if (tile == null || tile.data == null)
        {
            Debug.Log("Tile considerd null");
            return;
        }

        BlockData data = tile.data;

        switch (data.blockType)
        {
            case BlockData.BlockType.Normal:
                BumpTile(tilePos);
                Debug.Log("normal");
                break;

            case BlockData.BlockType.OneTime:
                BumpTile(tilePos);
                tilemap.SetTile(tilePos, solidBlockTile);
                break;

            case BlockData.BlockType.Brick:
                BumpTile(tilePos);
                BreakTile(tilePos);
                break;

            case BlockData.BlockType.Solid:
                //nothing
                break;

            case BlockData.BlockType.Dangerous:
                BumpTile(tilePos);
                HurtPlayer();
                break;

            case BlockData.BlockType.Special:
                BumpTile(tilePos);
                TriggerSpecial(tilePos, data); //just for now
                break;
        }
    }

    private void BumpTile(Vector3Int pos)
    {
        StartCoroutine(BumpCoroutine(pos));
        Debug.Log("Ran Matrix");
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

    private void JumpThroughTile()
    {
        if (tilemap == null)
            return;


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

        float x = bumpDuration / 2f;
        //to animate first half up and second half down
        
        //animate
        LeanTween.value(0f, 1f, x).setOnUpdate((float value) =>
        {
            Vector3 newPos = Vector3.Lerp(originalPos, bumpPos, value);

            //new matrix
            Matrix4x4 matrix4X4 = originalMatrix;
            matrix4X4.SetColumn(3, new Vector4(newPos.x, newPos.y, newPos.z, 1));

            //set matrix and pos
            tilemap.SetTransformMatrix(pos, matrix4X4);
        }
        ).setOnComplete(()=>
        {
            LeanTween.value(0f, 1f, x).setOnUpdate((float value) =>
            {
                Vector3 newPos = Vector3.Lerp(bumpPos, originalPos, value);

                //new matrix
                Matrix4x4 matrix4X4 = originalMatrix;
                matrix4X4.SetColumn(3, new Vector4(newPos.x, newPos.y, newPos.z, 1));

                //set matrix and pos
                tilemap.SetTransformMatrix(pos, matrix4X4);
            }
            ).setOnComplete(() =>
            {
                tilemap.SetTransformMatrix(pos, originalMatrix);
            });
        }
        );

        yield return new WaitForSeconds(bumpDuration);
        tilemap.SetTransformMatrix(pos, originalMatrix);
    }
}
