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
    },

    setPosition: function(index, x, y, z){
        window.setPlayerPosition(index, x, y, z);
    },

    setMyOrientation: function(x, y, z){
        window.setOrientation(x, y, z);
    },

    mutePlayer: function(index){
        window.muteIndex(index);
    },

    unmutePlayer: function(index){
        window.unmuteIndex(index);
    },
};
mergeInto(LibraryManager.library, voiceChat);
