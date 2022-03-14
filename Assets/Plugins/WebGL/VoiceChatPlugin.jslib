// var webRTC = {
//     initWebRTC: function(){
//         window.initializeWebRTC();
//     },
    
//     create: function(){  
//         window.createOffer()
//         var str2 = document.getElementById('callInput').value;
//         var buffer2 = _malloc(lengthBytesUTF8(str2) + 1);
//         stringToUTF8(str2, buffer2, lengthBytesUTF8(str2) + 1);
//         return buffer2;
//     },
//     answer: function(name){
//         console.log("answer");
//         var s = Pointer_stringify(name);
//         console.log(s);
//         window.answerOffer(s);
//     },
//     answered: function(){
//         window.answeredCall();
//     }
// };
// mergeInto(LibraryManager.library, webRTC);

var voiceChat = {
    initializeVoiceChat: function(){
        window.initialize();
    },

    getIds : function(){
        var ids = window.returnIds();
        var buffer = _malloc(lengthBytesUTF8(ids) + 1);
        stringToUTF8(ids, buffer, lengthBytesUTF8(ids) + 1);
        return buffer;
    },

    makeCall: function(index, id){
        var s = Pointer_stringify(id);
        console.log("making call with index %d to id %s", index, s);
        window.createCall(index, s);
    }
};
mergeInto(LibraryManager.library, voiceChat);
