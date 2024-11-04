using UnityEngine;

public class Player : BaseUnit
{
    [SerializeField] private Animator anim; // Animator 참조

    private Vector2 moveDirection = Vector2.zero;
    private PlayerJoyStickMove joystick;
    private bool isMoving = false; // 이동 상태 플래그

    /// <summary>
    /// 조이스틱 참조를 설정하고 이벤트에 구독합니다.
    /// </summary>
    /// <param name="joystickMove">PlayerJoyStickMove 인스턴스</param>
    public void SetJoystick(PlayerJoyStickMove joystickMove)
    {
        if (joystickMove != null)
        {
            joystick = joystickMove;
            joystick.OnMoveInput += HandleMoveInput;
        }
        else
        {
            //Debug.LogWarning("PlayerJoyStickMove 인스턴스가 null입니다.");
        }
    }

    private void OnDisable()
    {
        if (joystick != null)
        {
            joystick.OnMoveInput -= HandleMoveInput;
        }
    }

    private void HandleMoveInput(Vector2 direction)
    {
        // 카메라의 방향을 기준으로 이동 방향을 설정
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            //Debug.LogError("메인 카메라가 존재하지 않습니다.");
            return;
        }

        // 카메라의 forward와 right 벡터를 구하여 방향을 계산
        Vector3 camForward = Vector3.ProjectOnPlane(mainCam.transform.forward, Vector3.up).normalized;
        Vector3 camRight = mainCam.transform.right;

        // 조이스틱 입력을 월드 방향으로 변환
        Vector3 moveDir = camRight * direction.x + camForward * direction.y;
        moveDirection = new Vector2(moveDir.x, moveDir.z).normalized;
    }

    private void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (moveDirection != Vector2.zero && moveDirection.magnitude > 0)
        {
            if (!isMoving)
            {
                anim.SetBool("Run", true);
                isMoving = true;
            }

            // 이동 벡터 계산
            Vector3 movement = new Vector3(moveDirection.x, 0, moveDirection.y) * moveSpeed * Time.deltaTime;

            // 캐릭터 회전
            if (movement != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);
            }

            // 플레이어 위치 업데이트
            transform.position += movement;
        }
        else
        {
            if (isMoving)
            {
                isMoving = false;
                anim.SetBool("Run", false);
                anim.SetTrigger("Idle");
            }
        }
    }
}
