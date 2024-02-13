using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputAction clickAction;
    [SerializeField] private InputAction positionAction;
    [SerializeField] private Transform holdObjectPoint;

    public event Action<Product> OnProductSelected;

    private Camera cameraReference;
    private int rayCastMask;
    private Product holdingObject;
    private readonly List<RaycastResult> raycastResults = new();

    private void Start()
    {
        rayCastMask = LayerMask.GetMask("Objects");
        cameraReference = Camera.main;
        clickAction.Enable();
        positionAction.Enable();
        clickAction.performed += OnTapped;
    }

    private void FixedUpdate()
    {
        if (!holdingObject) return;

        MoveToPoint(holdingObject.Rigidbody, holdObjectPoint.position);

        Quaternion forwardRotation = Quaternion.LookRotation(holdObjectPoint.forward, Vector3.up);
        Quaternion uprightRotation = Quaternion.FromToRotation(forwardRotation * Vector3.up, Vector3.up);
        Quaternion targetRotation = uprightRotation * forwardRotation;
        RotateToTarget(holdingObject.Rigidbody, targetRotation);
    }

    private void OnTapped(InputAction.CallbackContext obj)
    {
        if (holdingObject) return;
        Vector2 touchPosition = positionAction.ReadValue<Vector2>();

        if (IsTappingOnUIElement(touchPosition)) return;

        if (!TryPickup(touchPosition, out Product selectedProduct)) return;
        SetHoldingProduct(selectedProduct);
        OnProductSelected?.Invoke(selectedProduct);
    }

    private bool IsTappingOnUIElement(Vector2 touchPosition)
    {
        PointerEventData pointer = new(EventSystem.current) { position = touchPosition };

        // This can hit UI with "picking mode" set to "position"
        EventSystem.current.RaycastAll(pointer, raycastResults);
        return raycastResults.Count > 0;
    }

    public Product GetHoldingProduct()
    {
        return holdingObject;
    }

    public void SetHoldingProduct(Product product)
    {
        holdingObject = product;
    }

    public void DropHoldingProduct()
    {
        if (!holdingObject) return;
        holdingObject.Rigidbody.velocity = Vector3.zero;
        holdingObject = null;
    }

    private bool TryPickup(Vector3 screenTouchPosition, out Product pickedUpObj)
    {
        Ray ray = cameraReference.ScreenPointToRay(screenTouchPosition);
        Physics.Raycast(ray, out RaycastHit hitInfo, 200f, rayCastMask);
        if (hitInfo.rigidbody)
        {
            Rigidbody selectedObj = hitInfo.rigidbody;
            return selectedObj.TryGetComponent(out pickedUpObj);
        }

        pickedUpObj = null;
        return false;
    }

    private void MoveToPoint(Rigidbody objToMove, Vector3 targetPos)
    {
        if (Time.deltaTime <= 0) return;
        Vector3 delta = targetPos - objToMove.position;
        objToMove.velocity = delta / Time.deltaTime;
    }

    private void RotateToTarget(Rigidbody objToMove, Quaternion targetRotation)
    {
        if (Time.deltaTime <= 0) return;

        Quaternion delta = targetRotation * Quaternion.Inverse(objToMove.rotation);
        delta.ToAngleAxis(out float angle, out Vector3 axis);

        // We get an infinite axis in the event that our rotation is already aligned.
        if (float.IsInfinity(axis.x)) return;

        if (angle > 180f) angle -= 360f;

        // Multiply to undershoot slightly, for smoother movement
        Vector3 angular = 0.9f * Mathf.Deg2Rad * angle / Time.deltaTime * axis.normalized;
        objToMove.angularVelocity = angular;
    }
}
