using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class MutilFunctionObject : MonoBehaviour, IPickable
{
    public enum CurrentType
    {
        Sommon = -1, Prespective = 1
    }
    public CurrentType currentType = CurrentType.Sommon;

    public void StopAllInteractive()
    {
        currentPickable.OnPickFinish();
        Destroy(currentPickable.GetBehaviour());
        Destroy(this);
    }

    void Start()
    {
        this.ObserveEveryValueChanged(m => m.currentType).Subscribe(OnCurrentTypeChange);
    }

    IPickable currentPickable;
    void OnCurrentTypeChange(CurrentType _currentType)
    {
        if (currentPickable != null)
        {
            Destroy(currentPickable.GetBehaviour());
        }

        if (_currentType == CurrentType.Sommon)
        {
            currentPickable = gameObject.AddComponent<SommonObject>();
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else
        {
            currentPickable = gameObject.AddComponent<PrespectiveObject>();
            gameObject.layer = LayerMask.NameToLayer("CanGrab");
        }
        if (isHover) currentPickable.OnPointEnter();
    }

    bool isHover = false;
    bool isPick = false;
    
    public void ChangeFunction()
    {
        if (isHover)
        {
            if (isPick)
            {
                return;
            }
            currentType = (CurrentType)((int)currentType * -1);
        }
    }
    public void OnPickStart(Transform pickPointer)
    {
        if (currentPickable == null) return;
        currentPickable.OnPickStart(pickPointer);
        isPick = true;
    }

    public void OnPickFinish()
    {
        if (currentPickable == null) return;
        currentPickable.OnPickFinish();
        isPick = false;
    }

    public void OnPointEnter()
    {
        if (currentPickable == null) return;
        currentPickable.OnPointEnter();
        isHover = true;
    }

    public void OnPointOut()
    {
        if (currentPickable == null) return;
        currentPickable.OnPointOut();
        isHover = false;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public MonoBehaviour GetBehaviour()
    {
        return this;
    }
}
