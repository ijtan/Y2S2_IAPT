using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LoadRemoteAssets : MonoBehaviour
{
    [SerializeField] private string _label;
    void Start()
    {
        Get(_label);
    }

    private async Task Get(string label)
    {
        var locations = await Addressables.LoadResourceLocationsAsync(label).Task;

        foreach (var location in locations)
            await Addressables.InstantiateAsync(location).Task;
    }

}
