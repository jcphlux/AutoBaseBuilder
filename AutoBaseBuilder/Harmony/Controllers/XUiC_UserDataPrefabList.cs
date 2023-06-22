using System;
using UnityEngine;
using UnityEngine.Scripting;


public class XUiC_UserDataPrefabList : XUiController
{
    public static string ID = "";
    private XUiC_UserDataPrefabFileList fileList;
    //private XUiC_SimpleButton btnLoad;
    //private XUiC_SimpleButton btnProperties;
    //private XUiC_SimpleButton btnSave;
    //private XUiC_SimpleButton btnLoadIntoPrefab;
    //private XUiC_SimpleButton btnApplyLoadedPrefab;
    //private XUiC_SimpleButton btnWorldPlacePrefab;
    //private XUiC_SimpleButton btnWorldReplacePrefab;
    //private XUiC_SimpleButton btnWorldDeletePrefab;
    //private XUiC_SimpleButton btnWorldApplyPrefabChanges;
    //private XUiC_SimpleButton btnWorldRevertPrefabChanges;

    public override void Init()
    {
        base.Init();
        XUiC_UserDataPrefabList.ID = this.WindowGroup.ID;        
        this.fileList = this.GetChildById("files") as XUiC_UserDataPrefabFileList;
        //this.btnLoad = this.GetChildById("btnLoad") as XUiC_SimpleButton;
        //this.btnProperties = this.GetChildById("btnProperties") as XUiC_SimpleButton;
        //this.btnSave = this.GetChildById("btnSave") as XUiC_SimpleButton;
        //this.btnLoadIntoPrefab = this.GetChildById("btnLoadIntoPrefab") as XUiC_SimpleButton;
        //this.btnApplyLoadedPrefab = this.GetChildById("btnApplyLoadedPrefab") as XUiC_SimpleButton;
        //this.groupList.SelectionChanged += new XUiEvent_ListSelectionChangedEventHandler<XUiC_PrefabGroupList.PrefabGroupEntry>(this.GroupListSelectionChanged);
        this.fileList.SelectionChanged += new XUiEvent_ListSelectionChangedEventHandler<XUiC_UserDataPrefabFileList.PrefabFileEntry>(this.FileList_SelectionChanged);
        this.fileList.OnEntryDoubleClicked += new XUiC_UserDataPrefabFileList.EntryDoubleClickedDelegate(this.FileList_OnEntryDoubleClicked);
        this.fileList.PageNumberChanged += new XUiEvent_ListPageNumberChangedEventHandler(this.FileListOnPageNumberChanged);
        //this.btnLoad.OnPressed += new XUiEvent_OnPressEventHandler(this.BtnLoad_OnPressed);
        //this.btnProperties.OnPressed += new XUiEvent_OnPressEventHandler(this.BtnPropertiesOnOnPressed);
        //this.btnSave.OnPressed += new XUiEvent_OnPressEventHandler(this.BtnSave_OnPressed);
        //this.btnLoadIntoPrefab.OnPressed += new XUiEvent_OnPressEventHandler(this.BtnLoadIntoPrefabOnOnPressed);
        //this.btnApplyLoadedPrefab.OnPressed += new XUiEvent_OnPressEventHandler(this.BtnApplyLoadedPrefabOnOnPressed);
        //if (this.GetChildById("btnCleanOtherPrefabs") is XUiC_SimpleButton childById1)
        //    childById1.OnPressed += new XUiEvent_OnPressEventHandler(this.BtnCleanOtherPrefabsOnOnPressed);
        //if (this.GetChildById("btnLoadIntoClipboard") is XUiC_SimpleButton childById2)
        //    childById2.OnPressed += new XUiEvent_OnPressEventHandler(this.BtnLoadIntoClipboardOnOnPressed);
        //if (this.GetChildById("btnNew") is XUiC_SimpleButton childById3)
        //    childById3.OnPressed += new XUiEvent_OnPressEventHandler(this.BtnNewOnOnPressed);
        //this.btnWorldPlacePrefab = this.GetChildById("btnWorldPlacePrefab") as XUiC_SimpleButton;
        //if (this.btnWorldPlacePrefab != null)
        //    this.btnWorldPlacePrefab.OnPressed += new XUiEvent_OnPressEventHandler(this.BtnWorldPlacePrefabOnPressed);
        //this.btnWorldReplacePrefab = this.GetChildById("btnWorldReplacePrefab") as XUiC_SimpleButton;
        //if (this.btnWorldReplacePrefab != null)
        //    this.btnWorldReplacePrefab.OnPressed += new XUiEvent_OnPressEventHandler(this.BtnWorldReplacePrefabOnPressed);
        //this.btnWorldDeletePrefab = this.GetChildById("btnWorldDeletePrefab") as XUiC_SimpleButton;
        //if (this.btnWorldDeletePrefab != null)
        //    this.btnWorldDeletePrefab.OnPressed += new XUiEvent_OnPressEventHandler(this.BtnWorldDeletePrefabOnPressed);
        //this.btnWorldApplyPrefabChanges = this.GetChildById("btnWorldApplyPrefabChanges") as XUiC_SimpleButton;
        //if (this.btnWorldApplyPrefabChanges != null)
        //    this.btnWorldApplyPrefabChanges.OnPressed += new XUiEvent_OnPressEventHandler(this.BtnWorldApplyPrefabChangesOnPressed);
        //this.btnWorldRevertPrefabChanges = this.GetChildById("btnWorldRevertPrefabChanges") as XUiC_SimpleButton;
        //if (this.btnWorldRevertPrefabChanges != null)
        //    this.btnWorldRevertPrefabChanges.OnPressed += new XUiEvent_OnPressEventHandler(this.BtnWorldRevertPrefabChangesOnPressed);
        //this.btnLoad.Enabled = false;
        //this.btnProperties.Enabled = false;
        //this.groupList.SelectedEntryIndex = 0;
    }

