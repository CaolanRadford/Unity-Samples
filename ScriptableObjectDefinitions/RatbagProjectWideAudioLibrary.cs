using UnityEngine;

namespace ScriptableObjectDefinitions
{
    [CreateAssetMenu]
    public class RatbagProjectWideAudioLibrary : ScriptableObject
    {
        //ratbag Sounds
        public AudioClip[] ratbag_Floored;
        public AudioClip[] ratBag_GettingUp;
        public AudioClip[] ratBag_Landed;
        public AudioClip[] ratbag_turning;
        public AudioClip[] ratbag_Hitting;
        public AudioClip[] ratbag_feetLanded;

        //bouncer sounds
        public AudioClip[] bouncer_Floored;
        public AudioClip[] bouncer_GettingUp;
        public AudioClip[] bouncer_Landed;
        public AudioClip[] bouncer_Punching;
        public AudioClip[] bouncer_Targetting;
        public AudioClip[] bouncer_GettingHit; 

        //vent
        public AudioClip[] vent_Opening; 

        //Misc
        public AudioClip[] cashMoney;
        public AudioClip[] coinInsert;
        public AudioClip[] TVStatic;
        public AudioClip[] PassingFlooredbouncer;
        public AudioClip[] HingeSqueek;
    }
}
