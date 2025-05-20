using System.Collections;
using UnityEngine;

public class HandBoneAttack : MonoBehaviour
{
    [SerializeField] SpriteRenderer Shadow;
    [SerializeField] SpriteRenderer Bone;
    [SerializeField] SpriteRenderer BoneMark;
    [SerializeField] GameObject ImpactFX;
    [SerializeField] float startShadowScale = 3f;
    [SerializeField] float TimeTillImpact = 2f;
    [SerializeField] float ImpactLingerTime = .1f;
    [SerializeField] float boneFallHeight = 10f;
    Cached<ContactDamage> cached;
    ContactDamage dmg => cached[this];

    void Start() => StartCoroutine(BoneFall());
    IEnumerator BoneFall()
    {
        var t = 0f;
        Vector3 startScale = Shadow.transform.localScale;
        Vector3 startBonePos = Bone.transform.localPosition;
        Bone.enabled = false;
        while (t < 1)
        {
            t += Time.deltaTime / TimeTillImpact;
            var animT = t * t;
            animT = Mathf.Clamp01(animT);
            Shadow.color = new(0, 0, 0, animT * .75f);
            Shadow.transform.localScale = startScale * (startShadowScale * (1 - animT) + 1 * animT);
            Bone.transform.localPosition = startBonePos + Vector3.forward * boneFallHeight * (1 - (animT - .8f) / .2f);
            Bone.enabled = animT >= .8f;
            yield return new WaitForFixedUpdate();
        }
        ImpactFX.SetActive(true);
        dmg.enabled = true;
        BoneMark.enabled = false;
        yield return new WaitForFixedUpdate();
        dmg.enabled = false;
        yield return new WaitForSeconds(ImpactLingerTime);
        while (t > 0)
        {
            t -= Time.deltaTime;
            Shadow.color = new(0, 0, 0, t * .75f);
            Bone.color = new(1, 1, 1, t);
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
}
