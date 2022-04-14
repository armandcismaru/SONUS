// These will be initialized later
var recognizer, recorder, callbackManager, audioContext, outputContainer;
// Only when both recorder and recognizer do we have a ready application
var isRecorderReady = isRecognizerReady = false;

// A convenience function to post a message to the recognizer and associate
// a callback to its response
function postRecognizerJob(message, callback) {
  var msg = message || {};
  if (callbackManager) msg.callbackId = callbackManager.add(callback);
  if (recognizer) recognizer.postMessage(msg);
};

// This function initializes an instance of the recorder
// it posts a message right away and calls onReady when it
// is ready so that onmessage can be properly set
function spawnWorker(workerURL, onReady) {
    recognizer = new Worker(workerURL);
    recognizer.onmessage = function(event) {
      onReady(recognizer);
    };
    // As arguments, you can pass non-default path to pocketsphinx.js and pocketsphinx.wasm:
    // recognizer.postMessage({'pocketsphinx.wasm': '/path/to/pocketsphinx.wasm', 'pocketsphinx.js': '/path/to/pocketsphinx.js'});
    recognizer.postMessage({});
};

// To display the hypothesis sent by the recognizer
// function updateHyp(hyp) {
//   if (outputContainer) outputContainer.innerHTML = hyp;
// };

var newWords = 0;
function updateHypSeg(hypseg) {
  //console.log(hypseg);
  //if (hypseg.length > 0 && hypseg[hypseg.length - 1].word.localeCompare("<sil>") != 0) window.unityInstance.SendMessage("BridgeVoiceRecognition", "TriggerSpell", hypseg[hypseg.length - 1].word);
  if (newWords < hypseg.length) {
    newWords++;
    window.unityInstance.SendMessage("BridgeVoiceRecognition", "TriggerSpell", hypseg[0].word);
  }
}

// This updates the UI when the app might get ready
// Only when both recorder and recognizer are ready do we enable the buttons
// function updateUI() {
//   if (isRecorderReady && isRecognizerReady) startBtn.disabled = stopBtn.disabled = false;
// };

// This is just a logging window where we display the status
function updateStatus(newStatus) {
  // console.log("Status: " + newStatus);
};

// A not-so-great recording indicator
function displayRecording(display) {
  // if (display) console.log("Recording Started");
  // else console.log("Recording Stopped");
  // if (display) document.getElementById('recording-indicator').innerHTML = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
  // else document.getElementById('recording-indicator').innerHTML = "";
};

// Callback function once the user authorises access to the microphone
// in it, we instanciate the recorder
function startUserMedia(stream) {
  var input = audioContext.createMediaStreamSource(stream);
  // Firefox hack https://support.mozilla.org/en-US/questions/984179
  window.firefox_audio_hack = input; 
  var audioRecorderConfig = {errorCallback: function(x) {updateStatus("Error from recorder: " + x);}};
  recorder = new AudioRecorder(input, audioRecorderConfig);
  // If a recognizer is ready, we pass it to the recorder
  if (recognizer) recorder.consumers = [recognizer];
  isRecorderReady = true;
  // updateUI();
  updateStatus("Audio recorder ready");
};

// This starts recording. We first need to get the id of the grammar to use
//var startRecording = function() {
function startRecording() {
  // var id = document.getElementById('grammars').value;
  if (recorder && recorder.start(0)) displayRecording(true);
};

// Stops recording
//var stopRecording = function() {
function stopRecording() {
  recorder && recorder.stop();
  displayRecording(false);
};

// Called once the recognizer is ready
// We then add the grammars to the input select tag and update the UI
var recognizerReady = function() {
    //  updateGrammars();
     isRecognizerReady = true;
    //  updateUI();
     updateStatus("Recognizer ready");
};

// We get the grammars defined below and fill in the input select tag
// var updateGrammars = function() {
//   var selectTag = document.getElementById('grammars');
//   for (var i = 0 ; i < grammarIds.length ; i++) {
//       var newElt = document.createElement('option');
//       newElt.value=grammarIds[i].id;
//       newElt.innerHTML = grammarIds[i].title;
//       selectTag.appendChild(newElt);
//   }                          
// };

// This adds a grammar from the grammars array
// We add them one by one and call it again as
// a callback.
// Once we are done adding all grammars, we can call
// recognizerReady()
var feedGrammar = function(g, index, id) {
  if (id && (grammarIds.length > 0)) grammarIds[0].id = id.id;
  if (index < g.length) {
    grammarIds.unshift({title: g[index].title});
    postRecognizerJob({command: 'addGrammar', data: g[index].g},
                        function(id) {feedGrammar(grammars, index + 1, {id:id});});
  } else {
    // We are adding keyword spotting which has id 0
    grammarIds.push({"id":0, "title": "Keyword spotting"});
    recognizerReady();
  }
};

// This adds words to the recognizer. When it calls back, we add grammars
var feedWords = function(words) {
     postRecognizerJob({command: 'addWords', data: words},
                  function() {feedGrammar(grammars, 0);});
};

