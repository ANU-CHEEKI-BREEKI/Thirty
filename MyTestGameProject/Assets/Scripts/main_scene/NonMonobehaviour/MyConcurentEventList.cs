using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class MyConcurentEventList
{
    List<Action> list = new List<Action>();
    object sync = new object();

    public void Add(Action item)
    {
        lock(sync)
        {
            list.Add(item);
        }
    }

    public bool Remove(Action item)
    {
        lock (sync)
        {
            return list.Remove(item);
        }
    }

    public int Count
    {
        get
        {
            lock (sync)
            {
                return list.Count;
            }
        }
    }

    public void DoAll()
    {
        lock(sync)
        {
            if(list.Count > 0)
            {
                foreach (var item in list)
                    item();
                list.Clear();
            }
        }
    }
}
