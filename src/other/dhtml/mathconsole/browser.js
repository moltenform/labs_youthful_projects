//Found online
function XBrowserAddHandler(target,eventName,handlerName)
{
    if ( target.addEventListener )
      target.addEventListener(eventName, handlerName, false);
    else if ( target.attachEvent )
      target.attachEvent("on" + eventName, handlerName);
    else
      target["on" + eventName] = handlerName;
}