    private void GroupListSelectionChanged(
      XUiC_ListEntry<XUiC_PrefabGroupList.PrefabGroupEntry> _previousEntry,
      XUiC_ListEntry<XUiC_PrefabGroupList.PrefabGroupEntry> _newEntry)
    {
        string _filter = (string)null;
        if (_newEntry != null)
            _filter = _newEntry.GetEntry().filterString;
        this.fileList.SetGroupFilter(_filter);
        if (this.fileList.EntryCount <= 0)
            return;
        this.fileList.SelectedEntryIndex = 0;
    }

    private void FileListOnPageNumberChanged(int _pageNumber) => this.fileList.SelectedEntryIndex = this.fileList.Page * this.fileList.PageLength;

    private void FileList_SelectionChanged(
      XUiC_ListEntry<XUiC_UserDataPrefabFileList.PrefabFileEntry> _previousEntry,
      XUiC_ListEntry<XUiC_UserDataPrefabFileList.PrefabFileEntry> _newEntry)
    {
        //this.btnLoad.Enabled = _newEntry != null;
        //this.btnProperties.Enabled = _newEntry != null;
    }

    

    private void FileList_OnEntryDoubleClicked(XUiC_UserDataPrefabFileList.PrefabFileEntry _entry)
    {
        if (!PrefabEditModeManager.Instance.IsActive())
            return;
        this.BtnLoad_OnPressed((XUiController)this, -1);
    }

    private void BtnLoad_OnPressed(XUiController _sender, int _mouseButton)
    {
        if (this.fileList.SelectedEntry?.GetEntry() == null)
            return;
        XUiC_SaveDirtyPrefab.Show(this.xui, new Action<XUiC_SaveDirtyPrefab.ESelectedAction>(this.loadPrefab));
    }

