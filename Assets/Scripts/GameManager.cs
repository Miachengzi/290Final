using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hertzole.GoldPlayer;

public enum Stage
{
    Preface,
    InHome,
    Ending,
}
public class GameManager : MonoBehaviour
{
    public Stage stage;

    public int collectedClueNum;

    public Transform panel;

    private Dictionary<string, Transform> clueImages = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> clueObjects = new Dictionary<string, Transform>();
    public Text clueNumText;

    public GoldPlayerController playerController;

    private string currentObjName = "";
    private bool isShowPanel = false;
    public int lastShowPanelFrameIndex = -1;

    public AudioClip collectAudioClip;

    private void Awake()
    {
        clueImages.Clear();
        clueImages.Add("Cup", panel.Find("Objects").Find("cup_image"));
        clueImages.Add("Ticket", panel.Find("Objects").Find("ticket_image"));
        clueImages.Add("RC", panel.Find("Objects").Find("rc_image"));
        clueImages.Add("Book", panel.Find("Objects").Find("book_image"));
        clueImages.Add("Photo", panel.Find("Objects").Find("photo_image"));
        clueImages.Add("Envelope", panel.Find("Objects").Find("envelope_image"));
        clueImages.Add("Box", panel.Find("Objects").Find("box_image"));

        clueObjects.Clear();
        clueObjects.Add("Cup", GameObject.Find("cup_clue").transform);
        clueObjects.Add("Ticket", GameObject.Find("ticket_clue").transform);
        clueObjects.Add("RC", GameObject.Find("rc_clue").transform);
        clueObjects.Add("Book", GameObject.Find("book_clue").transform);
        clueObjects.Add("Photo", GameObject.Find("photo_clue").transform);
        clueObjects.Add("Envelope", GameObject.Find("envelope_clue").transform);
        clueObjects.Add("Box", GameObject.Find("storageBox_clue").transform);
    }
    // Start is called before the first frame update
    void Start()
    {
        HideAndLockCursor();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ShowAndFreeCursor();

        if (Cursor.lockState != CursorLockMode.Locked && Input.GetMouseButtonDown(0))
            HideAndLockCursor();

        if (Input.GetMouseButtonDown(0) && lastShowPanelFrameIndex!=-1 && lastShowPanelFrameIndex + 10 < Time.frameCount)
            HidePanelAndAllImage();
    }
    void HideAndLockCursor()
    {
        // 隐藏鼠标指针
        Cursor.visible = false;
        // 锁定鼠标到游戏窗口中心（防止移出屏幕）
        Cursor.lockState = CursorLockMode.Locked;
    }
    void ShowAndFreeCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void EnableObjectPanel(string ObjectName)
    {
        DisactiveAllImage();

        panel.gameObject.SetActive(true);

        if(clueImages[ObjectName] != null)
            clueImages[ObjectName].gameObject.SetActive(true);
        else
        {
            Debug.LogError(ObjectName + " isnt exist");
        }

        // disable current transform's interactable
        DisablePlayerHeadRotate();

        DisableCurrentObjectInteractable(clueObjects[ObjectName].GetComponent<GoldPlayerInteractable>());

        AudioSource.PlayClipAtPoint(collectAudioClip, clueObjects[ObjectName].position);

        currentObjName = ObjectName;
        isShowPanel = true;
        lastShowPanelFrameIndex = Time.frameCount;
    }

    private void DisactiveAllImage()
    {
        foreach (var image in clueImages.Values)
        {
            image.gameObject.SetActive(false);
        }
    }
    private void HidePanelAndAllImage()
    {
        if (isShowPanel)
        {
            panel.gameObject.SetActive(false);
            DisactiveAllImage();
            EnablePlayerHeadRotate();
            // enable current stuff interactable
            Invoke("EnableCurrentObjectInteractable", 0.666f);
            
            isShowPanel=false;
        }
        
    }

    private void EnableCurrentObjectInteractable()
    {
        if (clueObjects.ContainsKey(currentObjName))
            clueObjects[currentObjName].GetComponent<GoldPlayerInteractable>().IsInteractable = true;
    }

    private void DisablePlayerHeadRotate()
    {
        playerController.Camera.CanLookAround = false;
    }

    private void EnablePlayerHeadRotate()
    {
        playerController.Camera.CanLookAround = true;
    }

    private void DisableCurrentObjectInteractable(GoldPlayerInteractable obj)
    {
        obj.IsInteractable = false;
    }
}
