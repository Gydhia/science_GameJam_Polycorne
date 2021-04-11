using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour
{   
    public enum SoundNames
    {
        WhispCrash,
        StarDeparture,
        WhispArrival,

        LevelCompletionSound,

        DragCard,
        HoverCard,
        MergeCard
    }
    public enum MusicNames
    {
        MenuTheme,

        MainTheme,
        ActionTheme
    }

    private Dictionary<MusicNames, List<AudioSource>> MusicSources = new Dictionary<MusicNames, List<AudioSource>>();
    private Dictionary<SoundNames, List<AudioSource>> SoundSources = new Dictionary<SoundNames, List<AudioSource>>();

    public List<MusicSO> Musics = new List<MusicSO>();
    public List<SoundSO> Sounds = new List<SoundSO>();

    public GameObject MusicsContainer;
    public GameObject SoundsContainer;

    public AudioMixer AudioMixer;
    public AudioMixerGroup AudioMixerGroup;
    public float FadingTime = 1f;

    private MusicSO _selectedMusic;
    private Coroutine _musicCoroutine;

    public static SoundController Instance;
    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }
    
    public void DoDatMusicWork()
    {
        PlayMusic(MusicNames.MenuTheme);
    }

    public void DoDatSoundWork()
    {
        StartCoroutine(testsound());      
    }
    public IEnumerator testsound()
    {
        for (int i = 0; i < 100; i++) {
            PlaySound(SoundNames.WhispCrash);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        }
    }

    public void DoDatMusicStop()
    {
        StopMusic();
    }

    // MUSICS
    public void PlayMusic(MusicNames musicID)
    {
        _selectedMusic = Musics.Find(music => music.MusicID == musicID);
        if(_selectedMusic == null) {
            Debug.LogError("The music to play has not been found -- " + musicID);
            return; 
        }
        if (MusicSources.ContainsKey(musicID)) {
            Debug.LogWarning("The music is already being played -- " + musicID);
            return;
        }
            
        _musicCoroutine = StartCoroutine(PlayingMusic(_selectedMusic));
    }

    public IEnumerator PlayingMusic(MusicSO music)
    {
        List<AudioSource> audios = new List<AudioSource>();
        MusicSources.Add(music.MusicID, audios);

        foreach (ClipSO clip in music.MainLines) {
            AudioSource audio = SetupAudioMusic(clip);
            audios.Add(audio);
        }

        if (music.BarsToWait[0])
            yield return new WaitForSeconds(music.MainLines[0].clip.length);

        foreach (ClipSO clip in music.MelodicLines) {
            AudioSource audio = SetupAudioMusic(clip);
            audios.Add(audio);
        }

        if (music.BarsToWait[1])
            yield return new WaitForSeconds(music.MelodicLines[0].clip.length);

        foreach (ClipSO clip in music.AccompanimentLines) {
            AudioSource audio = SetupAudioMusic(clip);
            audios.Add(audio);
        }
    }

    public void StopMusic()
    {
        if (_selectedMusic == null) return;
        StopCoroutine(_musicCoroutine);

        StartCoroutine(FadeMixerGroup.StartFade(AudioMixer, "Fading", FadingTime, -40f));
        StartCoroutine(StoppingMusic(_selectedMusic));
    }

    public IEnumerator StoppingMusic(MusicSO music)
    {
        yield return new WaitForSeconds(FadingTime);

        for (int i = 0; i < MusicSources[_selectedMusic.MusicID].Count; i++)
        {
            AudioSource audio = MusicSources[_selectedMusic.MusicID][i];
            Destroy(audio.gameObject);
        }
        MusicSources[_selectedMusic.MusicID].Clear();

        MusicSources.Remove(music.MusicID);
        _selectedMusic = null;
        AudioMixer.SetFloat("Fading", 0f);
    }

    public AudioSource SetupAudioMusic(ClipSO clipObject)
    {
        AudioSource audio = Instantiate(Resources.Load("Prefabs/AudioPrefab") as GameObject, MusicsContainer.transform).GetComponent<AudioSource>();
        audio.clip = clipObject.clip;
        audio.loop = true;
        audio.outputAudioMixerGroup = AudioMixerGroup;
        
        if(clipObject.Echo != null) {
            AudioEchoFilter echo = audio.gameObject.AddComponent<AudioEchoFilter>();
            echo.delay = (60 / clipObject.BPM) * 1000f;
            echo.decayRatio = clipObject.Echo.DecayRatio;
            echo.dryMix = clipObject.Echo.DryMix;
        }
        if(clipObject.Reverb != null) {
            AudioReverbFilter reverb = audio.gameObject.AddComponent<AudioReverbFilter>();
            reverb.reverbPreset = clipObject.Reverb.ReverbPreset;
        }

        audio.Play();
        return audio;
    }

    // SOUNDS
    public void PlaySound(SoundNames soundID)
    {
        StartCoroutine(PlayingSound(Sounds.Find(sound => sound.SoundID == soundID)));
    }

    public IEnumerator PlayingSound(SoundSO sound)
    {
        AudioSource audio = Instantiate(Resources.Load("Prefabs/AudioPrefab") as GameObject, SoundsContainer.transform).GetComponent<AudioSource>();

        if (!SoundSources.ContainsKey(sound.SoundID))
            SoundSources.Add(sound.SoundID, new List<AudioSource>());
        SoundSources[sound.SoundID].Add(audio);

        audio.PlayOneShot(sound.Sound);

        yield return new WaitForSeconds(sound.Sound.length);

        SoundSources[sound.SoundID].Remove(audio);
        if(SoundSources[sound.SoundID].Count <= 0)
            SoundSources.Remove(sound.SoundID);

        Destroy(audio.gameObject);
    }
}
