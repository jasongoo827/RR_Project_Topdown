using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public PoolObject poolObject;

    // �� ������Ʈ�� ��� Ǯ���� ���Դ��� ���۷����� �� ������Ʈ�� �������� ������ assign ���ش�.
    public ObjectPool pool;

    private void Awake()
    {
        poolObject = this.GetComponent<PoolObject>();
        pool = GetComponentInParent<ObjectPool>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            ReturnToPool();
        }
    }

    // Ǯ�� ����� ������ ������Ʈ Ǯ�� �ٽ� ���� �ش�.
    void ReturnToPool()
    {
        pool.PushPoolObject(this.gameObject);
    }
}
