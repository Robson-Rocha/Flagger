# Flagger

Small and simple library to keep track of temporary flags

| Class | Description |
| ----- | ----- |
| [Flag](#flag) | Contains several methods which allows to configure contextual flags |
| [Flag\<T\>](#flag-t) | Allows for setting a value to the supplied member while the context lasts, and resetting the value on the context disposal |


## Flag

Contains several methods which allows to configure contextual flags


### Flag.Toggle(member)

Toggles (inverts) the value of the supplied boolean member

| Parameter Name | Description |
| -------------- | ----------- |
| member | *[Expression](https://docs.microsoft.com/dotnet/api/expression-1)\<[Func](https://docs.microsoft.com/dotnet/api/func-1)\<[Boolean](https://docs.microsoft.com/dotnet/api/boolean)\>\>*<br>Lambda expression which indicates the boolean member to be toggled. |

#### Returns

*[Flag\<Boolean\>](#flag-t)*<br>
Flag context which toggles the boolean member




### Flag.SetAndReset\<T\>(member, setValue)

Sets the value of the supplied boolean member to the provided value, and reset it to its original value on the context disposal

| Parameter Name | Description |
| -------------- | ----------- |
| member | *[Expression](https://docs.microsoft.com/dotnet/api/expression-1)\<[Func](https://docs.microsoft.com/dotnet/api/func-1)\<T\>\>*<br>Lambda expression which indicates the member to be set. |
| setValue | *T*<br>The value to be set at the context creation |

#### Returns

*[Flag\<T\>](#flag-t)*<br>
Flag context which sets and resets the member value



### Flag.SetAndUnset(member)

Sets the value of the supplied boolean member to true on context creation, and set it to false on the context disposal

| Parameter Name | Description |
| -------------- | ----------- |
| member | *[Expression](https://docs.microsoft.com/dotnet/api/expression-1)\<[Func](https://docs.microsoft.com/dotnet/api/func-1)\<[Boolean](https://docs.microsoft.com/dotnet/api/boolean)\>\>*<br>Lambda expression which indicates the boolean member to be toggled. |

#### Returns

*[Flag\<Boolean\>](#flag-t)*<br>
Flag context which sets and unsets the member value

### Flag.SetAndUnset\<T\>(member, setValue, unsetValue)

Sets the value of the supplied member to the provided set value, and set it to the provided unset value on the context disposal

| Parameter Name | Description |
| -------------- | ----------- |
| member | *[Expression](https://docs.microsoft.com/dotnet/api/expression-1)\<[Func](https://docs.microsoft.com/dotnet/api/func-1)\<T\>\>*<br>Lambda expression which indicates the member to be set. |
| setValue | *\<T\>*<br>The value to be set at the context creation |
| unsetValue | *\<T\>*<br>The value to be set at the contex disposal |

#### Returns

*[Flag\<T\>](#flag-t)*<br>
Flag context which sets and unsets the member value

### Flag.SetAndUnsetIfUnset(member)

If the current value of the supplied boolean member is false, set it to true on context creation, and set it to false on the context disposal. Otherwise, keeps it unchanged;

| Parameter Name | Description |
| -------------- | ----------- |
| member | *[Expression](https://docs.microsoft.com/dotnet/api/expression-1)\<[Func](https://docs.microsoft.com/dotnet/api/func-1)\<[Boolean](https://docs.microsoft.com/dotnet/api/boolean)\>\>*<br>Lambda expression which indicates the boolean member to be toggled. |

#### Returns

*[Flag\<Boolean\>](#flag-t)*<br>
Flag context which sets and unsets the member value



## Flag \<T\>

Allows for setting a value to the supplied member while the context lasts, and resetting the value on the context disposal

#### Type Parameters

- T - The type of the member to be set and reset

### *constructor* Flag(member, setValue, unsetValue)

Creates a Flag context, assigning the setValue to the supplied member, and storing the unsetValue for reseting the member on the context disposal

| Parameter Name | Description |
| -------------- | ----------- |
| member | [MemberExpression](https://docs.microsoft.com/dotnet/api/system.linq.expressions.memberexpression)<br>MemberExpression representing the member to be set and unset. |
| setValue | *\<T\>*<br>The value to be set at the context creation |
| unsetValue | *\<T\>*<br>The value to be set at the contex disposal |


### Dispose

Disposes of the flag context, resetting the supplied member value
