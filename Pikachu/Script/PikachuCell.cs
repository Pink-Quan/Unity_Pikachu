using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pikachu
{
    public class PikachuCell : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D boxCollider;
        public Color type { get; private set; }
        public bool isChoosing { get; private set; } = false;
        public Vector2Int index { get; private set; }
        Vector3 baseScale;

        private void Awake()
        {
            spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            boxCollider = transform.GetComponent<BoxCollider2D>();
            baseScale= transform.localScale;
        }

        public void Init(Vector3 spawnPos, Color type, Transform parten, Vector2Int index)
        {
            transform.position = spawnPos;
            this.type = type;
            spriteRenderer.color = type;
            transform.parent = parten;
            this.index = index;

        }

        public void ResetCell()
        {
            Invoke("DelayReset",0.2f);

        }
        private void DelayReset()
        {
            isChoosing = false;
            StopAllCoroutines();
            //Debug.Log("Clear " + index);
            transform.localScale = baseScale;
        }
        private void OnMouseDown()
        {
            ChooseCell();
            isChoosing = true;
        }
        private void ChooseCell()
        {
            PikachuManager.instance.ChooseOneCell(this);
            StartCoroutine(StartAnimation());
        }
        private IEnumerator StartAnimation()
        {
            while (true)
            {
                StartCoroutine(ScaleAnimtion(baseScale + baseScale * 0.15f, 0.5f));
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(ScaleAnimtion(baseScale - baseScale * 0.15f, 0.5f));
                yield return new WaitForSeconds(0.5f);
            }
        }
        private IEnumerator ScaleAnimtion(Vector3 to, float duration)
        {
            float clk = 0f;
            Vector3 baseScale = transform.localScale;
            while (clk <= duration)
            {
                clk += Time.deltaTime;
                transform.localScale = Vector3.Lerp(baseScale, to, clk / duration);

                yield return null;
            }
            transform.localScale = to;
        }
    }
}