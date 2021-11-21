
local this = {}

this.Awake = function(gameObject) 
    this.testEmpty = gameObject.transform:Find("testEmpty")
    ---@type UnityEngine.UI.Text
    this.title = gameObject.transform:Find("testEmpty/title"):GetComponent("Text")
    ---@type UnityEngine.UI.Button
    this.testBtn = gameObject.transform:Find("testBtn"):GetComponent("Button")
    ---@type UnityEngine.UI.Toggle
    this.testToggle = gameObject.transform:Find("testToggle"):GetComponent("Toggle")
    ---@type UnityEngine.UI.Image
    this.testImg = gameObject.transform:Find("testImg"):GetComponent("Image")
    ---@type UnityEngine.UI.ScrollRect
    this.testScroll = gameObject.transform:Find("testScroll"):GetComponent("ScrollRect")
    ---@type UnityEngine.UI.RawImage
    this.testRimg = gameObject.transform:Find("testRimg"):GetComponent("RawImage")
end

return this
