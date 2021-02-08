using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ParticleController : MonoBehaviour
{
    [SerializeField]
    private ParticlePoolData particlePoolData = null;

    [SerializeField]
    private Transform particleParent = null;
    [SerializeField]
    private Transform textParent = null;

    [SerializeField]
    private JudgeText judgeTextObj = null;
    [SerializeField]
    private int generateTextCount = 20;

    private int[] particleCurrentElement = null;
    private int textCurrentElement = 0;
    private JudgeText[] judgeTexts = null;

    private ParticleObject[][] particlesObjects = null;

    public IEnumerator Init()
    {
        float nextFlameTime = Clac.NextFlameTime();
        ParticlePoolStruct[] particlePools = particlePoolData.particlePools;

        Array.Resize(ref particleCurrentElement, particlePools.Length);
        Array.Resize(ref particlesObjects, particlePools.Length);
        for (int i = 0; i < particlesObjects.Length; i++) {

            Array.Resize(ref particlesObjects[i], particlePools[i].poolSize);
            for (int j = 0; j < particlesObjects[i].Length; j++) {

                // 処理時間がフレームレートの80%を超えたら次のフレームへ
                if (Time.realtimeSinceStartup >= nextFlameTime) {

                    yield return null;
                    nextFlameTime = Clac.NextFlameTime();
                }

                particlesObjects[i][j] = Instantiate(particlePools[i].particle, particleParent);
            }
        }

        Array.Resize(ref judgeTexts, generateTextCount);
        for (int i = 0; i < generateTextCount; i++) {

            // 処理時間がフレームレートの80%を超えたら次のフレームへ
            if (Time.realtimeSinceStartup >= nextFlameTime) {

                yield return null;
                nextFlameTime = Clac.NextFlameTime();
            }

            judgeTexts[i] = Instantiate(judgeTextObj, textParent);
        }
    }

    public void UpdateManage()
    {
        foreach (JudgeText judgeText in judgeTexts) {

            if (judgeText.isShow) {

                judgeText.UpdateManage();
            }
        }

        foreach (ParticleObject[] particleObjects in particlesObjects) {
            foreach (ParticleObject particleObject in particleObjects) {

                if (particleObject.isPlaying) {

                    particleObject.CheckPlaying();
                }
            }
        }
    }

    /// <summary>
    /// 判定文字を表示する
    /// </summary>
    /// <param name="judgeLane"></param>
    /// <param name="judgeSprite"></param>
    public void ShowJudgeText(Transform judgeLineTrans, Sprite judgeSprite)
    {
        judgeTexts[textCurrentElement].playAnim(judgeLineTrans, judgeSprite);

        textCurrentElement++;

        if (textCurrentElement >= generateTextCount) {

            textCurrentElement = 0;
        }
    }

    public void PlayParticle(Transform playTrans, int playType)
    {
        particlesObjects[playType][particleCurrentElement[playType]].Play(playTrans);

        particleCurrentElement[playType]++;

        if (particleCurrentElement[playType] >= particlePoolData.particlePools[playType].poolSize) {

            particleCurrentElement[playType] = 0;
        }
    }
}
