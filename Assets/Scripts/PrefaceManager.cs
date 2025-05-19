using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Scythe.Accessibility;
using VolFx;
using UnityEngine.Rendering;

public class PrefaceManager : MonoBehaviour
{
    public AudioSource audioSource;

    public Transform player;

    public PostProcessVolume postVolume;

    public VolFx.VolFx volFx;

    public VolumeProfile profile;

    private AdjustmentsVol adjustment;

    private ColorGrading colorGrading;

    [SerializeField] SubtitleCard subtitleCard;

    public GameObject canvas;
    public enum State
    {
        None,
        Start,
        PlayingPreface,
        FadingOut,
        LoadFirstScene,
    } public State state;

    public float fadeStrength = 1.0f; // 缩放强度（相对于原始尺寸）
    public float fadeSpeed = 3.1f;    // 缩放速度

    private float originalValue;

    // Start is called before the first frame update
    void Start()
    {
        state = State.None;

        adjustment = (AdjustmentsVol)profile.components.Find(component => component.name == "Adjustments" || component.GetType() == typeof(AdjustmentsVol));
        adjustment.m_Brightness.value = 0f;

        if (postVolume.profile.TryGetSettings(out colorGrading))
        {
            // 初始化参数（可选）
            originalValue = colorGrading.postExposure.value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(player.position.z > 3 && state == State.Start)
        {
            audioSource.Play();
            audioSource.time = 0;
            if (GameManager.Instance.stage == Stage.Ending)
            {
                SubtitleManager.instance.CueSubtitle(subtitleCard);
                SubtitleManager.instance.OnSubtitleFinished.AddListener(() => state = State.LoadFirstScene);
            }

            state = State.PlayingPreface;
        }
        if (state == State.PlayingPreface)
        {
            if(GameManager.Instance.stage == Stage.Preface && audioSource.time > 42.8f)
            {
                state = State.FadingOut;
            }
        }
        if (state == State.LoadFirstScene || Input.GetKeyDown(KeyCode.A))
        {
            GameManager.Instance.LoadSceneWithIndex(0);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            GameManager.Instance.LoadSceneWithIndex(1);
        }
    }
    private void FixedUpdate()
    {
        if (state == State.FadingOut)
        {
            var value = (Time.fixedDeltaTime * fadeSpeed) * fadeStrength;
            
            colorGrading.postExposure.value += (value);

            adjustment.m_Brightness.value = Mathf.InverseLerp(0f, 17f, colorGrading.postExposure.value);

            if (colorGrading.postExposure.value > 17.0f)
            {
                GameManager.Instance.LoadSceneWithIndex(1);
                state = State.None;
            }
        }
    }
    private void OnDestroy()
    {
        if(SubtitleManager.instance != null)
            SubtitleManager.instance.OnSubtitleFinished.RemoveAllListeners();
        adjustment.m_Brightness.value = 0f;
    }
}
