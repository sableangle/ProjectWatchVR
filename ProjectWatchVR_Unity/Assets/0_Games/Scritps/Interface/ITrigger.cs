using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrigger
{
    void OnWrapperTriggerEnter(GameObject whoGotHit,Collider other);
    void OnWrapperTriggerStay(GameObject whoGotHit,Collider other);
    void OnWrapperTriggerExit(GameObject whoGotHit,Collider other);
}