    private void loadPrefab(XUiC_SaveDirtyPrefab.ESelectedAction _action)
    {
        this.xui.playerUI.windowManager.Open(XUiC_InGameMenuWindow.ID, true);
        if (_action == XUiC_SaveDirtyPrefab.ESelectedAction.Cancel)
            return;
        PathAbstractions.AbstractedLocation location = this.fileList.SelectedEntry.GetEntry().location;
        bool flag = XUiC_LevelTools2Window.IsShowImposter(this.xui);
        if (flag && PrefabEditModeManager.Instance.HasPrefabImposter(location))
        {
            PrefabEditModeManager.Instance.LoadImposterPrefab(location);
        }
        else
        {
            if (flag)
            {
                GameManager.ShowTooltip(GameManager.Instance.World.GetLocalPlayers()[0], string.Format(Localization.Get("xuiPrefabsPrefabHasNoImposter"), (object)location.Name));
                XUiC_LevelTools2Window.SetShowImposter(this.xui, false);
            }
            PrefabEditModeManager.Instance.LoadVoxelPrefab(location);
        }
    }

    private void BtnPropertiesOnOnPressed(XUiController _sender, int _mouseButton)
    {
        PathAbstractions.AbstractedLocation location = this.fileList.SelectedEntry.GetEntry().location;
        PathAbstractions.AbstractedLocation loadedPrefab = PrefabEditModeManager.Instance.LoadedPrefab;
        if (location == loadedPrefab)
            XUiC_PrefabPropertiesEditor.Show(this.xui, XUiC_PrefabPropertiesEditor.EPropertiesFrom.LoadedPrefab, PathAbstractions.AbstractedLocation.None);
        else
            XUiC_PrefabPropertiesEditor.Show(this.xui, XUiC_PrefabPropertiesEditor.EPropertiesFrom.FileBrowserSelection, location);
    }

    private void BtnSave_OnPressed(XUiController _sender, int _mouseButton) => XUiC_SaveDirtyPrefab.Show(this.xui, new Action<XUiC_SaveDirtyPrefab.ESelectedAction>(this.savePrefab), XUiC_SaveDirtyPrefab.EMode.ForceSave);

    private void savePrefab(XUiC_SaveDirtyPrefab.ESelectedAction _action) => this.xui.playerUI.windowManager.Open(XUiC_InGameMenuWindow.ID, true);

    private void BtnLoadIntoPrefabOnOnPressed(XUiController _sender, int _mouseButton)
    {
        PathAbstractions.AbstractedLocation location = this.fileList.SelectedEntry.GetEntry().location;
        BlockToolSelection activeBlockTool = GameManager.Instance.GetActiveBlockTool() as BlockToolSelection;
        DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
        Prefab prefab = new Prefab();
        prefab.Load(location, _allowMissingBlocks: true);
        PathAbstractions.AbstractedLocation _location = location;
        Vector3i selectionStart = activeBlockTool.SelectionStart;
        Prefab _bad = prefab;
        dynamicPrefabDecorator.CreateNewPrefabAndActivate(_location, selectionStart, _bad);
        this.xui.playerUI.windowManager.Close(XUiC_InGameMenuWindow.ID);
    }

    private void BtnApplyLoadedPrefabOnOnPressed(XUiController _sender, int _mouseButton)
    {
        DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
        PrefabInstance activePrefab = dynamicPrefabDecorator?.ActivePrefab != null ? dynamicPrefabDecorator.ActivePrefab : (PrefabInstance)null;
        activePrefab.CleanFromWorld(GameManager.Instance.World, true);
        activePrefab.CopyIntoWorld(GameManager.Instance.World, true, false, FastTags.none);
        GameManager.Instance.World.m_ChunkManager.RemoveAllChunksOnAllClients();
    }

    private void BtnCleanOtherPrefabsOnOnPressed(XUiController _sender, int _mouseButton) => throw new NotImplementedException();

    private void BtnLoadIntoClipboardOnOnPressed(XUiController _sender, int _mouseButton)
    {
        PathAbstractions.AbstractedLocation location = this.fileList.SelectedEntry.GetEntry().location;
        BlockToolSelection activeBlockTool = GameManager.Instance.GetActiveBlockTool() as BlockToolSelection;
        Prefab prefab = new Prefab();
        prefab.Load(location, _allowMissingBlocks: true);
        Prefab _prefab = prefab;
        activeBlockTool.LoadPrefabIntoClipboard(_prefab);
        this.xui.playerUI.windowManager.Close(XUiC_InGameMenuWindow.ID);
    }

