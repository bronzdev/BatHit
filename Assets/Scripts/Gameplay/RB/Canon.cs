using UnityEngine;

public class Canon : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform launcherPoint;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float launchVelocity = 15;
    private Vector3 mousePos;

    private void Update()
    {
        mousePos = Input.mousePosition;
        mousePos.z = 10;
        target.position = Camera.main.ScreenToWorldPoint(mousePos);
        transform.LookAt(target);

        if (Input.GetButtonDown("Fire1"))
        {
            GameObject ball = Instantiate(projectile, launcherPoint.position, launcherPoint.rotation);
            ball.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, launchVelocity * 100, 0));
        }

        if (Input.GetMouseButtonDown(2))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}