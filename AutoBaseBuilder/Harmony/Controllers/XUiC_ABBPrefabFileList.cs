using System.Collections.Generic;


public class XUiC_ABBPrefabFileList : XUiC_PrefabFileList
{
    private readonly List<PathAbstractions.AbstractedLocation> prefabSearchList = new List<PathAbstractions.AbstractedLocation>();

    public override void Init()
    {
        base.Init();
    }

    public override void RebuildList(bool _resetFilter = false)
    {
        this.allEntries.Clear();
        this.prefabSearchList.Clear();
        foreach (PathAbstractions.AbstractedLocation availablePaths in PathAbstractions.PrefabsSearchPaths.GetAvailablePathsList())
            if (availablePaths.Type == PathAbstractions.EAbstractedLocationType.UserDataPath)
                this.allEntries.Add(new XUiC_PrefabFileList.PrefabFileEntry(availablePaths));

        this.allEntries.Sort();
        SelectedEntry = null;
        RefreshView(_resetFilter);

    }
}

