using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrigger
{
    void OnTriggerEnter(GameObject whoGotHit,Collider other);
    void OnTriggerStay(GameObject whoGotHit,Collider other);
    void OnTriggerExit(GameObject whoGotHit,Collider other);
}
