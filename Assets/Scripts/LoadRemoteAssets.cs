using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LoadRemoteAssets : MonoBehaviour
{
    public string label;
    // Start is called before the first frame update
    void Start()
    {
        Get(label);
    }


    private async Task Get(string label)
    {
        var locs = await Addressables.LoadResourceLocationsAsync(label).Task;
        foreach(var loc in locs)
        {
            await Addressables.Instantiate(loc).Task;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
