 using UnityEngine;

 public static  class CoreUtils
    {
        public static bool IsNull<T>(this  T component)
        {
            return component == null;
        }

        public static bool IsDivisible(this int test, int divisible)
        {
            bool isDivisble = test % divisible == 0;
            return isDivisble;
        }

        public static void ResetRigidbody(this Rigidbody rigidbody)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.inertiaTensorRotation = Quaternion.identity;
            rigidbody.inertiaTensor = Vector3.zero;
            rigidbody.centerOfMass = Vector3.zero;
        }

        public static void PickUpThrowable(this Rigidbody rigidbody, Transform startPos, Transform parent)
        {
            rigidbody.ResetRigidbody();
            rigidbody.isKinematic = true;
            rigidbody.transform.position = startPos.transform.position;
            rigidbody.transform.SetParent(parent);
            rigidbody.gameObject.SetActive(true);
        }
        
        public static  void AttachThrowable(  this Rigidbody rigidbody,Transform parent, Transform stonepivotPos)
        {
            rigidbody.ResetRigidbody();
            rigidbody.isKinematic = true;
            rigidbody.transform.SetParent(parent);
            rigidbody.transform.position = stonepivotPos.position;
            rigidbody.transform.rotation = stonepivotPos.rotation;
            rigidbody.transform.gameObject.SetActive(true);
        }
        
        public static void ThrowObject( this Rigidbody rigidbody,Transform parent, Transform destinationPos, float force)
        {
            rigidbody.transform.SetParent(parent);
            rigidbody.isKinematic = false;
            Vector3 relativePos = destinationPos.position - rigidbody.transform.position;
            rigidbody.AddForce(force * relativePos);
        }

        public static void PlayAudioClip(this AudioSource audioSource, AudioClip audioClip)
        {
            audioSource.Stop();
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        


    }