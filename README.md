# SmartPot / GreenGrowTech

An experimental smart plant-pot system combining a custom AVR board, an Android
app, a SQL Server database, a .NET API, a serial gateway and an optional camera.
The app displays plant and pot information and sends pump, greenhouse and photo
requests through the API to the connected hardware.

[Proof-of-concept video](https://www.youtube.com/watch?v=sH559vDkE20&t=21s&ab_channel=RaulSimonca)

## How the parts connect

```text
Android app <--> SQLserverFinale API <--> SQL Server (PlantsDB)
                         ^
                         | HTTP
                       potHub <--> serial/radio <--> SmartPot AVR board

Camera host <--> board UART; camera host --> API image upload
```

`potHub` is a console serial gateway. Although its project uses the web SDK and
has launch profiles, it does not start an HTTP server.

## Repository layout

| Directory | Contents |
| --- | --- |
| `AndroidApp/` | Java Android app (`com.example.greengrowtechapp`). |
| `SQLserverFinale/` | ASP.NET Core 8 API, Entity Framework models and migrations. |
| `potHub/` | .NET 8 serial-to-HTTP gateway for readings and control commands. |
| `SmartPotV4/` | Atmel/Microchip Studio solution with C and C++ firmware projects. |
| `PCB/` | KiCad board designs, component libraries, modules and manufacturing outputs. |
| `CameraCode/` | Python UART/camera scripts and legacy camera installation references. |
| `Programmer/` | Programming, serial, RS485 and demo utilities, including third-party code. |
| `I2CTests/` | Separate firmware experiments for sensors and I2C. |
| `proj_test/` | Supporting experiments and plant-data tools; not needed for normal startup. |

The root files `BackUpDataBase` and `DatabaseBackUp` are legacy SQL Server backup
files, despite having no extension. They are retained as database restore inputs.

## Requirements

- Windows with the .NET 8 SDK for the API and gateway.
- SQL Server or SQL Server LocalDB, plus a database administration tool such as
  SQL Server Management Studio (SSMS).
- Android Studio with JDK 17 and Android SDK 33. The project uses Android Gradle
  Plugin 8.1.0 and the included Gradle 8.0 wrapper. The app requires Android 12
  (API 31) or later.
- For hardware: the matching SmartPot PCB, serial/radio interface and
  Atmel/Microchip Studio with AVR32DA32 support and a compatible programmer.
- For the optional camera: a compatible Linux camera host, Python 3, an enabled
  UART and a working `libcamera-still` command.

Commands below run from the repository root in PowerShell unless stated otherwise.

## 1. Prepare the database

Restore a suitable root backup into a local database, for example `PlantsDB`.
In SSMS, use **Databases > Restore Database > Device**, browse to the backup file
(select all file types if necessary), and review the source database and file
destinations before restoring into a new database.

The API expects `Plant`, `User` and `Pot` tables. The checked-in migrations include
the original schema and a later change to make `PotId` an identity column. This
repository does not automatically create, migrate or seed the database at startup.
Use a restored database as the initial setup path; inspect migrations before
applying them to an existing database because `PotIdentity` drops and recreates
the `PotId` column.

Verify that the database contains the plant and user records you want to use.
A pot references an existing `PlantName` and `UserId`, and `PotName` must be unique.
Backup contents and record IDs may differ between the two backups.

## 2. Configure and start the API

For a fresh checkout, create the local configuration file once:

```powershell
Copy-Item SQLserverFinale/SQLserverFinale/appsettings.example.json SQLserverFinale/SQLserverFinale/appsettings.json
```

If `appsettings.json` already exists, keep it and edit the connection string for
your database instead. This local file is ignored by Git. The example uses
Windows authentication with LocalDB; use your own SQL Server connection string
when running a different instance.

An environment variable can override the connection string for the current shell:

```powershell
$env:ConnectionStrings__DefaultConnection = 'Server=(localdb)\MSSQLLocalDB;Database=PlantsDB;Integrated Security=True;TrustServerCertificate=True;'
```

Start the API on port 3000:

```powershell
dotnet restore SQLserverFinale/SQLserverFinale/SQLserverFinale.csproj
dotnet run --project SQLserverFinale/SQLserverFinale/SQLserverFinale.csproj --no-launch-profile --urls http://0.0.0.0:3000
```

This overrides the old machine-specific address in `launchSettings.json`. When
using an IDE launch profile instead, update that profile to your server's address.
Allow the chosen TCP port through the host firewall for LAN clients.

Check the running API in a second PowerShell window:

```powershell
Invoke-RestMethod http://localhost:3000/api/Plant
Invoke-RestMethod http://localhost:3000/api/Pot
```

Swagger is currently disabled in `Program.cs`, so `/swagger` is not a startup check.

## 3. Configure and run the Android app

1. Open `AndroidApp/` in Android Studio and let Gradle sync.
2. In `app/src/main/java/com/example/greengrowtechapp/MainActivity.java`, set
   `Url` to `http://<API_HOST>:3000/api/`, including the trailing slash. A physical
   phone needs the API computer's LAN address; `localhost` refers to the phone.
   The Android emulator normally reaches the host through `10.0.2.2`.
3. In `app/src/main/res/xml/network_security_config.xml`, add the same host
   without a scheme or port to the domains allowed to use HTTP.
4. Set `indexOfCurrentUser` to an existing database user. Its current source
   default is `303`; this must match the gateway's user for an integrated setup.
5. Ensure that user has at least one pot before starting the app: startup reads
   the first pot returned for that user. Set the initial pot ID consistently in
   `MainActivity.java` if needed.
6. Run the app on an Android 12+ phone or emulator. Use the plant/pot screens to
   view records, select a pot and send control requests.

You can also build a debug APK from PowerShell:

```powershell
Push-Location AndroidApp
.\gradlew.bat assembleDebug
Pop-Location
```

The APK is written to `AndroidApp/app/build/outputs/apk/debug/app-debug.apk`.

## 4. Configure the board and serial gateway

Open `SmartPotV4/SmartPotV4.atsln` in Atmel/Microchip Studio. The fuller sensor and
control implementation is `SmartPotV4_Cpp`; the sibling `SmartPotV4` C project
contains test code. Select the intended project explicitly before building or
flashing. Verify the target, board connections and programmer against the PCB.

In `SmartPotV4/SmartPotV4_Cpp/main.cpp`, `INDEX` is the pot identifier and `TYPE`
is the hardware type. The source currently uses `INDEX = 4` and `TYPE = 2`.
Match the firmware's pot ID to the database record. Do not assume a newly created
identity value equals the firmware's ID.

Before starting the gateway, edit `potHub/potHub/Program.cs`:

- Set `SerialPortStream("COM22", 9600)` to the actual interface and matching baud rate.
- Set **all four** `httpClient.BaseAddress` assignments to the API address, for
  example `http://localhost:3000` when both programs run on the same computer.
- Align `USER`, literal `UserId = 1` assignments and the hard-coded
  `/api/Pot/1/{index}` request with the Android user's database ID. Changing
  `USER` alone does not update all of these values.

These settings are currently in source code; the gateway does not read them from
`appsettings.json` or environment variables.

```powershell
dotnet restore potHub/potHub/potHub.csproj
dotnet run --project potHub/potHub/potHub.csproj --no-launch-profile
```

Start the API first, connect the serial interface, then start the gateway. The
gateway waits for the board's identification data. If it stays at `Looking for
data.`, check the serial port, baud rate and board transmission; reset the board
after opening the gateway if its startup identification was missed.

## 5. Optional camera

On the Linux camera host, confirm `libcamera-still` can capture a photo and the
UART is enabled. In `CameraCode/startup_uart.py`, set the serial device (currently
`/dev/ttyAMA0`, 9600 baud) and upload URL to your setup. The upload endpoint is
`http://<API_HOST>:3000/api/image/upload`.

Run from `CameraCode/` on that host:

```bash
python3 -m venv .venv
source .venv/bin/activate
python -m pip install -r requirements.txt
python startup_uart.py
```

This script reacts to UART bytes `0x15 0x01 0x05`, captures `test.jpg`, and uploads
it as multipart field `image`. The older `finalScript.py` checks decimal `15`
(`0x0F`) as its header instead, so these scripts are not interchangeable. Match
the script to the board's camera command; this needs hardware verification.
An uploaded `test.jpg` is available at `/api/image/test.jpg`.

The camera installation script and link lists are legacy hardware references.
Use a camera stack compatible with the actual camera and OS; the old ARM64 `.deb`
downloads are kept locally when cleaning an existing checkout and are ignored by Git.

## Troubleshooting and current limits

| Symptom | Check |
| --- | --- |
| API fails on database access | SQL instance, database restore, connection string and table names. |
| Phone cannot reach API | API host/port, LAN connection, firewall and Android HTTP domain list. |
| No pots or startup error in app | Existing user ID and at least one associated pot. |
| Gateway cannot open serial port | Correct device, port permissions and another program holding the port. |
| Readings or controls target the wrong pot | Firmware `INDEX`, database `PotId` and user IDs in every client. |
| Photo never arrives | Camera UART command/header, capture command and upload URL. |

This is a prototype with hard-coded network addresses and IDs. The current API
handles user passwords directly and does not implement production authentication;
use test accounts on a trusted development network. Camera integration and the
experimental NPK/SPI paths need verification on the target hardware. No end-to-end
hardware validation is implied by the repository cleanup.

## Git and local files

Track source code, project definitions, migrations, PCB designs and required
libraries. Build outputs, IDE state, camera-host home files, installed Python
packages, caches, KiCad locks/backups and local configuration are ignored.

For an existing checkout, cleanup removes generated and local files from Git's
index while leaving their working copies in place. The large
`Programmer/iot-sunxi-driver/` source tree is third-party code and is retained
with its original documentation and license. The original README also identifies
`Programmer/` as externally sourced; verify attribution before redistributing it.
