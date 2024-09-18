using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// необходимо чтобы название скрипта и название класса совпадали
public class CustomCharacterController : MonoBehaviour{ 	
	public Animator anim;
    public Rigidbody rig;
    public Transform mainCamera;
    public float jumpForce = 3.5f; 
    public float walkingSpeed = 2f;
    public float runningSpeed = 6f;
    public float currentSpeed;
    private float animationInterpolation = 1f;
    public InventoryManager inventoryManager;
    public QuickslotInventory quickslotInventory;
    // Start is called before the first frame update

    public Transform AimTarget;
    void Start()
    {
        // Прекрепляем курсор к середине экрана
        Cursor.lockState = CursorLockMode.Locked;
        // и делаем его невидимым
        Cursor.visible = false;
    }
    void Run()
    {
        animationInterpolation = Mathf.Lerp(animationInterpolation, 1.5f, Time.deltaTime * 3);
        anim.SetFloat("x", Input.GetAxis("Horizontal") * animationInterpolation);
        anim.SetFloat("y", Input.GetAxis("Vertical") * animationInterpolation);

        currentSpeed = Mathf.Lerp(currentSpeed, runningSpeed, Time.deltaTime * 3);
    }
    void Walk()
    {
        // Mathf.Lerp - отвчает за то, чтобы каждый кадр число animationInterpolation(в данном случае) приближалось к числу 1 со скоростью Time.deltaTime * 3.
        // Time.deltaTime - это время между этим кадром и предыдущим кадром. Это позволяет плавно переходить с одного числа до второго НЕЗАВИСИМО ОТ КАДРОВ В СЕКУНДУ (FPS)!!!
        animationInterpolation = Mathf.Lerp(animationInterpolation, 1f, Time.deltaTime * 3);
        anim.SetFloat("x", Input.GetAxis("Horizontal") * animationInterpolation);
        anim.SetFloat("y", Input.GetAxis("Vertical") * animationInterpolation);

        currentSpeed = Mathf.Lerp(currentSpeed, walkingSpeed, Time.deltaTime * 3);
    }
    public void ChangeLayerWEight(float newLayerWeight)
    {
        StartCoroutine(SmoothLayerWeightChange(anim.GetLayerWeight(1), newLayerWeight, 0.3f));
    }
    IEnumerator SmoothLayerWeightChange(float oldWeight, float newWeight,float changeDuration)
    {
        float elapsed = 0.0f;
        while (elapsed < changeDuration) 
            {
                float curentWeight = Mathf.Lerp(oldWeight, newWeight, elapsed / changeDuration);
                anim.SetLayerWeight(1, curentWeight);
                elapsed += Time.deltaTime;
                yield return null;
            }
            anim.SetLayerWeight(1, newWeight);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (quickslotInventory.activeSlot != null)
            { 
                if(quickslotInventory.activeSlot.item !=null)
                {
                    if (quickslotInventory.activeSlot.item.itemType == ItemType.Instrument)
                    {
                        if (inventoryManager.IsOpened == false)
                        {
                            anim.SetBool("Hit", true);
                        }
                    }
                }

            }
        }
        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            anim.SetBool("Hit", false);
        }
        // Устанавливаем поворот персонажа когда камера поворачивается 
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,mainCamera.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        
        // Зажаты ли кнопки W и Shift?
        if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            // Зажаты ли еще кнопки A S D?
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                // Если да, то мы идем пешком
                Walk();
            }
            // Если нет, то тогда бежим!
            else
            {
                Run();
            }
        }
        // Если W & Shift не зажаты, то мы просто идем пешком
        else
        {
            Walk();
        }
        //Если зажат пробел, то в аниматоре отправляем сообщение тригеру, который активирует анимацию прыжка
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                anim.SetBool("jump running", true);
            }
            else
            {
                anim.SetBool("jump", true);
            }
              // Setează bool-ul pentru săritură
            Jump();
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("jump"))
        {
            anim.SetBool("jump", false);  // Resetează bool-ul
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Running Jump"))
        {
            anim.SetBool("jump running", false);  // Resetează bool-ul
        }
        Ray desiredTirgetRay = mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        Vector3 desiredTaergetPosition = desiredTirgetRay.origin + desiredTirgetRay.direction* 0.7f;
        AimTarget.position = desiredTaergetPosition;

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        // Здесь мы задаем движение персонажа в зависимости от направления в которое смотрит камера
        // Сохраняем направление вперед и вправо от камеры 
        Vector3 camF = mainCamera.forward;
        Vector3 camR = mainCamera.right;
        // Чтобы направления вперед и вправо не зависили от того смотрит ли камера вверх или вниз, иначе когда мы смотрим вперед, персонаж будет идти быстрее чем когда смотрит вверх или вниз
        // Можете сами проверить что будет убрав camF.y = 0 и camR.y = 0 :)
        camF.y = 0;
        camR.y = 0;
        Vector3 movingVector;
        // Тут мы умножаем наше нажатие на кнопки W & S на направление камеры вперед и прибавляем к нажатиям на кнопки A & D и умножаем на направление камеры вправо
        movingVector = Vector3.ClampMagnitude(camF.normalized * Input.GetAxis("Vertical") * currentSpeed + camR.normalized * Input.GetAxis("Horizontal") * currentSpeed,currentSpeed);
        // Magnitude - это длинна вектора. я делю длинну на currentSpeed так как мы умножаем этот вектор на currentSpeed на 86 строке. Я хочу получить число максимум 1.
        anim.SetFloat("magnitude", movingVector.magnitude/currentSpeed);
        Debug.Log(movingVector.magnitude / currentSpeed);
        // Здесь мы двигаем персонажа! Устанавливаем движение только по x & z потому что мы не хотим чтобы наш персонаж взлетал в воздух
        rig.velocity = new Vector3(movingVector.x, rig.velocity.y,movingVector.z);
        // У меня был баг, что персонаж крутился на месте и это исправил с помощью этой строки
        rig.angularVelocity = Vector3.zero;
    }
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
    public void Jump()
    {
        // Выполняем прыжок по команде анимации.
        rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    public void Hit()
    {
        foreach (Transform item in quickslotInventory.allWeapons)
        {
            if (item.gameObject.activeSelf)
            {
                item.GetComponent<GatherResurces>().GatherResource();
            }
        }
    }
}