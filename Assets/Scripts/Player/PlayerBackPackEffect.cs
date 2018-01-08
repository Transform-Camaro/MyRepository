
using UnityEngine;

public class PlayerBackPackEffect
{
    public Transform mTransform;

    private ParticleSystem mParticleSystem;
    private ParticleSystem[] childParticleSystems;

    public PlayerBackPackEffect(Transform transform)
    {
        mTransform = transform;


        float mSpeed = GameController.instance.gameConfig.playerInfo.timeOfOneGrid;
        float duration = mSpeed * GameController.instance.UNIT_TIME;

        mParticleSystem = mTransform.GetComponent<ParticleSystem>();
        
        childParticleSystems = mTransform.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < childParticleSystems.Length; i++)
        {
            ParticleSystem.MainModule childmain = childParticleSystems[i].main;
            childmain.duration = duration * 2;
        }

        ParticleSystem.MainModule main = mParticleSystem.main;
        main.duration = duration;
    }

    public void PlayEffect()
    {
        mParticleSystem.Play();
    }

}
