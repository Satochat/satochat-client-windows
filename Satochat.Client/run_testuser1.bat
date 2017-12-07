@echo off
pushd bin\Debug
runas /user:testuser1 /savecred Satochat.Client.exe
popd
