using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    //#region Knight Death

    //static List<Enemy> KnightDeathInvokers = new List<Enemy>();
    //static List<UnityAction> KnightDeathListeners = new List<UnityAction>();

    //public static void AddKnightDeathInvoker (Enemy invoker)
    //{
    //    KnightDeathInvokers.Add(invoker);
    //    foreach (UnityAction listener in KnightDeathListeners)
    //    {
    //        invoker.AddDeathEventListener(listener);
    //    }
    //}
    //public static void AddKnightDeathListener (UnityAction listener)
    //{
    //    KnightDeathListeners.Add(listener);
    //    foreach (Enemy invoker in KnightDeathInvokers)
    //    {
    //        invoker.AddDeathEventListener(listener);
    //    }
    //}

    //public static void RemoveKnightDeathInvoker(Enemy invoker)
    //{
    //    KnightDeathInvokers.Remove(invoker);
    //}
    //#endregion

    #region Enemy Death

    static Dictionary<EnemySpawner, List<Enemy>> EnemyDeathInvokers
    = new Dictionary<EnemySpawner, List<Enemy>>();
    static Dictionary<EnemySpawner, List<UnityAction>> EnemyDeathListeners
        = new Dictionary<EnemySpawner, List<UnityAction>>();

    public static void AddEnemyDeathInvoker(EnemySpawner spawner, Enemy invoker)
    {
        // initialize dictionary if not initialized
        if (!EnemyDeathInvokers.ContainsKey(spawner))
        {
            EnemyDeathInvokers.Add(spawner, new List<Enemy>());
            EnemyDeathListeners.Add(spawner, new List<UnityAction>());
        }

        EnemyDeathInvokers[spawner].Add(invoker);
        foreach (UnityAction listener in EnemyDeathListeners[spawner])
        {
            invoker.AddEnemyDeathListener(listener);
        }
        
    }

    public static void AddEnemyDeathListener(EnemySpawner spawner, UnityAction listener)
    {
        // initialize dictionary if not initialized
        if (!EnemyDeathInvokers.ContainsKey(spawner))
        {
            EnemyDeathInvokers.Add(spawner, new List<Enemy>());
            EnemyDeathListeners.Add(spawner, new List<UnityAction>());
        }

        EnemyDeathListeners[spawner].Add(listener);
        foreach (Enemy invoker in EnemyDeathInvokers[spawner])
        {
            invoker.AddEnemyDeathListener(listener);
        }
    }

    public static void RemoveEnemyDeathInvoker(EnemySpawner spawner, Enemy invoker)
    {
        if (EnemyDeathInvokers.ContainsKey(spawner))
            EnemyDeathInvokers[spawner].Remove(invoker);
    }
    #endregion

    #region Enemy Resurrect

    static Dictionary<EnemySpawner, List<Enemy>> EnemyResurrectInvokers
    = new Dictionary<EnemySpawner, List<Enemy>>();
    static Dictionary<EnemySpawner, List<UnityAction>> EnemyResurrectListeners
        = new Dictionary<EnemySpawner, List<UnityAction>>();

    public static void AddEnemyResurrectInvoker(EnemySpawner spawner, Enemy invoker)
    {
        // initialize dictionary if not initialized
        if (!EnemyResurrectInvokers.ContainsKey(spawner))
        {
            EnemyResurrectInvokers.Add(spawner, new List<Enemy>());
            EnemyResurrectListeners.Add(spawner, new List<UnityAction>());
        }

        EnemyResurrectInvokers[spawner].Add(invoker);
        foreach (UnityAction listener in EnemyResurrectListeners[spawner])
        {
            invoker.AddEnemyResurrectListener(listener);
        }
    }

    public static void AddEnemyResurrectListener(EnemySpawner spawner, UnityAction listener)
    {
        // initialize dictionary if not initialized
        if (!EnemyResurrectInvokers.ContainsKey(spawner))
        {
            EnemyResurrectInvokers.Add(spawner, new List<Enemy>());
            EnemyResurrectListeners.Add(spawner, new List<UnityAction>());
        }

        EnemyResurrectListeners[spawner].Add(listener);
        foreach (Enemy invoker in EnemyResurrectInvokers[spawner])
        {
            invoker.AddEnemyResurrectListener(listener);
        }
    }

    public static void RemoveEnemyResurrectInvoker(EnemySpawner spawner, Enemy invoker)
    {
        if (EnemyResurrectInvokers.ContainsKey(spawner))
            EnemyResurrectInvokers[spawner].Remove(invoker);
    }
    #endregion

    #region Chest Opened

    static List<Chest> ChestOpenedInvokers = new List<Chest>();
    static List<UnityAction> ChestOpenedListeners = new List<UnityAction>();

    public static void AddChestOpenedInvoker(Chest invoker)
    {
        ChestOpenedInvokers.Add(invoker);
        foreach (UnityAction listener in ChestOpenedListeners)
        {
            invoker.AddChestOpenedListener(listener);
        }
    }

    public static void AddChestOpenedListener(UnityAction listener)
    {
        ChestOpenedListeners.Add(listener);
        foreach (Chest invoker in ChestOpenedInvokers)
        {
            invoker.AddChestOpenedListener(listener);
        }
    }
    #endregion

    #region Chest Closed

    static List<Chest> ChestClosedInvokers = new List<Chest>();
    static List<UnityAction> ChestClosedListeners = new List<UnityAction>();

    public static void AddChestClosedInvoker(Chest invoker)
    {
        ChestClosedInvokers.Add(invoker);
        foreach (UnityAction listener in ChestClosedListeners)
        {
            invoker.AddChestClosedListener(listener);
        }
    }

    public static void AddChestClosedListener(UnityAction listener)
    {
        ChestClosedListeners.Add(listener);
        foreach (Chest invoker in ChestClosedInvokers)
        {
            invoker.AddChestClosedListener(listener);
        }
    }
    #endregion
}
