using UnityEngine;
using DG.Tweening;
using System.Collections;

public class BatManager : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform launcherPoint;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform batHandleCenter;
    [SerializeField] private Animation batHandleAnimation;
    [SerializeField] private float launchVelocity = 15;
    private Vector3 mousePos;
    private const float animDuration = 0.1f;
    private bool isBallShot = false;

    private void Awake()
    {
        UiLevelManagerCanvas.OnShootBall += OnShootBall;
    }

    private void OnDestroy()
    {
        UiLevelManagerCanvas.OnShootBall -= OnShootBall;
    }

    private void OnShootBall()
    {
        if (isBallShot)
        {
            return;
        }
        mousePos = Input.mousePosition;
        mousePos.z = 10;
        target.position = Camera.main.ScreenToWorldPoint(mousePos);
        transform.LookAt(target);
        StartCoroutine(SwingBatAndShoot());
    }

    private IEnumerator SwingBatAndShoot()
    {
        isBallShot = true;
        yield return new WaitForEndOfFrame();
        batHandleAnimation.Play();
        yield return new WaitForSeconds(animDuration);
        GameObject ball = Instantiate(ballPrefab, launcherPoint.position, launcherPoint.rotation);
        ball.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, launchVelocity * 100, 0));
        isBallShot = false;
    }
}