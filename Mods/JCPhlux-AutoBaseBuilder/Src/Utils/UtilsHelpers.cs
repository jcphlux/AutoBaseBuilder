using System;
using System.Collections.Generic;
using UnityEngine;

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


    internal static bool CanBuildAtPosition(Vector3i pos) => GameManager.Instance.World.CanPlaceBlockAt(pos, GameManager.Instance.World.GetGameManager().GetPersistentLocalPlayer(), false);

    internal static bool CanBuildAtPosition(
        World world,
        Chunk chunk,
        Vector3i blockPos,
        PersistentPlayerData lpRelative,
        int claimSize,
        bool includeAllies)
    {

        Vector3i worldPos = chunk.GetWorldPos();
        // Check if block to be repaired is within a trader area?
        if (world.IsWithinTraderArea(worldPos + blockPos)) return false;

        foreach (var player in world.gameManager.GetPersistentPlayerList().Players)
        {

            PersistentPlayerData playerData = player.Value;
            // PlatformUserIdentifierAbs playerId = player.Key;

            // First check if user is not myself
            if (lpRelative != playerData)
            {
                // Check if allies should be considered and if ACL is there
                if (includeAllies == false || playerData.ACL == null) continue;
                // Now check the actual ACL if player is allied with ourself
                if (!playerData.ACL.Contains(lpRelative.UserIdentifier)) continue;
            }

            // Get all land-claim blocks of the allied user (or our-self)
            if (player.Value.GetLandProtectionBlocks() is List<Vector3i> claimPositions)
            {
                for (int i = 0; i < claimPositions.Count; ++i)
                {
                    // Fetch block value at position where claim block should be
                    BlockValue blockValue = world.GetBlock(claimPositions[i]);
                    // The "primary" flag is encoded in `blockValue.meta`
                    if (BlockLandClaim.IsPrimary(blockValue))
                    {
                        // Now check if the block is inside the range
                        if (Mathf.Abs(claimPositions[i].x - blockPos.x) > claimSize) continue;
                        if (Mathf.Abs(claimPositions[i].z - blockPos.z) > claimSize) continue;
                        // Block within my claim
                        return true;
                    }
                }
            }

        }

        // Not inside my claim
        return false;
    }

}