using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Managers
{
    public static class AudioManager
    {
        /*
            1 -> 100%, 0 -> 0%
        */
        public static float MasterVolume { get; private set; } = 1f;
        public static float BGMVolume { get; private set; } = 1f;
        public static float SFXVolume { get; private set; } = 1f;

        public static float EffectVolume { get { return SFXVolume * MasterVolume; } }

        public static string CurrentSong { get; private set; } = "";

        public static void Play(string name, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f)
        {
            //ResourcesManager.FindSoundEffect(name).Play(volume: SFXVolume * MasterVolume, pitch: pitch, pan: pan);
            Play(ResourcesManager.FindSoundEffect(name), volume, pitch, pan);
        }

        public static void Play(SoundEffect sound, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f)
        {
            if((SFXVolume * MasterVolume) > 0f)
                sound.Play(volume: SFXVolume * MasterVolume * volume, pitch: pitch, pan: pan);
        }

        public static void PlayMusic(string song)
        {
            if (!CurrentSong.Equals(song))
            {
                MediaPlayer.Volume = MasterVolume * BGMVolume;
                MediaPlayer.Play(ResourcesManager.FindSong(song));
                MediaPlayer.IsRepeating = true;
                CurrentSong = song;
            }
        }

        public static void AdjustMasterVolume(float amount)
        {
            MasterVolume += amount;
            MasterVolume = Math.Clamp(MasterVolume, 0f, 1f);
            MediaPlayer.Volume = MasterVolume * BGMVolume;
            Play("Pause");
        }

        public static void AdjustSFXVolume(float amount)
        {
            SFXVolume += amount;
            SFXVolume = Math.Clamp(SFXVolume, 0f, 1f);
            Play("Pause");
        }

        public static void AdjustBGMVolume(float amount)
        {
            BGMVolume += amount;
            BGMVolume = Math.Clamp(BGMVolume, 0f, 1f);
            Play("Pause");
            MediaPlayer.Volume = MasterVolume * BGMVolume;
        }
    }
}
