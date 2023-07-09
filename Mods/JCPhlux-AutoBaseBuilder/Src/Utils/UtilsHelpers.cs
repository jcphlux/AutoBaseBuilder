using System;

internal static class UtilsHelpers
{
    internal static BlockFace BlockFaceFromSimpleRotation(byte rotation) => NormalizeSimpleRotation(rotation) switch
    {
        0 => BlockFace.North,
        1 => BlockFace.East,
        2 => BlockFace.South,
        3 => BlockFace.West,
        _ => throw new ArgumentOutOfRangeException(),
    };

    internal static object BlockFaceFromSimpleRotation(int rotation) => BlockFaceFromSimpleRotation((byte)rotation);

    internal static byte SimpleRotationFromBlockFace(BlockFace face) => face switch
    {
        BlockFace.North => 0,
        BlockFace.East => 1,
        BlockFace.South => 2,
        BlockFace.West => 3,
        _ => throw new ArgumentOutOfRangeException(),
    };

    internal static byte NormalizeSimpleRotation(byte rotation) => rotation switch
    {
        27 => 0,
        26 => 1,
        25 => 2,
        24 => 3,
        _ => (byte)(rotation % 4),
    };

    internal static byte NormalizeSimpleRotation(int rotation) => NormalizeSimpleRotation((byte)rotation);

    internal static byte MirrorSimpleRotation(byte rotation) => NormalizeSimpleRotation(rotation) switch
    {
        0 => 2,
        1 => 3,
        2 => 0,
        3 => 1,
        _ => throw new ArgumentOutOfRangeException(),
    };

}