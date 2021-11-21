# EGUIScriptCreator
一个Unity的UI脚本生成插件  
           

## 模板结构介绍

在插件ConfigPath路径中填写的目录会被当做模板路径，插件会将其下的每个文件夹作为一个配置好的模板处理。

其中每个文件夹的名称会作为插件SelectTemp中的供选项。

每个文件夹下应该包含ComponentSetting.json、EventSetting.json、PropertySetting.json以及ClassTemp文件夹


ComponentSett.json中componentName为组件名称对应关键字[propertyComp]，sort为组件优先级，sort越小，在插件自动选择默认组件时优先级越高，例如：在UGUI的Button下带有Text，则Text默认不被选中。

EventSetting.json中eventName为事件名称对应关键字[eventName]和[EventName]

PropertySetting.json比较好理解了，看下面的关键字说明部分吧。

## 模板关键字
### PropertySetting.json中的关键字

***[propertyName]***
：当前选择的物体的子物体名称

***[propertyComp]***
：当前选择的物体的子物体中被选择的组件名称

***[gameObjectPath]***
：当前选择的物体的子物体的路径，从当前选择的物体起

### EventSetting.json中的关键字
***[eventName]***
：当前选择的物体的子物体中被选择的事件名称，对应EventSetting.json中eventName字段

***[EventName]***
：同[eventName]，但会将首字母大写

***[PropertyName]***
：同[propertyName]，但会将首字母大写

### ClassTemp中的关键字，集合关键字必须单独占用一行
***[propertyDefine]***
：子物体定义集合

***[propertyFind]***
：子物体查找集合

***[eventAdd]***
：事件添加集合

***[eventFunction]***
：事件函数集合

***[modelName]***
：你填写的modelName中的值

***[gameObjectName]***
：当前选择的物体的名称

## 注意
当模板改变后，需要手动点击LoadConfig按钮

其他就参考示例吧。

QQ:377693703
