# Screen Monitor

Screen Monitor is a simple utility that performs three primary actions:

1. Captures the screen and saves it as a PNG on an hourly basis.

```
ScreenMonitor.exe capture
```

2. Compiles the series of PNG screenshots from the previous hour into an MP4 file.

```
ScreenMonitor.exe compile
```

3. Clean up any folders that are older than one day.

```
ScreenMonitor.exe clean
```

## Task Scheduler

Use Windows Task Scheduler to automatically run these commands at your preferred intervals. Simply schedule each command (capture, compile, and clean) according to how frequently you want them to run.
