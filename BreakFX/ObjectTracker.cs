using GameManagement;
using ReplayEditor;
using System.Collections.Generic;
using UnityEngine;

namespace BreakFX
{
    public class RecordedFrame
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public float Time;

        public RecordedFrame(Vector3 position, Quaternion rotation, float time)
        {
            Position = position;
            Rotation = rotation;
            Time = time;
        }
    }

    public class ObjectTracker : MonoBehaviour
    {
        private float nextRecordTime;
        private List<RecordedFrame> recordedFrames;
        private Rigidbody rigidBody;
        private BoxCollider collider;
        private int BufferFrameCount;
        private Animation anim;
        private AnimationClip clip;
        private bool clipUpdated;
        private Vector3 lastPosition;
        private Quaternion lastRotation;

        private void Awake()
        {
            recordedFrames = new List<RecordedFrame>();
            rigidBody = GetComponent<Rigidbody>();
            collider = GetComponent<BoxCollider>();
            BufferFrameCount = Mathf.RoundToInt(ReplaySettings.Instance.FPS * ReplaySettings.Instance.MaxRecordedTime);
        }

        private void RecordFrame()
        {
            if (nextRecordTime > PlayTime.time)
            {
                return;
            }
            if (nextRecordTime < PlayTime.time - 1f)
            {
                nextRecordTime = PlayTime.time + 1f / 30f;
            }
            else
            {
                nextRecordTime += 1f / 30f;
            }

            RecordedFrame tempRecordedFrame;
            if (recordedFrames.Count >= BufferFrameCount)
            {
                tempRecordedFrame = recordedFrames[0];
                recordedFrames.RemoveAt(0);
                tempRecordedFrame.Time = PlayTime.time;
            }
            else
            {
                tempRecordedFrame = new RecordedFrame(transform.localPosition, transform.localRotation, PlayTime.time);
            }

            tempRecordedFrame.Position = transform.localPosition;
            tempRecordedFrame.Rotation = transform.localRotation;
            tempRecordedFrame.Time = PlayTime.time;
            recordedFrames.Add(tempRecordedFrame);
        }

        private void Update()
        {
            if (GameStateMachine.Instance.CurrentState.GetType() == typeof(PlayState))
            {
                RecordFrame();
            }
            if (GameStateMachine.Instance.CurrentState.GetType() == typeof(ReplayState))
            {
                if (!rigidBody.isKinematic)
                {
                    lastPosition = transform.localPosition;
                    lastRotation = transform.localRotation;
                    rigidBody.isKinematic = true;
                    collider.isTrigger = true;
                    BreakFXController.Instance.needReset = true;
                    BreakFXController.Instance.wasInReplay = true;
                }

                anim = gameObject.GetComponent<Animation>();

                if (anim == null)
                {
                    anim = gameObject.AddComponent<Animation>();
                }

                if (!clip || !clipUpdated)
                {
                    clip = new AnimationClip();
                    clip.legacy = true;
                    clip.name = $"{gameObject.name}";

                    AnimationCurve curve_pos_x = new AnimationCurve();
                    AnimationCurve curve_pos_y = new AnimationCurve();
                    AnimationCurve curve_pos_z = new AnimationCurve();
                    AnimationCurve curve_rot_x = new AnimationCurve();
                    AnimationCurve curve_rot_y = new AnimationCurve();
                    AnimationCurve curve_rot_z = new AnimationCurve();
                    AnimationCurve curve_rot_w = new AnimationCurve();

                    foreach (RecordedFrame frame in recordedFrames)
                    {
                        curve_pos_x.AddKey(frame.Time, frame.Position.x);
                        curve_pos_y.AddKey(frame.Time, frame.Position.y);
                        curve_pos_z.AddKey(frame.Time, frame.Position.z);
                        curve_rot_x.AddKey(frame.Time, frame.Rotation.x);
                        curve_rot_y.AddKey(frame.Time, frame.Rotation.y);
                        curve_rot_z.AddKey(frame.Time, frame.Rotation.z);
                        curve_rot_w.AddKey(frame.Time, frame.Rotation.w);
                    }

                    clip.SetCurve("", typeof(Transform), "localPosition.x", curve_pos_x);
                    clip.SetCurve("", typeof(Transform), "localPosition.y", curve_pos_y);
                    clip.SetCurve("", typeof(Transform), "localPosition.z", curve_pos_z);

                    clip.SetCurve("", typeof(Transform), "localRotation.x", curve_rot_x);
                    clip.SetCurve("", typeof(Transform), "localRotation.y", curve_rot_y);
                    clip.SetCurve("", typeof(Transform), "localRotation.z", curve_rot_z);
                    clip.SetCurve("", typeof(Transform), "localRotation.w", curve_rot_w);
                    clipUpdated = true;
                }

                anim.AddClip(clip, clip.name);
                anim.animatePhysics = true;

                var state = anim[clip.name];

                if (!anim.isPlaying && ReplayEditorController.Instance.playbackController.TimeScale != 0.0)
                {
                    anim.Play(clip.name);
                }

                state.time = ReplayEditorController.Instance.playbackController.CurrentTime;
                state.speed = ReplayEditorController.Instance.playbackController.TimeScale;

                if (BreakFXController.Instance.boardBroken && BreakFXController.Instance.bbPrefab != null)
                {
                    if (ReplayEditorController.Instance.playbackController.GetCurrentFrame().time <= recordedFrames[0].Time)
                    {
                        BreakFXController.Instance.frontTrans.GetComponent<Renderer>().enabled = false;
                        BreakFXController.Instance.backTrans.GetComponent<Renderer>().enabled = false;
                        BreakFXController.Instance.UnhideReplayBoard();
                    }
                    else
                    {
                        BreakFXController.Instance.frontTrans.GetComponent<Renderer>().enabled = true;
                        BreakFXController.Instance.backTrans.GetComponent<Renderer>().enabled = true;
                        BreakFXController.Instance.HideReplayBoard();
                    }
                }
            }
        }
    }
}
