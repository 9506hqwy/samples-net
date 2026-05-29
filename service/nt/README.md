# NT Service

Create NT service.

```sh
sc create "Sample .Net Service" binPath= "C:\path\to\Service.exe"
```

```text
[SC] CreateService SUCCESS
```

Start NT service.

```sh
sc start "Sample .Net Service"
```

```text
SERVICE_NAME: Sample .Net Service
        TYPE               : 10  WIN32_OWN_PROCESS
        STATE              : 2  START_PENDING
                                (NOT_STOPPABLE, NOT_PAUSABLE, IGNORES_SHUTDOWN)
        WIN32_EXIT_CODE    : 0  (0x0)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x7d0
        PID                : 7452
        FLAGS              :
```

Stop NT service.

```sh
sc stop "Sample .Net Service"
```

```text
SERVICE_NAME: Sample .Net Service
        TYPE               : 10  WIN32_OWN_PROCESS
        STATE              : 3  STOP_PENDING
                                (STOPPABLE, NOT_PAUSABLE, ACCEPTS_SHUTDOWN)
        WIN32_EXIT_CODE    : 0  (0x0)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x0
```

Delete NT service.

```sh
sc delete "Sample .Net Service"
```

```text
[SC] DeleteService SUCCESS
```