    private void BtnNewOnOnPressed(XUiController _sender, int _mouseButton) => XUiC_SaveDirtyPrefab.Show(this.xui, new Action<XUiC_SaveDirtyPrefab.ESelectedAction>(this.newPrefab));

    private void newPrefab(XUiC_SaveDirtyPrefab.ESelectedAction _action)
    {
        this.xui.playerUI.windowManager.Open(XUiC_InGameMenuWindow.ID, true);
        if (_action == XUiC_SaveDirtyPrefab.ESelectedAction.Cancel)
            return;
        if (XUiC_LevelTools2Window.IsShowImposter(this.xui))
            XUiC_LevelTools2Window.SetShowImposter(this.xui, false);
        PrefabEditModeManager.Instance.NewVoxelPrefab();
    }

    private void BtnWorldPlacePrefabOnPressed(XUiController _sender, int _mouseButton)
    {
        GameUtils.DirEightWay closestDirection = GameUtils.GetClosestDirection(this.xui.playerUI.entityPlayer.rotation.y, true);
        if (!(GameManager.Instance.GetActiveBlockTool() is BlockToolSelection activeBlockTool) || GameManager.Instance.GetDynamicPrefabDecorator() == null)
            return;
        PathAbstractions.AbstractedLocation location = this.fileList.SelectedEntry.GetEntry().location;
        Prefab _bad = new Prefab();
        _bad.Load(location);
        int num1;
        switch (closestDirection)
        {
            case GameUtils.DirEightWay.N:
                num1 = 2;
                break;
            case GameUtils.DirEightWay.E:
                num1 = 3;
                break;
            case GameUtils.DirEightWay.S:
                num1 = 0;
                break;
            case GameUtils.DirEightWay.W:
                num1 = 1;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        int num2 = MathUtils.Mod(num1 - _bad.rotationToFaceNorth, 4);
        Vector3i vector3i1;
        switch (closestDirection)
        {
            case GameUtils.DirEightWay.N:
                vector3i1 = new Vector3i(-_bad.size.x / 2, 0, 0);
                break;
            case GameUtils.DirEightWay.E:
                vector3i1 = new Vector3i(0, 0, -_bad.size.z / 2);
                break;
            case GameUtils.DirEightWay.S:
                vector3i1 = new Vector3i(-_bad.size.x / 2, 0, 1 - _bad.size.z);
                break;
            case GameUtils.DirEightWay.W:
                vector3i1 = new Vector3i(1 - _bad.size.x, 0, -_bad.size.z / 2);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        Vector3i vector3i2 = vector3i1 with { y = _bad.yOffset };
        PrefabInstance prefabAndActivate = GameManager.Instance.GetDynamicPrefabDecorator().CreateNewPrefabAndActivate(_bad.location, activeBlockTool.SelectionStart + vector3i2, _bad);
        while (num2-- > 0)
            prefabAndActivate.RotateAroundY();
        prefabAndActivate.UpdateImposterView();
        this.xui.playerUI.windowManager.Close(XUiC_InGameMenuWindow.ID);
    }

    private void BtnWorldReplacePrefabOnPressed(XUiController _sender, int _mouseButton)
    {
        if (GameManager.Instance.GetDynamicPrefabDecorator() == null)
            return;
        PrefabInstance prefabInstance = GameManager.Instance.GetDynamicPrefabDecorator().RemoveActivePrefab(GameManager.Instance.World);
        PathAbstractions.AbstractedLocation location = this.fileList.SelectedEntry.GetEntry().location;
        Prefab _bad = new Prefab();
        _bad.Load(location);
        Vector3i _position = prefabInstance.boundingBoxPosition + new Vector3i(0.0f, (float)_bad.yOffset - prefabInstance.yOffsetOfPrefab, 0.0f);
        PrefabInstance prefabAndActivate = GameManager.Instance.GetDynamicPrefabDecorator().CreateNewPrefabAndActivate(_bad.location, _position, _bad);
        while (prefabInstance.rotation-- > (byte)0)
            prefabAndActivate.RotateAroundY();
        prefabAndActivate.UpdateImposterView();
    }

    private void BtnWorldDeletePrefabOnPressed(XUiController _sender, int _mouseButton) => GameManager.Instance.GetDynamicPrefabDecorator().RemoveActivePrefab(GameManager.Instance.World);

    private void BtnWorldApplyPrefabChangesOnPressed(XUiController _sender, int _mouseButton)
    {
        PrefabInstance activePrefab = GameManager.Instance.GetDynamicPrefabDecorator()?.ActivePrefab;
        if (activePrefab == null)
            return;
        activePrefab.CleanFromWorld(GameManager.Instance.World, true);
        activePrefab.CopyIntoWorld(GameManager.Instance.World, true, false, FastTags.none);
        activePrefab.UpdateImposterView();
    }

    private void BtnWorldRevertPrefabChangesOnPressed(XUiController _sender, int _mouseButton)
    {
        PrefabInstance activePrefab = GameManager.Instance.GetDynamicPrefabDecorator()?.ActivePrefab;
        if (activePrefab == null)
            return;
        activePrefab.UpdateBoundingBoxPosAndScale(GameManager.Instance.GetDynamicPrefabDecorator().ActivePrefab.lastCopiedPrefabPosition, activePrefab.prefab.size);
        activePrefab.rotation = activePrefab.lastCopiedRotation;
        activePrefab.UpdateImposterView();
    }

    public override void OnOpen()
    {
        base.OnOpen();
        PathAbstractions.AbstractedLocation _location = PathAbstractions.AbstractedLocation.None;
        if (this.fileList.SelectedEntry != null)
            _location = this.fileList.SelectedEntry.GetEntry().location;
        //if (this.groupList.SelectedEntry != null)
        //{
        //    string name = this.groupList.SelectedEntry.GetEntry().name;
        //    this.groupList.RebuildList(false);
        //    if (!this.groupList.SelectByName(name))
        //        this.groupList.SelectedEntryIndex = 0;
        //}
        this.fileList.RebuildList(false);
        if (_location.Type != PathAbstractions.EAbstractedLocationType.None)
            this.fileList.SelectByLocation(_location);
        this.RefreshBindings();
    }

    public override void Update(float _dt)
    {
        base.Update(_dt);
        //this.btnSave.Enabled = PrefabEditModeManager.Instance.VoxelPrefab != null;
        BlockToolSelection activeBlockTool = GameManager.Instance.GetActiveBlockTool() as BlockToolSelection;
        DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
        PrefabInstance activePrefab = dynamicPrefabDecorator?.ActivePrefab;
        //this.btnLoadIntoPrefab.Enabled = this.fileList.SelectedEntry != null && activeBlockTool != null && activeBlockTool.SelectionActive && dynamicPrefabDecorator != null;
        //this.btnApplyLoadedPrefab.Enabled = activePrefab != null && !activePrefab.IsBBInSyncWithPrefab();
        //this.btnWorldPlacePrefab.Enabled = activeBlockTool != null && activeBlockTool.SelectionActive && this.fileList.SelectedEntry != null;
        //this.btnWorldReplacePrefab.Enabled = activePrefab != null && this.fileList.SelectedEntry != null;
        //this.btnWorldDeletePrefab.Enabled = activePrefab != null;
        //this.btnWorldApplyPrefabChanges.Enabled = activePrefab != null && !activePrefab.IsBBInSyncWithPrefab();
        //this.btnWorldRevertPrefabChanges.Enabled = activePrefab != null && !activePrefab.IsBBInSyncWithPrefab() && activePrefab.bPrefabCopiedIntoWorld;
    }
}

