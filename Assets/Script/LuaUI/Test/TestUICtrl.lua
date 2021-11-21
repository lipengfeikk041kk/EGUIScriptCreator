
local this = {}
this.uiType = "normal"
this.assetModel = "Test"
this.assetName = "TestUI"
local panel = require("TestUIPanel")

this.Awake = function(gameObject)
    panel.Awake(gameObject)
    
    UIEventListener.Get(panel.testBtn.gameObject).onClick = function(go, eventData)
        this.TestBtnOnClick(go, eventData)
    end
    UIEventListener.Get(panel.testImg.gameObject).onDrag = function(go, pos, eventData)
        this.TestImgOnDrag(go, pos, eventData)
    end
    UIEventListener.Get(panel.testRimg.gameObject).onEnter = function(go)
        this.TestRimgOnEnter(go)
    end
end

this.OnShowPage = function()
    
end

this.OnClose = function()
    
end

this.OnDispose = function()
    
end


this.TestBtnOnClick = function(go, eventData)
    
end

this.TestImgOnDrag = function(go, pos, eventData)
    
end

this.TestRimgOnEnter = function(go)
    
end

return this
