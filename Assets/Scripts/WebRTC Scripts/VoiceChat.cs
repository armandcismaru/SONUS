/*using Unity.WebRTC;
using UnityEngine;
using System.Collections;

namespace UnityWebGLMicrophone
{
    public class VoiceChat : MonoBehaviour
    {
        [SerializeField] private AudioSource inputAudioSource;
        [SerializeField] private AudioSource outputAudioSource;
        [SerializeField] private AudioClip audioSample;

        private static RTCConfiguration configuration = new RTCConfiguration
        {
            iceServers = new[] { new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } } }
        };

        private RTCPeerConnection pc1Local, pc1Remote, pc2Local, pc2Remote;
        private MediaStream _sendStream;
        private MediaStream _receiveStream;
        private bool isCalled = false;

        private void Awake()
        {
            WebRTC.Initialize();
            //Microphone.Init();
            //Microphone.QueryAudioInput();
        }

        private void Start()
        {
            StartCoroutine(WebRTC.Update());           
            // Triggers functions based on user interaction: OnStart, OnCall, OnHangup
            
        }

        private void OnDestroy()
        {
            WebRTC.Dispose();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Alpha1))
                OnStart();
           
            if (Input.GetKeyDown(KeyCode.Alpha2))
                OnCall();
            if (Input.GetKeyDown(KeyCode.Alpha3))
                OnHangUp();
        }

        private void OnStart()
        {
            Debug.Log("Starting communication streams");
            _sendStream = new MediaStream();

            inputAudioSource.clip = audioSample;
            inputAudioSource.loop = true;
            inputAudioSource.Play();

            var audioTrack = new AudioStreamTrack(inputAudioSource);
            _sendStream.AddTrack(audioTrack);
        }

        private void OnCall()
        {
            Debug.Log("Starting peer calls...");

            //_receiveStream = new MediaStream();

            pc1Local = new RTCPeerConnection(ref configuration);
            pc1Remote = new RTCPeerConnection(ref configuration)
            {
                OnTrack = e =>
                {
                    if (e.Track is AudioStreamTrack audioTrack)
                    {
                        outputAudioSource.SetTrack(audioTrack);
                        outputAudioSource.loop = true;
                        outputAudioSource.Play();
                    }
                }
            };

            pc1Local.OnIceCandidate = candidate => pc1Remote.AddIceCandidate(candidate);
            pc1Remote.OnIceCandidate = candidate => pc1Local.AddIceCandidate(candidate);
            Debug.Log("pc1: created local and remote peer connection object");

            foreach (var track in _sendStream.GetTracks())
            {
                pc1Local.AddTrack(track, _sendStream);
                //pc2Local.AddTrack(track, _sendStream);
            }

            Debug.Log("Adding local stream to pc1Local/pc2Local");

            StartCoroutine(NegotiationPeer(pc1Local, pc1Remote));
            //StartCoroutine(NegotiationPeer(pc2Local, pc2Remote));
        }

        private void OnHangUp()
        {
            Debug.Log("Hanging up stream...");

            foreach (var track in _sendStream.GetTracks())
            {
                track.Dispose();
            }
            _sendStream.Dispose();
            _sendStream = null;
            pc1Local.Close();
            pc1Remote.Close();
            pc1Local.Dispose();
            pc1Remote.Dispose();
            pc1Local = null;
            pc1Remote = null;

            inputAudioSource.Stop();
            inputAudioSource.clip = null;
            outputAudioSource.Stop();
            outputAudioSource.clip = null;
        }

        private static void OnCreateSessionDescriptionError(RTCError error)
        {
            Debug.LogError($"Failed to create session description: {error.message}");
        }

        private static IEnumerator NegotiationPeer(RTCPeerConnection localPeer, RTCPeerConnection remotePeer)
        {
            var opCreateOffer = localPeer.CreateOffer();
            yield return opCreateOffer;

            if (opCreateOffer.IsError)
            {
                OnCreateSessionDescriptionError(opCreateOffer.Error);
                yield break;
            }

            var offerDesc = opCreateOffer.Desc;
            yield return localPeer.SetLocalDescription(ref offerDesc);
            Debug.Log($"Offer from LocalPeer \n {offerDesc.sdp}");
            yield return remotePeer.SetRemoteDescription(ref offerDesc);

            var opCreateAnswer = remotePeer.CreateAnswer();
            yield return opCreateAnswer;

            if (opCreateAnswer.IsError)
            {
                OnCreateSessionDescriptionError(opCreateAnswer.Error);
                yield break;
            }

            var answerDesc = opCreateAnswer.Desc;
            yield return remotePeer.SetLocalDescription(ref answerDesc);
            Debug.Log($"Answer from RemotePeer \n {answerDesc.sdp}");
            yield return localPeer.SetRemoteDescription(ref answerDesc);
        }
    }
}*/