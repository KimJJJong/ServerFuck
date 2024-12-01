/*using System.Collections.Concurrent;
using UnityEngine;

/// <summary>
/// �Ű����� ���� �Լ��� ���ν����忡�� ����
/// </summary>
public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher _instance;

    public static MainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject container = new GameObject("MainThreadDispatcher");
                _instance = container.AddComponent<MainThreadDispatcher>();
            }

            return _instance;
        }
    }

    private ConcurrentQueue<System.Action> _executionQueue;

    public void Init()
    {
        _executionQueue = new ConcurrentQueue<System.Action>();
    }

    private void Update()
    {
        while (_executionQueue.TryDequeue(out System.Action execution))
        {
            execution?.Invoke();
        }
    }

    public void Add(System.Action execution)
    {
        _executionQueue.Enqueue(execution);
    }
}*/