using UnityEngine;

[CreateAssetMenu(menuName = "Blocks/Block Data")]
public class BlockData : ScriptableObject
{
    public enum BlockType
    {
        Normal,
        OneTime,
        Brick,
        Solid,
        JumpThrough,
        Dangerous,
        Special
    }

    public BlockType blockType;

    public bool canBeBumped = true;
    public bool canBreak = false;
    public bool oneTimeUse = false;
    public bool hurtsPlayer = false;
    public bool canJumpThrough = false;

    public Sprite usedSprite;
}
