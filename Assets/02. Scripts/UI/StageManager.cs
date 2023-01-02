using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[System.Serializable]
public class UpgradeDatas
{
    public List<UpgradeData> datas;
}


public class StageManager : MonoSingleton<StageManager>
{
    [SerializeField] UpgradePanelManager upgradePanel;
    //[SerializeField] List<UpgradeData> upgrades;
    [SerializeField] List<UpgradeDatas> upgrades;
    [SerializeField] List<UpgradeData> acquiredUpgrades;
    [SerializeField] PlayerMove player;
    [SerializeField] PlayerScriptableObject playerScriptableObject;
    [SerializeField] HealthBar playerHealth;
    [SerializeField] EnemySpawnPoolController enemySpawnPoolController;

    [Header("Load Main Menu")]
    [SerializeField] private GameObject FadeImage;
    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime = 1f;

    /*
    [Header("Dialogues")]
    [SerializeField] public DialogueTrigger dialogueTrigger;*/

    public bool isGameOver { get; private set; }

    private int typeOfPlugIn = 6;
    private List<UpgradeData> selectedUpgrades;

    public event EventHandler UpgradeHealth;
    public event EventHandler UpgradeSetShieldActive;
    public event EventHandler UpgradeMoveSpeed;
    public event EventHandler UpgradeEntireAttack;
    public event EventHandler UpgradeSetPetActive;
    public event EventHandler UpgradePetAbility;
    public event EventHandler UpgradeSetForceFieldActive;
    public event EventHandler UpgradePetCoolTime;
    public event EventHandler UpgradeFusionComponent;
    public event EventHandler GameOverEvent;

