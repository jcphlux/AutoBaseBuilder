using HarmonyLib;
using System.Reflection;

public class AutoBaseBuilder : IModApi
{
    public void InitMod(Mod mod)
    {
        Log.Out($"JCPhlux AutoBaseBuilder Harmony Patch: {GetType()}");
        Harmony harmony = new(GetType().ToString());
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    //[HarmonyPatch(typeof(BlockToolSelection))]
    //[HarmonyPatch("ExecuteAttackAction")]
    //public class BlockToolSelection_ExecuteAttackAction
    //{
    //    public static bool Prefix(ItemInventoryData _data, bool _bReleased, PlayerActionsLocal playerActions)
    //    {
    //        Log.Out("BlockToolSelection_ExecuteAttackAction" + _data.itemValue.type);
    //        if (_bReleased)
    //            return false;

    // if (_data.itemValue.type == 242 && _data is ItemClassBlock.ItemBlockInventoryData) {
    // Log.Out("BlockToolSelection_ExecuteAttackAction" + _data.itemValue.type); return false; }

    //        return true;
    //    }
    //}

    [HarmonyPatch(typeof(TileEntity))]
    [HarmonyPatch("Instantiate")]
    public class TileEntity_Instantiate
    {
        public static bool Prefix(TileEntityType type, Chunk _chunk, ref TileEntity __result)
        {
            if (type == (TileEntityType)189)
            {
                __result = new TileEntityAutoBaseBuilder(_chunk);
                return false;
            }
            return true;
        }
    }
}