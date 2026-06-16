//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Valve.VR.Extras
{
    public class SteamVR_LaserPointer : MonoBehaviour
    {
        public SteamVR_Behaviour_Pose pose;

        public SteamVR_Action_Boolean activePointer = SteamVR_Input.GetBooleanAction("ActivePointer");
        //public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.__actions_default_in_InteractUI;
        public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");
        public bool isDefaultActivePointer = true;
        public bool isDefaultShowPointer = false;
        public bool IsDefaultShowPointer
        {
            get
            {
                return isDefaultShowPointer;
            }
            set
            {
                isDefaultShowPointer = value;
                if (value)
                    pointer.SetActive(true);
                else
                {
                    pointer.SetActive(false);
                    PointerOutFun();
                }
            }
        }
        public Color color;
        public float thickness = 0.002f;
        public Color inColor = Color.green;
        public GameObject holder;
        public GameObject pointer;
        public bool addRigidBody = false;
        public Transform reference;
        public event PointerEventHandler PointerIn;
        public event PointerEventHandler PointerOut;
        public event PointerEventHandler PointerClick;
        public event PointerEventHandler PointerClickUp;

        Transform previousContact = null;


        private void Start()
        {
            if (pose == null)
                pose = this.GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.LogError("No SteamVR_Behaviour_Pose component found on this object", this);

            if (interactWithUI == null)
                Debug.LogError("No ui interaction action has been set on this component.", this);


            holder = new GameObject();
            holder.transform.parent = this.transform;
            holder.transform.localPosition = Vector3.zero;
            holder.transform.localRotation = Quaternion.identity;

            pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pointer.transform.parent = holder.transform;
            pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
            pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
            pointer.transform.localRotation = Quaternion.identity;
            BoxCollider collider = pointer.GetComponent<BoxCollider>();
            if (addRigidBody)
            {
                if (collider)
                {
                    collider.isTrigger = true;
                }
                Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;
            }
            else
            {
                if (collider)
                {
                    Object.Destroy(collider);
                }
            }
            Material newMaterial = new Material(Shader.Find("Unlit/Color"));
            newMaterial.SetColor("_Color", color);
            pointer.GetComponent<MeshRenderer>().material = newMaterial;

            if (!isDefaultShowPointer)
            {
                pointer.SetActive(false);
            }
        }
        public virtual void OnPointerIn(PointerEventArgs e)
        {
            if (PointerIn != null)
                PointerIn(this, e);

            if (e.target.GetComponent<Button>() != null)
                pointer.GetComponent<MeshRenderer>().material.color = inColor;
        }

        public virtual void OnPointerClick(PointerEventArgs e)
        {
            if (PointerClick != null)
                PointerClick(this, e);
        }
        public virtual void OnPointerClickUp(PointerEventArgs e)
        {
            if (PointerClickUp != null)
                PointerClickUp(this, e);
        }
        public virtual void OnPointerOut(PointerEventArgs e)
        {
            if (PointerOut != null)
                PointerOut(this, e);
            pointer.GetComponent<MeshRenderer>().material.color = color;
        }


        private void Update()
        {
            if (!isDefaultActivePointer) return;

            if (activePointer.GetStateDown(pose.inputSource))
            {
                IsDefaultShowPointer = true;
            }
            if (activePointer.GetStateUp(pose.inputSource))
            {
                IsDefaultShowPointer = false;
            }
            if (!isDefaultShowPointer)
            {
                return;
            }

            float dist = 100f;

            Ray raycast = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            bool bHit = Physics.Raycast(raycast, out hit);

            if (previousContact && previousContact != hit.transform)
            {
                PointerOutFun();
            }
            if (bHit && previousContact != hit.transform)
            {
                PointerEventArgs argsIn = new PointerEventArgs();
                argsIn.fromInputSource = pose.inputSource;
                argsIn.distance = hit.distance;
                argsIn.flags = 0;
                argsIn.target = hit.transform;
                OnPointerIn(argsIn);
                previousContact = hit.transform;
            }
            if (!bHit)
            {
                previousContact = null;
            }
            if (bHit && hit.distance < 100f)
            {
                dist = hit.distance;
            }

            if (bHit && interactWithUI.GetStateDown(pose.inputSource))
            {
                PointerEventArgs argsClick = new PointerEventArgs();
                argsClick.fromInputSource = pose.inputSource;
                argsClick.distance = hit.distance;
                argsClick.flags = 0;
                argsClick.target = hit.transform;
                OnPointerClick(argsClick);
            }
            if (bHit && interactWithUI.GetStateUp(pose.inputSource))
            {
                PointerEventArgs argsClick = new PointerEventArgs();
                argsClick.fromInputSource = pose.inputSource;
                argsClick.distance = hit.distance;
                argsClick.flags = 0;
                argsClick.target = hit.transform;
                OnPointerClickUp(argsClick);
            }

            //if (interactWithUI != null && interactWithUI.GetState(pose.inputSource))
            //{
            //    pointer.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, dist);
            //    pointer.GetComponent<MeshRenderer>().material.color = clickColor;
            //}
            //else
            //{
            //    pointer.transform.localScale = new Vector3(thickness, thickness, dist);
            //    pointer.GetComponent<MeshRenderer>().material.color = color;
            //}
            pointer.transform.localScale = new Vector3(thickness, thickness, dist);
            pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);
        }
        void PointerOutFun()
        {
            PointerEventArgs args = new PointerEventArgs();
            args.fromInputSource = pose.inputSource;
            args.distance = 0f;
            args.flags = 0;
            args.target = previousContact;
            OnPointerOut(args);
            previousContact = null;
        }
    }
    public struct PointerEventArgs
    {
        public SteamVR_Input_Sources fromInputSource;
        public uint flags;
        public float distance;
        public Transform target;
    }

    public delegate void PointerEventHandler(object sender, PointerEventArgs e);
}