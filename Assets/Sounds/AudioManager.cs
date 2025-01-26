using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioPrefab;

    private static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        
    }

    public void PlayClip(AudioClip clip)
    {
        // create a new audio source
        AudioSource source = Instantiate(_audioPrefab);

        // set its variables
        source.clip = clip;
        source.volume = 1f;

        // play the sound
        source.Play();

        // ensure it stays alive, say when we reload RN
        DontDestroyOnLoad(source);

        // destroy GO after play time
        Destroy(source.gameObject, clip.length);
    }

}
