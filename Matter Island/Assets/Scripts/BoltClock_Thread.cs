using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class BoltClock_Thread {
    private ThreadStart my_thread;
    void startThread (){
        this.my_thread = delegate{
       //     MapDataThread (callback);
        };
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
