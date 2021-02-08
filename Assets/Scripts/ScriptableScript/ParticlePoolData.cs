using UnityEngine;

[CreateAssetMenu(fileName = "ParticlePoolData", menuName = "ScriptableObject/ParticlePoolData")]
public class ParticlePoolData : ScriptableObject {

    public ParticlePoolStruct[] particlePools;
}

[System.Serializable]
public struct ParticlePoolStruct {

    public ParticleObject particle;
    public int poolSize;
}
