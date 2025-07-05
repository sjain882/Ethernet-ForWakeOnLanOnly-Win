# Only use (Powerline) Ethernet as a Wake-On-LAN (WoL) provider - not as an internet connection

*No support provided*

### Situation

- System has a strong WiFi connection with high speeds
- The WiFi card is in a PCIe slot and is powered down on S3 sleep
- The BIOS offers no option to change this behaviour (e.g, HP/Dell proprietary)
- Therefore, the WiFi connection is lost on sleep so WoL is not possible
***
- Due to physical limitations, direct Ethernet cannot be provided
- Only a weak Powerline Ethernet connection can be provided with slow speeds
- WoL, however, is fully functional with this connection
***
- Windows heavily prioritises any Ethernet/wired connection over WiFi/wireless
- "Interface metric" workaround has not been respected for many years now
- Therefore, we are stuck with slow Powerline Ethernet speeds
- Only solution is to disable Powerline Ethernet on wake, and re-enable just it before sleep (this repo)

> [!IMPORTANT]
> Admin rights required!
> 
> You may experience issues if logged into a non-administrator account.

### How to execute tasks on suspend & resume?
#### Option 1: Task Scheduler
- You can setup a task to trigger on the following events:
  - Log: `System`
  - Source: `Kernel-Power`
  - Event ID: `42` for suspend (EnableEthernetBeforeSleep)
  - Event ID: `107` for resume (DisableEthernetAfterWakeup)
- This, however, is **unreliable** on some systems.
    - I was unable to run the tasks manually for testing purposes.
    - Others reported the triggers never firing.

#### Option 2: win7suspendresume / Power Triggers v2 
- Special program that allows scheduling & logging of tasks on suspend/resume
- Runs a background service that spawns scheduled tasks on a higher priority thread, meaning they're more likely to complete before sleep.
- **We will be using this program.**
- It appears to have some UAC bypass method to make the experience more seamless, but I haven't tested this on non-admin accounts (you might have to manually start the program as administrator on each login in these cases)
- It was previously available on Codeplex before the shutdown - you can view it in a friendly format [here](https://codeplexarchive.org/project/win7suspendresume).

### Pre-requisite: Install win7suspendresume / Power Triggers v2

**Alternative** to Steps 1-5 below: download a pre-made zip [here](https://github.com/sjain882/Ethernet-ForWakeOnLanOnly-Win/raw/refs/heads/main/Other/Win7SuspendResume%20Power%20Triggers%20v1.01.zip) directly in this repo. Otherwise, follow the below steps if you only trust the original source.

1. Download `win7suspendresume.zip` from [here](https://archive.org/download/sylirana_ms_codeplex_zips/tars/mscodeplex-w-2.tar/.%2Fwin7suspendresume.zip) (sourced from the [Codeplex Archive](https://ia903400.us.archive.org/view_archive.php?archive=/12/items/sylirana_ms_codeplex_zips/tars/mscodeplex-w-2.tar) on archive.org) or [here](https://github.com/sjain882/Ethernet-ForWakeOnLanOnly-Win/raw/refs/heads/main/Other/win7suspendresume.zip) (copy stored in this repo)
3. Extract the contents somewhere
4. Navigate to `releases\3`
5. Extract the `49d2ea10-7a2a-4265-81b0-0d8f3eb00d24` file somewhere. This is release v1.01 of Power Triggers v2 - other versions [reportedly](https://superuser.com/a/1272661) don't allow scheduling of custom programs.
6. Add the `.zip` extension (so its `49d2ea10-7a2a-4265-81b0-0d8f3eb00d24.zip`)
7. Extract the contents to somewhere suitable, e.g, `C:\Program Files\Power Triggers v2`
8. Run the program - it should automatically add itself to startup (visible in Task Manager startup tab).
9. NOTE: It appears to have some UAC bypass method to make the experience more seamless, but I haven't tested this on non-admin accounts (you might have to manually start the program as administrator on each login in these cases)

### How to use

1. Build `DisableEthernetAfterWakeup` and `EnableEthernetBeforeSleep` in Release | Any CPU (Visual Studio 2022)

2. Move to appropriate path like `C:\Program Files\WoL`

3. Open Powershell as administrator and maximise the window

4. Enter `Get-NetAdapter | Format-Table -AutoSize`

5. Note down your WoL Adapter name (in most cases this should just be `Ethernet`)

6. For each program, open the corresponding `.exe.config` file and ensure the value of `AdapterName` matches the name you noted down in Step 5.

7. Open Power Triggers v2 from the notification tray and click `...` on Suspendtasks

8. Click `Start Process` tab

9. Click `Other...`

10. Click `...` on File row

11. Find `EnableEthernetBeforeSleep`

12. Change `Action` to `Wait for Exit` (NOTE: this just means the task's execution will only be logged when execution completes - it doesn't stop the PC from sleeping until execution completes)

13. Repeat Steps 7 to 12 but for `Resumetasks` and select `DisableEthernetAfterWakeup` instead.

14. Optionally click `Test`.

15. Click `OK` on everything and sleep the PC then try to use WoL (via e.g, ASUS router UI or [Simple WoL App](https://github.com/herzhenr/simple-wake-on-lan))

16. Done.

## Other approaches I considered

### Pause sleep with wakelock, wait for connection, then disable wakelock

Code snippet [here](https://gist.github.com/brianhassel/e918c7b9f1a6265ff8f9).

Order of execution:

1. Sleep button pressed (system/physical PC case)

2. Power Triggers v2 fires suspend event

3. C# program starts and immediately sets `SetThreadExecutionState(ExecutionState.EsContinuous | ExecutionState.EsSystemRequired)`, hopefully interrupting sleep

4. It then enables WoL adapter and loops until it determines connection successful

5. Only then does it remove the wakelock with `SetThreadExecutionState(ExecutionState.EsContinuous);`

6. Since sleep was interrupted, re-send a sleep command with `rundll32.exe powerprof.dll,SetSuspendState 0,1,0`.

What actually happened:
- Sleep continued anyway.
- When PC awoke, it slept immediately again.
- The C# program didn't interrupt sleep no matter what I tried.
- It seems that once a sleep is initiated, it cannot be cancelled by a change in `SetThreadExecutionState` - this must be done *before* the sleep is initiated. OBS replay buffer is a good example of this working.

### Only sleep with custom C# program

- Don't use Task Scheduler, Power Triggers v2
- Don't use start menu, Fn key or even physical PC sleep button to sleep PC
- Instead, only use a shortcut to a C# program that enables adapter, waits for connection, then sends sleep command itself.
- This has too many UX downsides so I didn't consider it.

### The final best approach

- This only leaves this repo's solution as the best approach - execute as fast as possible, without any error checking or status updates.
- This gives us the best chance of WoL adapter successfully establishing a connection before sleep finishes (around 2s according to MSDN).