using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;



public class BannerAdScript : MonoBehaviour
{
    static BannerAdScript instance;

#if UNITY_IOS
    string game_id="4746965";
    string placement_id="Banner_iOS";
#elif UNITY_ANDROID
    string game_id="4746964";
    string placement_id="Banner_Android";
#endif

    private void Awake()
    {
        AssignInstance();
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        InitializeAd();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AssignInstance()
    {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeAd()
    {
        Advertisement.Initialize(game_id, true);
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(placement_id);
    }
}
