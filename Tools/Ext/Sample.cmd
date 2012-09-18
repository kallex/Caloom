@ECHO OFF
%~d0
CD "%~dp0"

SET DestStorage=UseDevelopmentStorage=true
SET DestUri=http://127.0.0.1:10000/devstoreaccount1/
REM SET DestStorage=DefaultEndpointsProtocol=http;AccountName=user;AccountKey=key
REM SET DestUri=https://myaccount.blob.core.windows.net/

ECHO == UPLOAD TXT FILES TO TEST-CONTAINER ==
CloudCopy.exe "%~dp0Content\*.txt" "%DestUri%test-container" "%DestStorage%"
ECHO.
IF ERRORLEVEL 0 ECHO OK
IF NOT ERRORLEVEL 0 ECHO Something went wrong!
PAUSE
ECHO.

ECHO == LISTING BLOBS IN TEST-CONTAINER ==
CloudCopy.exe "%DestUri%test-container" "%DestStorage%" -L
ECHO.
PAUSE
ECHO.

ECHO == UPLOADING ALL THE CONTENTS TO TEST-CONTAINER ==
CloudCopy.exe "%~dp0Content" "%DestUri%test-container" "%DestStorage%"
ECHO.
PAUSE
ECHO.

ECHO == LISTING BLOBS IN TEST-CONTAINER ==
CloudCopy.exe "%DestUri%test-container" "%DestStorage%" -L
ECHO.
PAUSE
ECHO.

ECHO == DOWNLOADING TXT FILES ==
CloudCopy.exe "%DestUri%test-container/*.txt" "%~dp0Downloaded" "%DestStorage%"
ECHO.
PAUSE
ECHO.

ECHO == DOWNLOADING ALL THE BLOBS ==
CloudCopy.exe "%DestUri%test-container" "%~dp0Downloaded" "%DestStorage%"
ECHO.
PAUSE
ECHO.

ECHO == COPYING TXT BLOBS FROM TEST-CONTAINER TO TEST-CONTAINER2...
CloudCopy.exe "%DestUri%test-container/*.txt" "%DestUri%test-container2" "%DestStorage%"
ECHO.
PAUSE
ECHO.

ECHO == REMOVING CONTAINERS ==
CloudCopy.exe "%DestUri%test-container" "%DestStorage%" -R
CloudCopy.exe "%DestUri%test-container2" "%DestStorage%" -R
ECHO.
PAUSE