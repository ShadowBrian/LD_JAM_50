using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class VolcanoFinalAnimation : MonoBehaviour
{
    private static int AtmosphereThickness = Shader.PropertyToID("_AtmosphereThickness");
    public float animationTime;

    public CinemachineVirtualCamera animationCamera;
    public AnimationCurve scaleCurve;
    public ParticleSystem smokeParticles;
    public ParticleSystem lavaParticles;
    public Material skyboxMaterial;

    public float minShakeAmplitude;
    public float maxShakeAmplitude;

    private bool _playingLavaParticles;

    //Unity Functions
    //====================================================================================================================//

    private void OnDisable()
    {
        skyboxMaterial.SetFloat(AtmosphereThickness, 1f);
    }

    //====================================================================================================================//

    public void StartAnimation()
    {
        skyboxMaterial.SetFloat(AtmosphereThickness, 5f);
        //StartCoroutine(PlayAnimationCoroutine());
    }

    private IEnumerator PlayAnimationCoroutine()
    {
        smokeParticles.Stop();
        animationCamera.Priority = 1000;
        CameraShake.SetActiveCamera(animationCamera);
        CameraShake.Shake(minShakeAmplitude, animationTime / 2f);
        yield return new WaitForSeconds(1.5f);

        for (float t = 0; t <= animationTime; t += Time.deltaTime)
        {
            var tValue = t / animationTime;
            var currentScale = transform.localScale;

            currentScale.y = scaleCurve.Evaluate(tValue);

            if (_playingLavaParticles == false && tValue > 0.9f)
            {
                _playingLavaParticles = true;
                CameraShake.Shake(0f, 0f);

                yield return new WaitForSeconds(0.5f);

                CameraShake.Shake(maxShakeAmplitude, 1000f);
                lavaParticles.Play();
            }


            transform.localScale = currentScale;
            yield return null;
        }
    }
}
