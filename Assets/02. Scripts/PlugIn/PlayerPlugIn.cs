using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPlugIn
{
    public event EventHandler<OnPlugInUnlockedEventArgs> OnPlugInUnlocked;
    public class OnPlugInUnlockedEventArgs : EventArgs
    {
        public PlugInType plugInType;
    }

    public enum PlugInType
    {
        None,
        GauntletAttack_1, // �ܼ� ������ ���� 15%
        GauntletAttack_2, // ������ 30% ����, ���ݼӵ� 10% ����
        GauntletAttack_3, // ���ݼӵ� 15% ����, ������ 25% ����
        GauntletAttack_4, // ���ݷ� 50% ����, �������� OR ���ݼӵ� 10% ����[�����]
        Utility_1, // ü�� ��ȭ
        Utility_2, // �ǵ� ����
        Utility_3, // �̵��ӵ� ����
        Utility_4, // ü�� ��ȭ �� ���ؽ� ȭ�� ��ü�� ������ ���� ������

        /*��ȯ���� ��� 
         * 1. ��ȯ�� ����
         * 2. ��ȯ�� Melee ���� ��ȭ
         * 3. ��ȯ�� Debuff ��ȭ
         * 4. ��ȯ�� Damage&�Ӽ����� �÷��̾� ��ȭ ���¿� ���缭 ��ȭ
         */
        //SummonAttack 4���� ����
        SummonAttack_1,
        SummonAttack_2,
        SummonAttack_3,
        SummonAttack_4,

        FireAttack_1,
        FireAttack_2,
        FireAttack_3,
        FireAttack_4,
        IceAttack_1,
        IceAttack_2,
        IceAttack_3,
        IceAttack_4,

        //���� ������ �ν����� ��ü�� ����� �ν�[Corrosion]���� ��Ī ����
        CorrosionAttack_1,
        CorrosionAttack_2,
        CorrosionAttack_3,
        CorrosionAttack_4,
        ElectricAttack_1,
        ElectricAttack_2,
        ElectricAttack_3,
        ElectricAttack_4,

    }

    private List<PlugInType> unlockedPlugInTypeList;
    //private int plugInNum = 29;

    public PlayerPlugIn()
    {
        unlockedPlugInTypeList = new List<PlugInType>();

    }
    
    private void UnlockPlugIn(PlugInType plugInType)
    {
        if (!IsPlugInUnlocked(plugInType))
        {
            unlockedPlugInTypeList.Add(plugInType);
            OnPlugInUnlocked?.Invoke(this, new OnPlugInUnlockedEventArgs { plugInType = plugInType });
            
        }
    }

    public bool IsPlugInUnlocked(PlugInType plugInType)
    {
        return unlockedPlugInTypeList.Contains(plugInType);
    }


    public PlugInType GetPlugInRequirement(PlugInType plugInType)
    {
        switch (plugInType)
        {
            case PlugInType.GauntletAttack_2: return PlugInType.GauntletAttack_1;
            case PlugInType.GauntletAttack_3: return PlugInType.GauntletAttack_1;
            case PlugInType.GauntletAttack_4:
                return IsPlugInUnlocked(PlugInType.GauntletAttack_2) ? PlugInType.GauntletAttack_2 : PlugInType.GauntletAttack_3;

            case PlugInType.Utility_2: return PlugInType.Utility_1;
            case PlugInType.Utility_3: return PlugInType.Utility_2;
            case PlugInType.Utility_4: return PlugInType.Utility_3;

        }
        return PlugInType.None;
    }

    public bool TryUnlockPlugIn(PlugInType plugInType)
    {
        PlugInType plugInRequirement = GetPlugInRequirement(plugInType);

        if (plugInRequirement != PlugInType.None)
        {
            if (IsPlugInUnlocked(plugInRequirement))
            {
                UnlockPlugIn(plugInType);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            UnlockPlugIn(plugInType);
            return true;
        }
    }
}
