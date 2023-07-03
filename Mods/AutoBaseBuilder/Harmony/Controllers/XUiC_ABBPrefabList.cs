using Platform;
using System;
using UnityEngine;


public class XUiC_ABBPrefabList : XUiController
{
    public static string ID = "";
    private XUiC_ABBPrefabFileList fileList;
    private XUiV_Texture prefabPreview;
    private XUiV_Label noPreviewLabel;
    private XUiC_SimpleButton btnPreview;
    private Vector3i prefabOffset = Vector3i.zero;
    private byte prefabRotation = 0;
    private ABBPrefabInstance prefabInstance;
    private TileEntityAutoBaseBuilder tileEntity;
    public PlayerActionsLocal playerInput => PlatformManager.NativePlatform.Input.PrimaryPlayer;
    private byte Rotation => tileEntity?.blockValue.rotation switch
    {
        0 or 27 => 0,
        1 or 26 => 1,
        2 or 25 => 2,
        3 or 24 => 3,
        _ => throw new ArgumentOutOfRangeException(),
    };

    public override void Init()
    {
        base.Init();
        ID = WindowGroup.ID;
        prefabPreview = (XUiV_Texture)GetChildById("prefabPreview").ViewComponent;
        noPreviewLabel = (XUiV_Label)GetChildById("noPreview").ViewComponent;
        noPreviewLabel.IsVisible = false;
        fileList = GetChildById("files") as XUiC_ABBPrefabFileList;
        btnPreview = GetChildById("btnPreview") as XUiC_SimpleButton;
        fileList.SelectionChanged += new XUiEvent_ListSelectionChangedEventHandler<XUiC_ABBPrefabFileList.PrefabFileEntry>(OnEntrySelectionChanged);
        fileList.OnEntryDoubleClicked += new XUiC_ABBPrefabFileList.EntryDoubleClickedDelegate(OnEntryDoubleClicked);
        fileList.PageNumberChanged += new XUiEvent_ListPageNumberChangedEventHandler(OnPageNumberChanged);
        btnPreview.OnPressed += new XUiEvent_OnPressEventHandler(OnPreviewPressed);
        btnPreview.Enabled = false;
    }

