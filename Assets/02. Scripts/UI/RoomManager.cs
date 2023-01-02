using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RoomManager>();
                if (instance == null)
                {
                    var instanceContainer = new GameObject("RoomManager");
                    instance = instanceContainer.AddComponent<RoomManager>();
                }
            }
            return instance;
        }
    }

    private static RoomManager instance;

    [System.Serializable]
    public class StartPositionArray
    {
        public List<Transform> StartPosition = new List<Transform>();
    }

    [SerializeField] private GameObject SceneLoader;
    [SerializeField] private Animator Transition;
    [SerializeField] EnemySpawnPoolController enemySpawnPoolController;
    [SerializeField] private float transitionTime = 4f;
    [SerializeField] private int bossStagenum = 9;

    [Header("Start Position Dictionary")]
    [SerializeField] private List<int> keys;
    [SerializeField] private List<Transform> transforms;

    [SerializeField] private List<GameObject> potals;
    [SerializeField] DialogueTrigger dt;

    private Dictionary<int, Transform> startPositionDic;
    private GameObject Player;
    private const int numOfStageType = 4;
    private int currentStage = 0;
    private int countStage = 0;
    private int stageNum;

    private void Awake()
    {
        //Dictionary 초기화
        var count = Mathf.Min(keys.Count, transforms.Count);
        startPositionDic = new Dictionary<int, Transform>(count);

        for (int i = 0; i < count; i++)
        {
            startPositionDic.Add(keys[i], transforms[i]);
        }
    }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        enemySpawnPoolController.OnSetActivePotal += SetActivePotal;
        if(countStage ==0)
        {
            dt.TriggerDialogue();
        }

    }

    public void NextStage()
    {
        //현재 스테이지의 포탈 off
        potals[currentStage].SetActive(false);

        StartCoroutine(ChangeStage());
    }


    private void SetActivePotal(object sender, EnemySpawnPoolController.OnSetActivePotalEventArgs e)
    {
        //Debug.Log(stageNum);
        potals[currentStage].SetActive(true);
    }

    private IEnumerator ChangeStage()
    {
        if (countStage == bossStagenum)
        {
            Debug.Log("Boss Room");
            yield return null;
        }

        SceneLoader.SetActive(true);
        yield return new WaitForSeconds(transitionTime);

        int randomKey;

        while (true)
        {
            randomKey = Random.Range(0, numOfStageType);
            if (currentStage == randomKey) continue;
            break;
        }

        //Enemy spawn pool controller의 stage num 초기화
        enemySpawnPoolController.stageNum = randomKey;
        enemySpawnPoolController.StartMethod();

        //이전 스테이지의 key 값을 현재의 값으로 초기화
        currentStage = randomKey;
        countStage++;
        Player.transform.position = startPositionDic[randomKey].position;

        yield return new WaitForSeconds(transitionTime);

        Transition.SetBool("End", true);


        SceneLoader.SetActive(false);
        
        yield return null;
    }
}
