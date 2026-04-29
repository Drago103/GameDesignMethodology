using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour
{
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 endPos;
    [SerializeField] float duration;
    [SerializeField] float cooldown;
    
    private bool CanMove = true;
    private float T = 0f;

    IEnumerator Moveobject(Vector3 SPos, Vector3 EPos, float duration)
    {
        CanMove = false;
        T = 0f;
        while (T < 1f)
        {
            transform.position = Vector3.Lerp(SPos, EPos, T);
            T+= Time.deltaTime / duration;

            yield return null;
        }

        transform.position = EPos;

        Vector3 tempPos = SPos;
        startPos = EPos;
        endPos = tempPos;
        StartCoroutine(MoveCooldown(cooldown));
        
    }

    IEnumerator MoveCooldown(float cd)
    {
        Debug.Log("[obj] movement started");
        yield return new WaitForSeconds(cd);
        CanMove = true;
        Debug.Log("[obj] movement off cooldown");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            StartCoroutine(Moveobject(startPos, endPos, duration));
        }
    }
}
