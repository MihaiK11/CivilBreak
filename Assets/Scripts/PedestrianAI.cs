using UnityEngine;
using System.Collections.Generic;

public class PedestrianAI : MonoBehaviour
{
    [Header("Параметры движения")]
    public float walkSpeed = 0.5f;
    public float rotationSpeed = 300f;

    [Header("Вейпоинты")]
    public PedestrianWaypoint currentTarget;
    private PedestrianWaypoint previousTarget;

    [Header("Анимации")]
    public string animPhoneWalk = "locom_f_phoneWalking_40f";
    public string animBasicWalk = "locom_f_basicWalk_30f";
    public string animJogging = "locom_m_jogging_30f";

    [Header("Точность попадания в точку")]
    public float reachThreshold = 0.04f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning($"{gameObject.name} не содержит Animator!");
            return;
        }

        // Выбираем анимацию и настраиваем скорость
        int index = Random.Range(0, 3);
        switch (index)
        {
            case 0:
                animator.Play(animPhoneWalk);
                walkSpeed = 0.05f;
                break;
            case 1:
                animator.Play(animBasicWalk);
                walkSpeed = 0.1f;
                break;
            case 2:
                animator.Play(animJogging);
                walkSpeed = 0.15f;
                break;
        }
    }

    void Update()
    {
        if (currentTarget == null) return;

        Vector3 direction = currentTarget.transform.position - transform.position;
        float distance = direction.magnitude;

        if (distance < reachThreshold)
        {
            ChooseNextWaypoint();
            return;
        }

        // Резкий поворот (только по XZ)
        Vector3 lookDir = new Vector3(direction.x, 0f, direction.z);
        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }

        // Движение — теперь с учётом Y
        transform.position += direction.normalized * walkSpeed * Time.deltaTime;
    }

    void ChooseNextWaypoint()
    {
        if (currentTarget.connected == null || currentTarget.connected.Count == 0) return;

        List<PedestrianWaypoint> options = new List<PedestrianWaypoint>(currentTarget.connected);
        if (previousTarget != null && options.Count > 1)
            options.Remove(previousTarget);

        previousTarget = currentTarget;
        currentTarget = options[Random.Range(0, options.Count)];
    }
}