    public void SetTileEntity(TileEntityAutoBaseBuilder _tileEntity)
    {
        tileEntity = _tileEntity;

        if (tileEntity.prefabLocation != null & fileList.SelectByLocation(tileEntity.prefabLocation))
        {
            prefabOffset = _tileEntity.prefabOffset;
            prefabRotation = _tileEntity.prefabRotation;
            OnEntrySelectionChanged(null, fileList.SelectedEntry);
            OnPreviewPressed(this, -1);
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        ClearPrefab();
        btnPreview.Enabled = fileList.SelectedEntry != null;
    }

    private void OnPageNumberChanged(int _pageNumber) => fileList.SelectedEntryIndex = fileList.Page * fileList.PageLength;

    private void OnEntrySelectionChanged(
      XUiC_ListEntry<XUiC_ABBPrefabFileList.PrefabFileEntry> _previousEntry,
      XUiC_ListEntry<XUiC_ABBPrefabFileList.PrefabFileEntry> _newEntry)
    {
        btnPreview.Enabled = _newEntry != null;
        ClearPrefab();
        if (prefabPreview.Texture != null)
        {
            Texture2D texture = (Texture2D)prefabPreview.Texture;
            prefabPreview.Texture = null;
            UnityEngine.Object.Destroy(texture);
        }
        if (_newEntry?.GetEntry() != null)
        {
            string path = _newEntry.GetEntry().location.FullPathNoExtension + ".jpg";
            if (SdFile.Exists(path))
            {
                Texture2D tex = new(1, 1, TextureFormat.RGB24, false);
                byte[] data = SdFile.ReadAllBytes(path);
                tex.LoadImage(data);
                prefabPreview.Texture = tex;
                noPreviewLabel.IsVisible = false;
                prefabPreview.IsVisible = true;
                return;
            }
        }
        noPreviewLabel.IsVisible = true;
        prefabPreview.IsVisible = false;
    }

    private void OnEntryDoubleClicked(XUiC_ABBPrefabFileList.PrefabFileEntry _entry)
    {
        if (!btnPreview.Enabled)
            return;
        OnPreviewPressed(this, -1);
    }

    private void OnPreviewPressed(XUiController _sender, int _mouseButton)
    {
        PathAbstractions.AbstractedLocation location = fileList.SelectedEntry.GetEntry().location;
        LoadPrefab(location);
        //prefabInstance.SetBoundingBoxPosition(prefabPos + Vector3i.right);
    }

    protected void Update()
    {
        if (prefabInstance == null)
        {
            Log.Out("prefabInstance is null");
            return;
        }
        Vector3i move = Vector3i.zero;


        if (this.playerInput.MoveBack.WasPressed)
            move += -1 * Vector3i.forward;
        if (this.playerInput.MoveForward.WasPressed)
            move += Vector3i.forward;
        if (this.playerInput.MoveLeft.WasPressed)
            move += -1 * Vector3i.right;
        if (this.playerInput.MoveRight.WasPressed)
            move += Vector3i.right;

        prefabInstance.MoveBoundingBox(move);
        prefabOffset += move;
        //if (Input.GetKeyDown(KeyCode.LeftArrow))

        //foreach (PlayerAction action in playerInput.GUIActions.Actions)
        //{
        //    if (action.WasPressed)
        //        Log.Out("Player Input :" + action.Name);
        //}

    }

    private void LoadPrefab(PathAbstractions.AbstractedLocation location)
    {
        Prefab selectedPrefab = new();
        selectedPrefab.Load(location);

        Vector3i prefabPos = GetPrefabPos(selectedPrefab);
        DynamicPrefabDecorator dpd = GameManager.Instance.GetDynamicPrefabDecorator();
        prefabInstance = new(dpd.GetNextId(), selectedPrefab.location, prefabPos, 0, selectedPrefab, 0);
        prefabInstance.CreateBoundingBox();
        dpd.AddPrefab(prefabInstance);

        SelectionBoxManager.Instance.SetActive("DynamicPrefabs", prefabInstance.name, _bActive: true);

        int rotCount = Rotation;
        while (rotCount-- > 0)
            prefabInstance.RotateAroundY();
        prefabInstance.UpdateImposterView();
        btnPreview.Enabled = false;
    }

    private void ClearPrefab()
    {
        prefabInstance?.CleanFromWorld(GameManager.Instance.World, true);
        DynamicPrefabDecorator dpd = GameManager.Instance.GetDynamicPrefabDecorator();
        dpd.RemoveActivePrefab(GameManager.Instance.World);
        prefabOffset = Vector3i.zero;
        prefabInstance = null;
    }

    private Vector3i GetPrefabPos(Prefab _prefab)
    {
        Vector3i prefabPos = tileEntity.ToWorldPos();
        prefabPos.y += _prefab.yOffset;

        int v = _prefab.rotationToFaceNorth % 2;
        switch (Rotation)
        {
            case 0:
                prefabPos.z += 1;
                prefabPos.x -= (v == 0 ? _prefab.size.x : _prefab.size.z) / 2;
                break;
            case 1:
                prefabPos.z -= (v == 0 ? _prefab.size.x : _prefab.size.z) / 2;
                prefabPos.x += 1;
                break;
            case 2:
                prefabPos.z -= v != 0 ? _prefab.size.x : _prefab.size.z;
                prefabPos.x -= (v == 0 ? _prefab.size.x : _prefab.size.z) / 2;
                break;
            case 3:
                prefabPos.z -= (v == 0 ? _prefab.size.x : _prefab.size.z) / 2;
                prefabPos.x -= v != 0 ? _prefab.size.x : _prefab.size.z;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return prefabPos + prefabOffset;
    }
}

