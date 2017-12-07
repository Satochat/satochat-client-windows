@echo off
pushd bin\Debug
runas /user:Steffen /savecred Satochat.Client.exe
popd
