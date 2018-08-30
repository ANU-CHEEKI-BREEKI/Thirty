using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Exit : MonoBehaviour
{
    int playersUnitCountITrigger = 0;
    [SerializeField] [Range(0, 5)] float deltaTime;

    Transform thisTransform;

    Squad playerSquad;

    bool goToNextLevel = false;

    Collider2D collider;

    void Start()
    {
        Transform parent = transform.parent;
        if (parent == null)
        {
            Destroy(gameObject);
            return;
        }

        GroundBlock block = parent.GetComponent<GroundBlock>();
        if (block != null && block.posInMinigrid == GameManager.Instance.entranceBlockPosition)
        {
            Destroy(gameObject);
            return;
        }

        thisTransform = transform;
        playerSquad = GameObject.FindWithTag("Player").GetComponent<Squad>();
        collider = GetComponent<Collider2D>();
    }

    IEnumerator CheckCenterSquadToExit()
    {
        while (true)
        {
            if (collider.OverlapPoint(playerSquad.CenterSquad))
            {
                goToNextLevel = true;

                Action act = null;
                act = () =>
                {
                    TempValuesEndLevelScreen.Instance.Show();
                    FadeScreen.Instance.OnFadeOn -= act;
                };

                FadeScreen.Instance.OnFadeOn += act;
                FadeScreen.Instance.FadeOn(1f);

                break;
            }
            yield return new WaitForSeconds(deltaTime);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Ground.Instance.WorkIsDone)
        {
            Unit unit = collision.GetComponent<Unit>();
            if (unit != null && unit.gameObject.layer == LayerMask.NameToLayer("ALLY"))
            {
                playersUnitCountITrigger++;

                if (playersUnitCountITrigger == 1 && !goToNextLevel)
                    StartCoroutine(CheckCenterSquadToExit());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Ground.Instance.WorkIsDone)
        {
            Unit unit = collision.GetComponent<Unit>();
            if (unit != null && unit.gameObject.layer == LayerMask.NameToLayer("ALLY"))
            {
                playersUnitCountITrigger--;

                if (playersUnitCountITrigger == 0 && !goToNextLevel)
                    StopCoroutine(CheckCenterSquadToExit());
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (playersUnitCountITrigger > 0)
        {
            if (goToNextLevel)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(thisTransform.position, 2);
        }
    }
}