    private void Start()
    {
        //enemySpawnPoolController.OnStageEnd += UpdatePlugIn;
        player.OnTriggerPortal += UpdatePlugIn;
        enemySpawnPoolController.UpgradePlugIn += UpdatePlugIn;
        FindObjectOfType<Health>().OnDeath += EndGame;
        PlayerVariable pv = new PlayerVariable();


        playerScriptableObject.meleeAttackDamage = pv.meleeAttackDamage;
        playerScriptableObject.skillActiveMeleeAttackDamage = pv.skillActiveMeleeAttackDamage;
        playerScriptableObject.burnTicks = pv.burnTicks;
        playerScriptableObject.maxBurnTicks = pv.maxBurnTicks;
        playerScriptableObject.burnDamage = pv.burnDamage;
        playerScriptableObject.corrosionTicks = pv.corrosionTicks;
        playerScriptableObject.corrosionDamage = pv.corrosionDamage;
        playerScriptableObject.maxCorrosionTicks = pv.maxCorrosionTicks;
        playerScriptableObject.enabledSecondUpgrade = pv.enabledSecondUpgrade;
        playerScriptableObject.enabledThirdUpgrade = pv.enabledThirdUpgrade;
        playerScriptableObject.enabledFourthUpgrade = pv.enabledFourthUpgrade;
        playerScriptableObject.isPetUpgraded = pv.isPetUpgraded;
        

    }
    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdatePlugIn_Temp();
        }*/
    }

    private void EndGame()
    {
        isGameOver = true;
        //Debug.Log("End game");
        GameOverEvent?.Invoke(this, EventArgs.Empty);
        //FindObjectOfType<UIManager>().OpenGameOverUI();
    }

    public void LoadMainScene()
    {
        StartCoroutine(LoadScene(0));
    }

    private IEnumerator LoadScene(int sceneIndex)
    {
        FadeImage.SetActive(true);
        transition.SetBool("Start", true);

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneIndex);
    }


    // For Plug In Test
    private void UpdatePlugIn_Temp()
    {
        if (selectedUpgrades == null)
        {
            selectedUpgrades = new List<UpgradeData>();
        }
        selectedUpgrades.Clear();
        selectedUpgrades.AddRange(GetUpgrades(4));
        upgradePanel.OpenPanel(selectedUpgrades);
    }

    //�ߵ� ���� �ʿ�
    private void UpdatePlugIn(object sender, EventArgs e)
    {
        if (selectedUpgrades == null)
        {
            selectedUpgrades = new List<UpgradeData>();
        }
        selectedUpgrades.Clear();
        selectedUpgrades.AddRange(GetUpgrades(4));
        upgradePanel.OpenPanel(selectedUpgrades);
        //enemySpawnPoolController.UpgradePlugIn -= UpdatePlugIn;
    }

    public List<UpgradeData> GetUpgrades(int count)
    {
        List<UpgradeData> upgradeList = new List<UpgradeData>();
        

        int totalNum = 0;

        for(int i = 0; i < upgrades.Count; i++)
        {
            totalNum += upgrades[i].datas.Count;
        }

        if (count > totalNum)
        {
            count = totalNum;
        }

        List<int> numList = new List<int>();
        for(int i = 0; i < typeOfPlugIn; i++)
        {
            numList.Add(i);
        }

        while (count != 0)
        {
            if (numList.Count == 0) break;

            int j = numList[Random.Range(0, numList.Count)];
            if (upgrades[j].datas.Count == 0)
            {
                numList.Remove(j);
                continue;
            }

            UpgradeData upgradeData = upgrades[j].datas[Random.Range(0, upgrades[j].datas.Count)];
            upgradeList.Add(upgradeData);
            numList.Remove(j);
            count--;
        }

        return upgradeList;
    }

    public void AddUpgradesIntoCurrentUpgradeList(List<UpgradeData> upgradesToAdd, int index)
    {
        //this.upgrades.AddRange(upgradesToAdd);
        if (upgradesToAdd.Count == 0) return;
        if (upgradesToAdd.Count == 1 && (this.upgrades[index].datas.Contains(upgradesToAdd[0]) || acquiredUpgrades.Contains(upgradesToAdd[0]))) return;

        this.upgrades[index].datas.AddRange(upgradesToAdd);
    }

    public void Upgrade(int selectedUpgradeId)
    {
        UpgradeData upgradeData = selectedUpgrades[selectedUpgradeId];

        if (acquiredUpgrades == null)
        {
            acquiredUpgrades = new List<UpgradeData>();
        }

        //Debug.Log(upgradeData.plugInType);

        switch (upgradeData.plugInType)
        {
            //���⿡ player �Ҵ� �޾Ƽ� �Լ��� ������ ��. 
            //index 0
            //�ܼ� ������ ���� 15%
            case PlugInType.GauntletAttack_1:
                player.UpgradeAttackDamage(playerScriptableObject.upgradeDamage_Gauntlet1);
                playerScriptableObject.latestGauntletDamageRate = playerScriptableObject.upgradeDamage_Gauntlet1;

                // �� 2��° ���׷��̵� �̹� �Ǿ��ִ� ���, �� �ɷ�ġ�� �ø���. 
                if (playerScriptableObject.isPetUpgraded) UpgradePetAbility?.Invoke(this, EventArgs.Empty);
                break;
            //������ 30% ����, ���ݼӵ� 10% ����
            case PlugInType.GauntletAttack_2:
                player.UpgradeAttackSpeed(playerScriptableObject.upgradeSpeed_Gauntlet2);
                player.UpgradeAttackDamage(playerScriptableObject.upgradeDamage_Gauntlet2);
                playerScriptableObject.latestGauntletDamageRate = playerScriptableObject.upgradeDamage_Gauntlet2;

                // �� 2��° ���׷��̵� �̹� �Ǿ��ִ� ���, �� �ɷ�ġ�� �ø���. 
                if (playerScriptableObject.isPetUpgraded) UpgradePetAbility?.Invoke(this, EventArgs.Empty);

                break;
            //���ݼӵ� 15% ����, ������ 25% ����
            case PlugInType.GauntletAttack_3:
                player.UpgradeAttackSpeed(playerScriptableObject.upgradeSpeed_Gauntlet3);
                player.UpgradeAttackDamage(playerScriptableObject.upgradeDamage_Gauntlet3);
                playerScriptableObject.latestGauntletDamageRate = playerScriptableObject.upgradeDamage_Gauntlet3;

                // �� 2��° ���׷��̵� �̹� �Ǿ��ִ� ���, �� �ɷ�ġ�� �ø���. 
                if (playerScriptableObject.isPetUpgraded) UpgradePetAbility?.Invoke(this, EventArgs.Empty);
                break;
            //���ݷ� 50% ����, �������� OR ���ݼӵ� 10% ����[�����]
            case PlugInType.GauntletAttack_4:
                //player.UpgradeAttackSpeed(1.1f);
                player.UpgradeAttackDamage(playerScriptableObject.upgradeDamage_Gauntlet4);
                playerScriptableObject.latestGauntletDamageRate = playerScriptableObject.upgradeDamage_Gauntlet4;

                // �� 2��° ���׷��̵� �̹� �Ǿ��ִ� ���, �� �ɷ�ġ�� �ø���. 
                if (playerScriptableObject.isPetUpgraded) UpgradePetAbility?.Invoke(this, EventArgs.Empty);
                // ��Ʋ�� 4��° ���׷��̵� bool ������ ������ �ٲ۴�.
                playerScriptableObject.gauntletFourthUpgrade = true;
                // ���� �÷��� �� ���׷��̵� Event Trigger �߻���Ų��
                UpgradeFusionComponent?.Invoke(this, EventArgs.Empty);
                break;

            // �Ӽ� ���� �ø��� ����

            /*
             * �� �Ӽ������� �ϳ��� ���� �������� �� �� ����.
             * 
             * ȭ��
             * ��Ʈ ������ ��ø �Ұ�, ��Ʈ�� �°� �ִ� �ο����Դ� �ٽñ� ���ʵǴ� ����
             * 1.���ݿ� ȭ�� ������ �߰�[��Ʈ ȭ�� ������ �߰�, �������� ª�� ��Ʈ��] 
             * 2.ȭ���� ��Ʈ ������ ����
             * 3.���� ���� ��ġ�� ȭ���� ���� ����?
             * 4.[ȭ�� ������ �� ����� ����, ����]
             * 
             */

            //index 1
            case PlugInType.CorrosionAttack_1: 
                upgrades[upgradeData.index + 1].datas.Clear();
                upgrades[upgradeData.index + 2].datas.Clear();
                player.EnableElementAttack(PlayerMove.Element.Corrosion);
                break;
            case PlugInType.CorrosionAttack_2: 
                //upgrades[upgradeData.index].datas.Clear();
                playerScriptableObject.corrosionTicks += 10;
                break;
            case PlugInType.CorrosionAttack_3: 
                playerScriptableObject.enabledThirdUpgrade = true;
                break;
            case PlugInType.CorrosionAttack_4: 
                playerScriptableObject.enabledFourthUpgrade = true;
                // ���� �÷��� �� ���׷��̵� Event Trigger �߻���Ų��
                UpgradeFusionComponent?.Invoke(this, EventArgs.Empty);
                break;
            //index 2
            case PlugInType.FireAttack_1: 
                upgrades[upgradeData.index - 1].datas.Clear();
                upgrades[upgradeData.index + 1].datas.Clear();
                player.EnableElementAttack(PlayerMove.Element.Fire);
                Debug.Log("Fire1");
                break;
            case PlugInType.FireAttack_2: 
                playerScriptableObject.burnDamage *= 2f;
                Debug.Log("Fire2");
                break;
            case PlugInType.FireAttack_3: 
                playerScriptableObject.enabledThirdUpgrade = true;
                Debug.Log("Fire3");
                break;
            case PlugInType.FireAttack_4: 
                playerScriptableObject.enabledFourthUpgrade = true;
                player.SetFireFourthUpgrade();
                // ���� �÷��� �� ���׷��̵� Event Trigger �߻���Ų��
                UpgradeFusionComponent?.Invoke(this, EventArgs.Empty);
                Debug.Log("Fire4");
                break;



            //������� �Ϸ�
            //index 3
            case PlugInType.IceAttack_1:
                upgrades[upgradeData.index - 2].datas.Clear();
                upgrades[upgradeData.index - 1].datas.Clear();
                player.EnableElementAttack(PlayerMove.Element.Ice);
                break;
            case PlugInType.IceAttack_2:
                playerScriptableObject.slowDownSpeed *= 1.4f;
                playerScriptableObject.enabledThirdUpgrade = true;
                break;
            case PlugInType.IceAttack_3:
                playerScriptableObject.enabledThirdUpgrade = true;
                break;
            case PlugInType.IceAttack_4:
                playerScriptableObject.enabledFourthUpgrade = true;
                // ���� �÷��� �� ���׷��̵� Event Trigger �߻���Ų��
                UpgradeFusionComponent?.Invoke(this, EventArgs.Empty);
                break;
            //index 4
            //ü�� ���� �÷����� ���ξ��� �̵��ӵ� ������ ������ ���?
            /*Utility Plugin ���� �̸� ����
             * 1. ü�� ��ȭ
             * 2. �ǵ� ����
             * 3. �̵��ӵ� ����
             * 4. ü�� ��ȭ �� ���ؽ� ȭ�� ��ü�� ������ ���� ������
             */
            case PlugInType.Utility_1:
                //Health or Barrier ++
                UpgradeHealth?.Invoke(this, EventArgs.Empty);
                break;
            case PlugInType.Utility_2:
                //ShieldEffect.SetActive(true);
                UpgradeSetShieldActive?.Invoke(this, EventArgs.Empty);
                break;
            case PlugInType.Utility_3:
                UpgradeMoveSpeed?.Invoke(this, EventArgs.Empty);
                break;
            case PlugInType.Utility_4:
                UpgradeEntireAttack?.Invoke(this, EventArgs.Empty);
                
                // Util 4��° ���׷��̵� bool ������ ������ �ٲ۴�.
                playerScriptableObject.utilFourthUpgrade = true;
                // ���� �÷��� �� ���׷��̵� Event Trigger �߻���Ų��
                UpgradeFusionComponent?.Invoke(this, EventArgs.Empty);
                break;
            //index 5
            /*��ȯ���� ��� 
             * 
             * ���Ÿ� ���� ������� ����
             * �÷��̾� �߽����� ���ɿ��� �׸��� ȸ��
             * ���� ����� ������ ��ų ���� 
             * ��ų ���� ��ġ ��ġ ���� W
             * 
             * 1. ��ȯ�� ����
             * 2. ��ȯ�� Melee ���� ��ȭ Ȥ�� Player Buff
             * 3. ��ȯ�� Debuff ��ȭ 
             * 4. ��ȯ�� Damage&�Ӽ����� �÷��̾� ��ȭ ���¿� ���缭 ��ȭ
             */
            case PlugInType.SummonAttack_1:
                UpgradeSetPetActive?.Invoke(this, EventArgs.Empty);
                break;
            case PlugInType.SummonAttack_2:
                playerScriptableObject.isPetUpgraded = true;
                UpgradePetAbility?.Invoke(this, EventArgs.Empty);
                break;
            case PlugInType.SummonAttack_3:
                UpgradeSetForceFieldActive?.Invoke(this, EventArgs.Empty);
                break;
            case PlugInType.SummonAttack_4:
                UpgradePetCoolTime?.Invoke(this, EventArgs.Empty);

                // Pet 4��° ���׷��̵� bool ������ ������ �ٲ۴�.
                playerScriptableObject.petFourthUpgrade = true;
                // ���� �÷��� �� ���׷��̵� Event Trigger �߻���Ų��
                UpgradeFusionComponent?.Invoke(this, EventArgs.Empty);
                break;
        }

        acquiredUpgrades.Add(upgradeData);
        //upgrades[upgradeData.index].datas.Clear();
        upgrades[upgradeData.index].datas.Remove(upgradeData);
        AddUpgradesIntoCurrentUpgradeList(upgradeData.upgrades, upgradeData.index);
    }
}
