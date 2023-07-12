using UnityEngine;

public class ABBPrefabInstance : PrefabInstance
{
    public ABBPrefabInstance(int _id, PathAbstractions.AbstractedLocation _location, Vector3i _position, byte _rotation, Prefab _bad, int _standaloneBlockSize) : base(_id, _location, _position, _rotation, _bad, _standaloneBlockSize)
    {
    }

    public new void UpdateImposterView()
    {
        SelectionBox box = GetBox();
        if (box == null)
        {
            Log.Error("PrefabInstance has not SelectionBox! (UIV)");
            return;
        }

        Transform transform = box.transform.Find("PrefabImposter");
        if (transform == null)
        {
            ThreadManager.RunCoroutineSync(prefab.ToTransform(_genBlockModels: true, _genTerrain: true, _genBlockShapes: true, _fillEmptySpace: false, box.transform, "PrefabImposter", new Vector3((float)(-boundingBoxSize.x) / 2f, 0.15f, (float)(-boundingBoxSize.z) / 2f), DynamicPrefabDecorator.PrefabPreviewLimit));
            transform = box.transform.Find("PrefabImposter");
            imposterBaseRotation = lastCopiedRotation;
        }

        int num = MathUtils.Mod(rotation - imposterBaseRotation, 4);
        Vector3 localEulerAngles = transform.localEulerAngles;
        localEulerAngles.y = 90f * (float)num;
        transform.localEulerAngles = localEulerAngles;
        Vector3 localPosition = transform.localPosition;
        localPosition.x = (float)boundingBoxSize.x / 2f * (float)((num >= 2) ? 1 : (-1));
        localPosition.z = (float)boundingBoxSize.z / 2f * (float)((num % 3 != 0) ? 1 : (-1));
        transform.localPosition = localPosition;
        transform.gameObject.SetActive(!IsBBInSyncWithPrefab());
    }
}