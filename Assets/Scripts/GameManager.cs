using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hertzole.GoldPlayer;
using TMPro;
using UnityEngine.SceneManagement;
using static Hertzole.GoldPlayer.GoldPlayerInteractable;
using UnityEngine.Events;

public enum Stage
{
    Preface,
    InHome,
    Ending,
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Stage stage;    
    public Transform panel;
    public TextMeshProUGUI clueText;
    public Text clueNumText;
    public GoldPlayerController playerController;
    public AudioClip collectAudioClip;

    [SerializeField]
    private string currentObjName = "";
    private bool isShowPanel = false;
    private int lastShowPanelFrameIndex = -1;
    private Dictionary<string, Transform> clueImages = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> clueObjects = new Dictionary<string, Transform>();
    private Dictionary<string, string> clueLines = new Dictionary<string, string>();
    private List<string> collectedObjs = new List<string>();
    private string[] lines;
    private Scene scene;
    
    private void Awake()
    {
        scene = SceneManager.GetActiveScene();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void InitializedInHomeStage()
    {
        panel = GameObject.Find("Canvas").transform.Find("Panel");
        clueText = panel.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        clueNumText = GameObject.Find("Canvas").transform.Find("ClueNumText").GetComponent<Text>();
        playerController = GameObject.Find("Gold Player Controller").GetComponent<GoldPlayerController>();
        collectAudioClip = Resources.Load<AudioClip>("Audio/Collection");

        //revealButton= GameObject.Find("Canvas").transform.Find("Reveal").GetComponent<Button>();
        //revealButton.onClick.RemoveAllListeners();
        //revealButton.gameObject.SetActive(false);

        collectedObjs = new List<string>();
        collectedObjs.Clear();

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

        // 初始化 交互对象
        foreach(var obj in clueObjects)
        {
            var interactorable = obj.Value.GetComponent<GoldPlayerInteractable>();
            string Name = obj.Key;
            interactorable.OnInteract.RemoveAllListeners();
            interactorable.OnInteract.AddListener(() => OnObjectInteracting(Name));
        }

        lines = new string[7];
        lines[0] = "You found two coffee mugs";
        lines[1] = "You found a corrupted Audio system.\n Next to it, a sticky note reads: “Your Shostakovich that day... sounded more beautiful than ever.";
        lines[2] = "You found a journal.\n On one of the pages, it reads:“We are not broken. Even if our parents refuse to understand.”";
        lines[3] = "You found a hospital visit slip.";
        lines[4] = "You found an unfinished letter.\n and the final sentence stops at: “If we ever meet again—”";
        lines[5] = "You found a plane ticket to Los Angeles, a passport, and a concert ticket for a violin recital.";
        lines[6] = "You found a group photo from a violin performance.\n Everyone on stage is smiling and bowing ";

        clueLines.Clear();
        clueLines.Add("Cup", lines[0]);
        clueLines.Add("Ticket", lines[5]);
        clueLines.Add("RC", lines[3]);
        clueLines.Add("Book", lines[2]);
        clueLines.Add("Photo", lines[6]);
        clueLines.Add("Envelope", lines[4]);
        clueLines.Add("Box", lines[1]);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        scene = SceneManager.GetActiveScene();
        if (scene.name == "Preface")
        {
            stage = Stage.Preface;
        }
        else if(scene.name == "Apartment")
        {
            stage = Stage.InHome;
        }
        else if(scene.name == "Ending")
        {
            stage = Stage.Ending;
        }        
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1) // 检查目标场景
        {
            stage = Stage.InHome;
        }
        else if(scene.buildIndex == 2)
        {
            stage = Stage.Ending;
        }
        else
        {
            stage = Stage.Preface;
        }
    }
    void Start()
    {
        HideAndLockCursor();
    }
    // Update is called once per frame
    void Update()
    {
        if(stage == Stage.Preface)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartCoroutine(LoadSceneWithInit(1));
            }
        }
        else if(stage == Stage.InHome)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ShowAndFreeCursor();

            if (Cursor.lockState != CursorLockMode.Locked && Input.GetMouseButtonDown(0))
                HideAndLockCursor();

            if (Input.GetMouseButtonDown(0) && lastShowPanelFrameIndex != -1 && lastShowPanelFrameIndex + 10 < Time.frameCount)
                HidePanelAndAllImage();

            if (Input.GetKeyDown(KeyCode.Return) && collectedObjs.Count == 7 && clueObjects[currentObjName].GetComponent<GoldPlayerInteractable>().IsInteractable)
            {
                SceneManager.LoadScene(2);
            }
        }
        else if(stage == Stage.Ending)
        {

        }
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
    public void OnObjectInteracting(string ObjectName)
    {
        DisactiveAllImage();

        panel.gameObject.SetActive(true);

        if(clueImages[ObjectName] != null)
            clueImages[ObjectName].gameObject.SetActive(true);
        else
        {
            Debug.LogError(ObjectName + " isnt exist");
        }

        clueText.text = clueLines[ObjectName];

        if (!collectedObjs.Contains(ObjectName))
        {
            collectedObjs.Add(ObjectName);
        }

        clueNumText.text = $"{collectedObjs.Count} / 7";

        // disable current transform's interactable
        DisablePlayerHeadRotateAndMovement();

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
            EnablePlayerHeadRotateAndMovement();
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
    private void DisablePlayerHeadRotateAndMovement()
    {
        playerController.Camera.CanLookAround = false;
        playerController.Movement.CanMoveAround = false;
    }
    private void EnablePlayerHeadRotateAndMovement()
    {
        playerController.Camera.CanLookAround = true;
        playerController.Movement.CanMoveAround = true;
    }
    private void DisableCurrentObjectInteractable(GoldPlayerInteractable obj)
    {
        obj.IsInteractable = false;
    }
    IEnumerator LoadSceneWithInit(int sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false; // 禁止自动跳转

        // 等待加载进度达到90%（Unity的诡异设定）
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true; // 激活场景
        yield return new WaitUntil(() => asyncLoad.isDone); // 确保场景完全加载

        // 延迟1帧确保所有对象初始化
        yield return null;
        if(sceneIndex == 1)
            GameManager.Instance.InitializedInHomeStage();
    }
    void OnRevealButtonClicked()
    {
        Debug.Log("按钮点击生效！");
        SceneManager.LoadScene(2);
    }
    void OnDestroy()
    {
        // 移除监听（避免内存泄漏）
        //foreach (var obj in clueObjects)
        //{
        //    var interactorable = obj.Value.GetComponent<GoldPlayerInteractable>();
        //    interactorable.OnInteract.RemoveAllListeners();
        //}
    }
}
