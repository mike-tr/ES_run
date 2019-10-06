using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner<T> : MonoBehaviour where T : Component, IReusable
{
    Dictionary<T, SpawnObjectHolder<T>> holderByPrefab = new Dictionary<T, SpawnObjectHolder<T>>();
    Dictionary<Transform, SpawnObjectHolder<T>> holderByParent = new Dictionary<Transform, SpawnObjectHolder<T>>();

    public int maxSaved = 250;
    public float chanceForNew = 0.05f;

    public bool log = false;

    public string logStats() {
        string log = "";
        foreach (SpawnObjectHolder<T> holder in holderByPrefab.Values) {
            log += holder.parent.name + " - items count : " + holder.items + " - available :" + holder.reusable.Count;
        }
        return log;
    }

    public T getReusable(T prefab) {
        if (holderByPrefab.TryGetValue(prefab, out SpawnObjectHolder<T> holder)) {
            int items = holder.reusable.Count;
            if (items > 0) {             
                return onReuse(holder);
            }
            return onCreate(prefab, holder);
        }
        holder = createHolder(prefab);
        return onCreate(prefab, holder);
    }



    public SpawnObjectHolder<T> getHolder(T prefab) {
        if(holderByPrefab.TryGetValue(prefab, out SpawnObjectHolder<T> holder)) {
            return holder;
        }
        return createHolder(prefab);
    }

    protected SpawnObjectHolder<T> createHolder(T prefab) {
        Transform parent = new GameObject(getName(prefab)).transform;
        parent.parent = transform;
        parent.localEulerAngles = Vector2.zero;
        parent.localPosition = Vector2.right * 1000;

        SpawnObjectHolder<T> holder = new SpawnObjectHolder<T>(prefab, parent);
        holderByPrefab.Add(prefab, holder);
        holderByParent.Add(parent, holder); ;
        return holder;
    }

    protected virtual T onReuse(SpawnObjectHolder<T> holder) {
        T item = holder.reusable[0];
        item.transform.position = Vector2.zero;
        holder.reusable.RemoveAt(0);
        item.gameObject.SetActive(true);
        item.resetObject();
        return item;
    }

    protected virtual T onCreate(T prefab, SpawnObjectHolder<T> holder) {
        T item = Instantiate(prefab);
        item.setReusable();
        item.transform.parent = holder.parent;
        holder.items++;
        return item;
    }

    protected virtual string getName(T item) {
        return item.name;
    }

    public bool dumpObject(T item) {
        if (holderByParent.TryGetValue(item.transform.parent, out SpawnObjectHolder<T> holder)) {
            if (holder.reusable.Count > maxSaved) {
                holder.items--;
                return false;
            }
            holder.reusable.Add(item);
            item.gameObject.SetActive(false);
            return true;
        }
        return false;
    }
}

public class SpawnObjectHolder<T> {
    public T prefab;
    public Transform parent;
    public int items = 0;
    public int live { get { return items - reusable.Count; } }

    public List<T> reusable = new List<T>();

    public SpawnObjectHolder(T prefab, Transform holder) {
        this.prefab = prefab;
        this.parent = holder;
    }
}