// This initializes the recognizer. When it calls back, we add words
var initRecognizer = function() {
    // You can pass parameters to the recognizer, such as : {command: 'initialize', data: [["-hmm", "my_model"], ["-fwdflat", "no"]]}
    postRecognizerJob({command: 'initialize', data: [["-kws_threshold", "1e-30"], ["-kws", "kws.txt"], ["-dict","kws.dict"]]},
                      function() {console.log("Speech Recognition Initialized");});
};

// When the page is loaded, we spawn a new recognizer worker and call getUserMedia to
// request access to the microphone
window.onload = function() {
  // outputContainer = document.getElementById("output");
  updateStatus("Initializing web audio and speech recognizer, waiting for approval to access the microphone");
  callbackManager = new CallbackManager();
  spawnWorker("js/recognizer.js", function(worker) {
      // This is the onmessage function, once the worker is fully loaded
      worker.onmessage = function(e) {
          // This is the case when we have a callback id to be called
          if (e.data.hasOwnProperty('id')) {
            var clb = callbackManager.get(e.data['id']);
            var data = {};
            if ( e.data.hasOwnProperty('data')) data = e.data.data;
            if(clb) clb(data);
          }
          // This is a case when the recognizer has a new hypothesis
          if (e.data.hasOwnProperty('hyp')) {
            // var newHyp = e.data.hyp;
            // if (e.data.hasOwnProperty('final') &&  e.data.final) newHyp = "Final: " + newHyp;
            // updateHyp(newHyp, newHypSeg);
            if (e.data.hasOwnProperty('hypseg')) {
              var newHypSeg = e.data.hypseg;
              updateHypSeg(newHypSeg);
            }
          }
          
          // This is the case when we have an error
          if (e.data.hasOwnProperty('status') && (e.data.status == "error")) {
            updateStatus("Error in " + e.data.command + " with code " + e.data.code);
          }
      };
      // Once the worker is fully loaded, we can call the initialize function
      // but before that we lazy-load two files for keyword spoting (key phrase
      // file plus associated dictionary.
      postRecognizerJob({command: 'lazyLoad',
                         data: {folders: [], files: [["/", "kws.txt", "../kws.txt"],
                                                     ["/", "kws.dict", "../kws.dict"]]}
                        }, initRecognizer);
  });

  // The following is to initialize Web Audio
  try {
    window.AudioContext = window.AudioContext || window.webkitAudioContext;
    window.URL = window.URL || window.webkitURL;
    audioContext = new AudioContext();
  } catch (e) {
    updateStatus("Error initializing Web Audio browser");
  }
  if (navigator.mediaDevices.getUserMedia) navigator.mediaDevices.getUserMedia({audio: true}).then(startUserMedia).catch(function(e) {
                                  updateStatus("No live audio input in this browser");
                              });
  else updateStatus("No web audio support in this browser");

// Wiring JavaScript to the UI
// var startBtn = document.getElementById('startBtn');
// var stopBtn = document.getElementById('stopBtn');

var recordingOn = false;
document.addEventListener("keydown", function(event) {
  if (event.key == "e" && recordingOn == false) {
    recordingOn = true;
    newWords = 0;
    startRecording();
  }
});
document.addEventListener("keyup", function(event) {
  if (event.key == "e" && recordingOn == true) {
    recordingOn = false;
    stopRecording();
  }
});

// startBtn.disabled = true;
// stopBtn.disabled = true;
//startBtn.onclick = startRecording;
//stopBtn.onclick = stopRecording;
};

 // This is the list of words that need to be added to the recognizer
 // This follows the CMU dictionary format
//var wordList = [["ACTION", "AE K SH AH N"], ["WAREHOUSES", "W EH R HH AW Z IH Z"]];
// This grammar recognizes digits
//var grammarDigits = {numStates: 1, start: 0, end: 0, transitions: [{from: 0, to: 0, word: "ACTION"}, {from: 0, to: 0, word: "WAREHOUSES"}]};
// This grammar recognizes a few cities names
// var grammarCities = {numStates: 1, start: 0, end: 0, transitions: [{from: 0, to: 0, word: "NEW-YORK"}, {from: 0, to: 0, word: "NEW-YORK-CITY"}, {from: 0, to: 0, word: "PARIS"}, {from: 0, to: 0, word: "SHANGHAI"}, {from: 0, to: 0, word: "SAN-FRANCISCO"}, {from: 0, to: 0, word: "LONDON"}, {from: 0, to: 0, word: "BERLIN"}]};
// // This is to play with beloved or belated OSes
// var grammarOses = {numStates: 7, start: 0, end: 6, transitions: [{from: 0, to: 1, word: "WINDOWS"}, {from: 0, to: 1, word: "LINUX"}, {from: 0, to: 1, word: "UNIX"}, {from: 1, to: 2, word: "IS"}, {from: 2, to: 2, word: "NOT"}, {from: 2, to: 6, word: "GOOD"}, {from: 2, to: 6, word: "GREAT"}, {from: 1, to: 6, word: "ROCKS"}, {from: 1, to: 6, word: "SUCKS"}, {from: 0, to: 4, word: "MAC"}, {from: 4, to: 5, word: "O"}, {from: 5, to: 3, word: "S"}, {from: 3, to: 1, word: "X"}, {from: 6, to: 0, word: "AND"}]};
var grammars = [];
var grammarIds = [];