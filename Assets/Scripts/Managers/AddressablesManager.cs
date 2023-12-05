using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace com.medcare360.utt
{
    public class AddressablesManager : MonoBehaviour
    {
        private static AddressablesManager _instance;

        private static void UpdateInstance()
        {
            if (_instance != null)
                return;

            _instance = FindObjectOfType(typeof(AddressablesManager)) as AddressablesManager;
        }

        private IEnumerator LoadAddressablesWithKeys<T>(IList<string> keys,
            Delegate loadSuccessDelegate, object[] loadDelegateParams,
            Delegate postSuccessDelegate, object[] postDelegateParams)
        {
            AsyncOperationHandle<IList<IResourceLocation>> locations =
                Addressables.LoadResourceLocationsAsync(keys,
                Addressables.MergeMode.Union, typeof(T));

            yield return locations;

            var loadOptions = new List<AsyncOperationHandle>(locations.Result.Count);

            foreach (IResourceLocation location in locations.Result)
            {
                AsyncOperationHandle<T> asset = Addressables.LoadAssetAsync<T>(location);

                yield return asset;

                object[] localCopy = loadDelegateParams;

                if (localCopy == null)
                {
                    localCopy = new object[1];
                    localCopy[0] = asset;
                }
                else
                {
                    Array.Resize(ref localCopy, localCopy.Length + 1);

                    localCopy[1] = localCopy[0];
                    localCopy[0] = asset;
                }
                loadSuccessDelegate?.DynamicInvoke(localCopy);
            }

            postSuccessDelegate?.DynamicInvoke(postDelegateParams);
            //OnInitialized.Invoke(this, exeController);
        }

        public static void LoadSOWithKeys<T>(IList<string> keys,
            Delegate loadSuccessDelegate, object[] loadDelegateParams,
            Delegate postSuccessDelegate, object[] postDelegateParams)
        {
            UpdateInstance();
            _instance.StartCoroutine(_instance.LoadAddressablesWithKeys<T>(keys,
                loadSuccessDelegate, loadDelegateParams,
                postSuccessDelegate, postDelegateParams));
        }
    }
}