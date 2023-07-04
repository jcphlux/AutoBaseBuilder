using HarmonyLib;
using System.Reflection;

public class JCPhluxControls : IModApi
{
    public void InitMod(Mod mod)
    {
        Log.Out("JCPhlux Controls Harmony Patch: " + GetType().ToString());
        Harmony harmony = new Harmony(GetType().ToString());
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

}
