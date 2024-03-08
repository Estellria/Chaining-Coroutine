# Chaining Coroutine

 매번 코루틴 호출을 위해 부가적인 함수를 만들 필요 없이 일반 객체를 생성함으로써 코루틴을 사용할 수 있습니다.


```C#
new ChainingCoroutine()
    .Bind(x => Debug.Log("Start"))
    .Bind(x => Debug.Log("End"))
    .Play();
```

<br>

Play 함수로 시작과 동시에 코루틴을 받아올 수 있습니다.

```C#
var cc = new ChainingCoroutine()
    .Bind(x => Debug.Log("Start"));

Coroutine coroutine = cc.Play();
```

<br>

# 기본적인 사용법

일반 객체를 만들듯 생성할 수 있습니다.


```C#
var cc = new ChainingCoroutine();
```


## 루틴 등록

```C#
cc.Bind(x => Debug.Log("binding"));
```


매개변수와 함수를 제공합니다.

```C#
cc.Bind(x => Debug.Log($"{x.elapsedTime} seconds, call count : {x.CalledCount}"));
```

|name|사용방법|
|:---|:---|
|elapsedTime|실행된 이후 경괴된 시간|
|calledCount|대기 루틴을 제외한 호출 횟수|

<br>

## 대기

Wait 함수로 대기 루틴을 등록할 수 있으며 전달된 객체는 내부에서 캐싱돼어 사용됩니다.

```C#
new ChainingCoroutine()
    .Bind(x => Debug.Log("Start waiting"))
    .Wait(new WaitForSeconds(3f))
    .Bind(x => Debug.Log("end"))
    .Play();
```

<br>

### 다른 코루틴 대기

다른 코루틴이 끝날때까지 대기합니다.

```C#
var cc1 = new ChainingCoroutine();
var cc2 = new ChainingCoroutine();

var cc1Co = cc1.Bind(x => print("start cc1"))
                .Wait(new WaitForSeconds(3f))
                .Bind(x => print("finish cc1"))
                .Play();

cc2.Bind(x => print("end cc2"))
    .WaitFor(cc1Co)
    .Bind(x => print("end cc2"))
    .Play();
```

<br>

### 조건 대기

두 번째 매개변수로 전달한 Bool 타입의 인자가 참일 경우에만 대기합니다.

```C#
new ChainingCoroutine()
      .Bind(x => print($"start. {x.elapsedTime} seconds"))
      .WaitIf(new WaitForSeconds(3f), x => true)
      .Bind(x => print($"end. {x.elapsedTime} seconds"))
      .Play()
```

<br>

### 달성 대기

해당 조건이 달성될때까지 대기합니다.

```C#
new ChainingCoroutine()
    .Bind(x => print("Click the screen with mouse"))
    .BeginLoop(x => x.To(0, 5))
        .WaitFor(c => Input.GetMouseButtonDown(0))
        .Bind(x => print($"click complete"))
    .EndLoop()
    .Bind(x => print(x.elapsedTime))
    .Play();
```

## 반복

BeginLoop 함수와 EndLoop 함수 사이의 루틴들을 BeginLoop 함수의 조건이 참일때 반복 수행됩니다.  

이때 각 루프들은 자동으로 yield return null을 수행하기 때문에 한 프레임을 쉬며 제어권을 반환합니다.

```C#
int index = 0;
new ChainingCoroutine()
    .BeginLoop(x => index < 10)
        .Bind(x => print($"{index++} call"))
        .Wait(new WaitForSeconds(0.5f))
    .EndLoop()
    .Bind(x => print(x.elapsedTime))
    .Play();
```

<br>

루프를 중첩해서 사용할 수 있습니다.

```C#
new ChainingCoroutine()
    .BeginLoop(x => x.To(0, 2))
        .Bind(x => print($"{color1} {++countA} loop 1"))
        .BeginLoop(x => x.To(0, 2, true))
            .Bind(x => print($"{color2} {++countB} loop 2"))
        .EndLoop()
    .EndLoop()
    .Bind(x => print(x.elapsedTime))
    .Play();
```
