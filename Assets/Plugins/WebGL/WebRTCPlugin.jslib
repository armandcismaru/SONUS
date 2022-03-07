var webRCT = {
    initWebRTC: function(){
        window.initializeWebRTC();
    },
    create: function(){
        return window.createOffer();
    },
    answer: function(name){
        window.answerOffer(name);
    },
    answered: function(){
        window.answeredCall();
    }
};
mergeInto(LibraryManager.library, webRCT);
