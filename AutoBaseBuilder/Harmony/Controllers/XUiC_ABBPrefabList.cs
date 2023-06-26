using System;
using UnityEngine;


public class XUiC_ABBPrefabList : XUiController
{
    public static string ID = "";
    private XUiC_ABBPrefabFileList fileList;
    private XUiV_Texture prefabPreview;
    private XUiV_Label noPreviewLabel;
    private XUiC_SimpleButton btnPreview;
    private HitInfoDetails blockInfo = new HitInfoDetails();
    private BlockFace prefabFace;
    private ABBPrefabInstance activePrefab;

    public override void Init()
    {
        base.Init();
        XUiC_ABBPrefabList.ID = this.WindowGroup.ID;
        this.prefabPreview = (XUiV_Texture)this.GetChildById("prefabPreview").ViewComponent;
        this.noPreviewLabel = (XUiV_Label)this.GetChildById("noPreview").ViewComponent;
        this.noPreviewLabel.IsVisible = false;
        this.fileList = this.GetChildById("files") as XUiC_ABBPrefabFileList;
        this.btnPreview = this.GetChildById("btnPreview") as XUiC_SimpleButton;
        this.fileList.SelectionChanged += new XUiEvent_ListSelectionChangedEventHandler<XUiC_ABBPrefabFileList.PrefabFileEntry>(this.FileList_SelectionChanged);
        this.fileList.OnEntryDoubleClicked += new XUiC_ABBPrefabFileList.EntryDoubleClickedDelegate(this.FileList_OnEntryDoubleClicked);
        this.fileList.PageNumberChanged += new XUiEvent_ListPageNumberChangedEventHandler(this.FileListOnPageNumberChanged);
        this.btnPreview.OnPressed += new XUiEvent_OnPressEventHandler(this.BtnPreviewOnPressed);

        this.btnPreview.Enabled = false;
    }

    public override void OnOpen()
    {
        base.OnOpen();
        Log.Out("XUiC_ABBPrefabList.OnOpen");
        WorldRayHitInfo hitInfo = this.xui.playerUI.entityPlayer.HitInfo;
        this.blockInfo.CopyFrom(hitInfo.hit);
        switch (this.blockInfo.blockValue.rotation)
        {
            case 0:
            case 27:
                this.prefabFace = BlockFace.North;
                break;
            case 1:
            case 26:
                this.prefabFace = BlockFace.West;
                break;
            case 2:
            case 25:
                this.prefabFace = BlockFace.South;
                break;
            case 3:
            case 24:
                this.prefabFace = BlockFace.East;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        Log.Out("Direction=" + this.prefabFace.ToString() + " Block Rotation=" + this.blockInfo.blockValue.rotation.ToString());

    }

    public override void OnClose()
    {
        base.OnClose();
        this.blockInfo.Clear();
        //this.activePrefab.CleanFromWorld(GameManager.Instance.World, true);
        GameManager.Instance.GetDynamicPrefabDecorator().RemoveActivePrefab(GameManager.Instance.World);
    }

    private void FileListOnPageNumberChanged(int _pageNumber) => this.fileList.SelectedEntryIndex = this.fileList.Page * this.fileList.PageLength;

    private void FileList_SelectionChanged(
      XUiC_ListEntry<XUiC_ABBPrefabFileList.PrefabFileEntry> _previousEntry,
      XUiC_ListEntry<XUiC_ABBPrefabFileList.PrefabFileEntry> _newEntry)
    {
        this.btnPreview.Enabled = _newEntry != null;
        this.updatePreview(_newEntry);
    }

    private void FileList_OnEntryDoubleClicked(XUiC_ABBPrefabFileList.PrefabFileEntry _entry)
    {
        if (!PrefabEditModeManager.Instance.IsActive())
            return;
        this.BtnPreviewOnPressed((XUiController)this, -1);
    }

    private void updatePreview(
    XUiC_ListEntry<XUiC_PrefabFileList.PrefabFileEntry> _newEntry)
    {
        if ((UnityEngine.Object)this.prefabPreview.Texture != (UnityEngine.Object)null)
        {
            Texture2D texture = (Texture2D)this.prefabPreview.Texture;
            this.prefabPreview.Texture = (Texture)null;
            UnityEngine.Object.Destroy((UnityEngine.Object)texture);
        }
        if (_newEntry?.GetEntry() != null)
        {
            string path = _newEntry.GetEntry().location.FullPathNoExtension + ".jpg";
            if (SdFile.Exists(path))
            {
                Texture2D tex = new Texture2D(1, 1, TextureFormat.RGB24, false);
                byte[] data = SdFile.ReadAllBytes(path);
                tex.LoadImage(data);
                this.prefabPreview.Texture = (Texture)tex;
                this.noPreviewLabel.IsVisible = false;
                this.prefabPreview.IsVisible = true;
                return;
            }
        }
        this.noPreviewLabel.IsVisible = true;
        this.prefabPreview.IsVisible = false;
    }

    private void BtnPreviewOnPressed(XUiController _sender, int _mouseButton)
    {
        PathAbstractions.AbstractedLocation location = this.fileList.SelectedEntry.GetEntry().location;
        Prefab selectedPrefab = new();
        selectedPrefab.Load(location);

        int rotCount = 0;
        Vector3i prefabPos = this.blockInfo.blockPos;
        prefabPos.y += selectedPrefab.yOffset;
        switch (this.prefabFace)
        {
            case BlockFace.North:
                prefabPos.z += 1;
                prefabPos.x -= selectedPrefab.size.x / 2;
                rotCount = 0;
                break;
            case BlockFace.West:
                prefabPos.z -= selectedPrefab.size.z / 2;
                prefabPos.x -= selectedPrefab.size.x;
                rotCount = 1;
                break;
            case BlockFace.South:
                prefabPos.z -= selectedPrefab.size.z;
                prefabPos.x -= selectedPrefab.size.x / 2;
                rotCount = 2;
                break;
            case BlockFace.East:
                prefabPos.z -= selectedPrefab.size.z / 2;
                prefabPos.x += 1;
                rotCount = 3;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        this.activePrefab = CreateNewPrefabAndActivate(selectedPrefab.location, prefabPos, selectedPrefab);
        while (rotCount-- > 0)
            this.activePrefab.RotateAroundY();
        //this.activePrefab.CopyIntoWorld(GameManager.Instance.World, false, false, FastTags.none);
        this.activePrefab.UpdateImposterView();


        //this.xui.playerUI.windowManager.Close(XUiC_InGameMenuWindow.ID);
    }

    public ABBPrefabInstance CreateNewPrefabAndActivate(PathAbstractions.AbstractedLocation _location, Vector3i _position, Prefab _bad, bool _bSetActive = true)
    {
        if (_bad == null)
        {
            _bad = new Prefab(new Vector3i(3, 3, 3));
        }
        DynamicPrefabDecorator dpd = GameManager.Instance.GetDynamicPrefabDecorator();

        ABBPrefabInstance prefabInstance = new ABBPrefabInstance(dpd.GetNextId(), _location, _position, 0, _bad, 0);
        prefabInstance.CreateBoundingBox();
        dpd.AddPrefab(prefabInstance);
        if (_bSetActive)
        {
            SelectionBoxManager.Instance.SetActive("DynamicPrefabs", prefabInstance.name, _bActive: true);
        }

        return prefabInstance;
    }
}

