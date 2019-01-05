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

    new Collider2D collider;

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
                    var playerEq = GameManager.Instance.SavablePlayerData.PlayerProgress.Equipment;

                    var inventory = Squad.playerSquadInstance.Inventory.StoredInventory;
                    inventory.Add(Squad.playerSquadInstance.Inventory.Helmet);
                    inventory.Add(Squad.playerSquadInstance.Inventory.Body);
                    inventory.Add(Squad.playerSquadInstance.Inventory.Weapon);
                    inventory.Add(Squad.playerSquadInstance.Inventory.Shield);
                    foreach (var i in inventory)
                    {
                        //"открываем" увиденную экипировку - но тлько в временное хранилище
                        //делаем это именно тка, потому что так было изначально. а ща я поменял концепцию открытия предметов.
                        //но менять всю структуру я манал.
                        if (!i.EquipmentStats.Empty && !playerEq.IsThisEquipmantAllowed(i.EquipmentStats) && !playerEq.IsThisEquipmantInTempValues(i.EquipmentStats))
                        {
                            playerEq.AddTempValue(i.EquipmentStats);
                            AllowedEquipmantPanel.MainInstance.AddEq(i);
                        }
                    }


                    TempValuesEndLevelScreen.Instance.Show();
                    var gm = GameManager.Instance;
                    gm.SavablePlayerData.PlayerProgress.Squad.SetSquadValues(Squad.playerSquadInstance);
                    gm.SavablePlayerData.PlayerProgress.Level.SetValues(gm.CurrentLevel);

                    GPSWrapper.Achivement.IncrementProgress(GPSConstants.achievement_beginer, 1, null);
                    GPSWrapper.Achivement.IncrementProgress(GPSConstants.achievement_experienced, 1, null);
                    GPSWrapper.Achivement.IncrementProgress(GPSConstants.achievement_god_of_war, 1, null);

                    if (Squad.playerSquadInstance.UnitCount == 0)
                        GPSWrapper.Achivement.Unlock(GPSConstants.achievement_always_go_to_the_end, null);

                    var lvl = GameManager.Instance.CurrentLevel;
                    if (lvl.Level == lvl.MaxLevel)
                    {
                        switch (lvl.GroundType)
                        {
                            case Ground.GroundType.GRASSLAND:
                                GPSWrapper.Achivement.Unlock(GPSConstants.achievement_grassland, null);
                                break;
                            case Ground.GroundType.SWAMP:
                                GPSWrapper.Achivement.Unlock(GPSConstants.achievement_swamp, null);
                                break;
                        }
                    }

                    if(AchivementStandToTheLast.killsCount >= 90)
                        GPSWrapper.Achivement.Unlock(GPSConstants.achievement_stand_to_the_last, null);

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
