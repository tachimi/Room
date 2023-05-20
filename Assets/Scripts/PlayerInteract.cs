using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Transform _inventory;
    [SerializeField] private Transform _interactableInventory;

    private InteractableItem _currentInteractableItem;
    private InteractableItem _lastInteractableItem;


    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, 3f))
        {
            var door = hitInfo.collider.GetComponent<Door>();

            if (door != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    door.SwitchDoorState();
                }
            }

            _currentInteractableItem = hitInfo.collider.GetComponent<InteractableItem>();
            
            if (_currentInteractableItem != null)
            {
                SelectItem();
                CollectItem(hitInfo);
            }
            else
            {
                ClearSelection();
            }
        }
        else
        {
            ClearSelection();
        }

        if (_inventory.childCount > 0)
        {
            DropItem();
        }
    }

    private void SelectItem()
    {
        ClearSelection();
        _currentInteractableItem.SetFocus();
        _lastInteractableItem = _currentInteractableItem;
    }

    private void ClearSelection()
    {
        if (_lastInteractableItem != null)
        {
            _lastInteractableItem.RemoveFocus();
            _lastInteractableItem = null;
        }
    }

    private void CollectItem(RaycastHit hitInfo)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_inventory.childCount > 0)
            {
                var childRigidBody = _inventory.GetComponentInChildren<Rigidbody>();
                var childTransform = _inventory.GetChild(0);

                childRigidBody.isKinematic = false;
                _inventory.DetachChildren();
                childTransform.SetParent(_interactableInventory);

                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var direction = ray.direction;

                childRigidBody.AddForce(direction * 2f, ForceMode.Impulse);
            }


            var itemPosition = hitInfo.transform.GetComponent<Transform>();
            itemPosition.position = _inventory.position;
            itemPosition.rotation = _inventory.rotation;
            itemPosition.SetParent(_inventory);
            var itemRigidbody = hitInfo.transform.GetComponent<Rigidbody>();
            itemRigidbody.isKinematic = true;
        }
    }

    private void DropItem()
    {
        var childRigidBody = _inventory.GetComponentInChildren<Rigidbody>();
        var childTransform = _inventory.GetChild(0);
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            childRigidBody.isKinematic = false;
            _inventory.DetachChildren();
            childTransform.SetParent(_interactableInventory);

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var direction = ray.direction;

            childRigidBody.AddForce(direction * 10f, ForceMode.Impulse);
        }
    }
}