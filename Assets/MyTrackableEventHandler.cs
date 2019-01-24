using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTrackableEventHandler : DefaultTrackableEventHandler {

    public MyComponent manager;

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        manager.trackables.Add(new ActiveTrackableInfo(mTrackableBehaviour.Trackable.ID, mTrackableBehaviour.TrackableName, mTrackableBehaviour.transform));
    }
}
