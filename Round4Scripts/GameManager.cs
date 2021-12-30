using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // the current day
    public int day;
    private Animator ani;
    private Dictionary<int, UnityAction> eventsDic = new Dictionary<int, UnityAction>();

    public List<GameObject> Screens;

    public List<Material> SkyboxList;

    public MicInput Mic;

    public GameObject EmergencyLight;

    public ScreenManager UIM;
    public GameObject Keyboard;
    public GameObject MessagePanel;

    public TMPro.TMP_InputField FinalInput;

    public SoundManager RobotSoundManager;

    public SoundManager BGM;
    public SoundManager HitSound;
    public AudioSource Ambience;

    public RouteRender routeRender;

    private void Awake()
    {
        eventsDic.Add(0, Day0);//
        eventsDic.Add(1, Day1);//ParentCall
        eventsDic.Add(3, Day3);//FirstNews
        eventsDic.Add(6, Day6);//Interview
        eventsDic.Add(7, Day7);//SecondNews
        eventsDic.Add(50, Day50);//NoWayOut
        eventsDic.Add(53, Day53);//HitNews3
        eventsDic.Add(99, Day99);//Final

        ani = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>();
    }
    private void Start()
    {
        day = Calendar.Day;
        Debug.Log(day);
        Calendar.Day += 1;
        while (!eventsDic.ContainsKey(Calendar.Day))
        {
            Calendar.Day += 1;
            if (Calendar.Day > 100)
            {
                break;
            }
        }
        
        eventsDic[day].Invoke();
    }

    private void Update()
    {

    }

    public void NextDay()
    {
        ani.Play("FadeOut");
        StartCoroutine(SceneLoad());
    }

    private IEnumerator SceneLoad()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(0);
    }

    private void DoSomething()
    {

    }

    private void Day0()
    {
        RenderSettings.skybox = SkyboxList[0];
        BGM.Play(0);
        UIM.UpdateLog();
        RobotSoundManager.Play(0);
        UIM.SetFuel(100);
        UIM.SwitchPanel(4);
        UIM.ButtonGroup.SetActive(false);
        Keyboard.SetActive(true);
        MessagePanel.SetActive(false);
    }

    private void Day1()
    {
        RenderSettings.skybox = SkyboxList[0];
        BGM.Play(0);
        DatabaseManager.Instance.AddEarthLog(0);
        DatabaseManager.Instance.AddEarthLog(1);
        UIM.UpdateLog();
        UIM.SetFuel(99);
        Screens[0].SetActive(true);
        routeRender.currentLevelIndex = 0; // set the route level to level0
    }

    private void Day3()
    {
        RenderSettings.skybox = SkyboxList[1];
        BGM.Play(0);
        DatabaseManager.Instance.AddEarthLog(2);
        UIM.UpdateLog();
        UIM.SetFuel(97);
        Screens[1].SetActive(true);
        routeRender.currentLevelIndex = 1; // set the route level to level1
    }

    private void Day6()
    {
        RenderSettings.skybox = SkyboxList[2];
        BGM.Play(0);
        DatabaseManager.Instance.AddEarthLog(3);
        DatabaseManager.Instance.AddPlayerLog(0);
        UIM.UpdateLog();
        UIM.SetFuel(95);
        Screens[2].SetActive(true);
        routeRender.currentLevelIndex = 2; // set the route level to level2
    }

    private void Day7()
    {
        RenderSettings.skybox = SkyboxList[3];
        BGM.Play(0);
        DatabaseManager.Instance.AddEarthLog(4);
        DatabaseManager.Instance.AddPlayerLog(1);
        UIM.UpdateLog();
        UIM.SetFuel(93);
        Screens[3].SetActive(true);
        routeRender.currentLevelIndex = 3; // set the route level to level3
    }

    private void Day50()
    {
        RobotSoundManager.Play(1);
        HitSound.Play(1);
        UIM.ReduceStamina();
        RenderSettings.skybox = SkyboxList[4];
        BGM.Play(1);
        DatabaseManager.Instance.AddEarthLog(5);
        DatabaseManager.Instance.AddEarthLog(6);
        DatabaseManager.Instance.AddPlayerLog(2);
        UIM.UpdateLog();
        UIM.SetFuel(47);
        EmergencyLight.SetActive(true);
        //BGM.Stop();
        Ambience.Stop();
        MessagePanel.SetActive(false);
        routeRender.currentLevelIndex = 4; // set the route level to level4
        UIM.ReduceStamina();
    }
    private void Day53()
    {
        RenderSettings.skybox = SkyboxList[5];
        BGM.Play(1);
        DatabaseManager.Instance.AddEarthLog(7);
        DatabaseManager.Instance.AddEarthLog(8);
        DatabaseManager.Instance.AddPlayerLog(3);
        UIM.UpdateLog();
        UIM.SetFuel(20);
        EmergencyLight.SetActive(true);
        Ambience.Stop();
        Screens[4].SetActive(true);
        UIM.ReduceStamina();
    }
    private void Day99()
    {
        RenderSettings.skybox = SkyboxList[6];
        DatabaseManager.Instance.InputTarget = FinalInput;
        BGM.Play(2);
        DatabaseManager.Instance.AddEarthLog(7);
        DatabaseManager.Instance.AddEarthLog(8);
        DatabaseManager.Instance.AddPlayerLog(3);
        UIM.SetFuel(1);
        UIM.SwitchPanel(5);
        UIM.ButtonGroup.SetActive(false);
        Keyboard.SetActive(true);
        EmergencyLight.SetActive(true);
        Ambience.Stop();
        MessagePanel.SetActive(false);
    }

    public void End()
    {
        DatabaseManager.Instance.Submit(DatabaseManager.Instance.Logs.Count, DatabaseManager.Instance.InputTarget.text);
        DatabaseManager.Instance.Logs.Add(new Signal(DatabaseManager.Instance.PlayerName, DatabaseManager.Instance.InputTarget.text));
        SceneManager.LoadScene(1);
    }
}
