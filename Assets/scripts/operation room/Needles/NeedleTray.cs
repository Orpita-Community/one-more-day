using System.Collections;
using UnityEngine;

public class NeedleTray : MonoBehaviour
{
    [System.Serializable]
    private struct NeedlesNumber
    {
        public NeedleType type;
        public int number;
    }
    [SerializeField] private NeedlesNumber[] needlesNumbers;
    [SerializeField] private GameObject[] NeedlesPrefabs;
    [SerializeField] private Vector2[] NeedleOffsetFromCenter;

    void Start()
    {
        GameEventHandler.OnNeedleUse += UseNeedle;
    }

    private void UseNeedle(NeedleType type)
    {
        for (int needleIndex = 0; needleIndex < needlesNumbers.Length; needleIndex++)
        {
            if (needlesNumbers[needleIndex].type == type)
            {
                needlesNumbers[needleIndex].number--;
                if (needlesNumbers[needleIndex].number > 0)
                {
                    StartCoroutine(SpawnNeedle(needleIndex));
                }
                return;
            }
        }
    }

    private IEnumerator SpawnNeedle(int index)
    {
        yield return new WaitForSeconds(0.2f);
        GameObject m_needle = Instantiate(NeedlesPrefabs[index], transform);
        m_needle.transform.SetLocalPositionAndRotation(NeedleOffsetFromCenter[index], Quaternion.identity);
    }
}
