@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO.
ECHO This script deletes all temporary build files in their
ECHO corresponding BIN and OBJ Folder contained in the following projects
ECHO.
ECHO XmlExplorerDemo
ECHO GenericXmlExplorerDemo
ECHO.
REM Ask the user if hes really sure to continue beyond this point XXXXXXXX
set /p choice=Are you sure to continue (Y/N)?
if not '%choice%'=='Y' Goto EndOfBatch
REM Script does not continue unless user types 'Y' in upper case letter
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
RMDIR .vs /S /Q
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

ECHO.
ECHO Deleting .vs and BIN, OBJ Folders in XmlExplorerDemo project folder
ECHO.

RMDIR /S /Q XmlExplorerDemo\bin
RMDIR /S /Q XmlExplorerDemo\obj

ECHO.
ECHO Deleting .vs and BIN, OBJ Folders in GenericXmlExplorerDemo project folder
ECHO.

RMDIR /S /Q GenericXmlExplorerDemo\bin
RMDIR /S /Q GenericXmlExplorerDemo\obj

ECHO.
ECHO Deleting .vs and BIN, OBJ Folders in Components\XmlExplorerLib project folder
ECHO.

RMDIR /S /Q Components\XmlExplorerLib\bin
RMDIR /S /Q Components\XmlExplorerLib\obj

ECHO.
ECHO Deleting .vs and BIN, OBJ Folders in Components\XmlExplorerVMLib project folder
ECHO.

RMDIR /S /Q Components\XmlExplorerVMLib\bin
RMDIR /S /Q Components\XmlExplorerVMLib\obj

PAUSE

:EndOfBatch
