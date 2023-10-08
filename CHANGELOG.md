# Changelog Unwired Modbus

All notable changes to this project will be documented in this file.

## ⭐ [v0.0.3]

> 🐛 **Bugfix** - Value convert error on Read/Write Coils.
> ⌚ **Temporary** - Connection recreated on each request.
> ⚡ **Feature** - Creation of all Read/Write methods in async version.

## ⭐ [v0.1.1]
> 🐛 **Bugfix** - Bug fix when performing value conversions in extension methods. When the ValueType passed in the WriteMultiplesHoldingAsync method was Byte, 
it was always converted to ushort and in some cases it was giving a conversion error, which caused an error when sending data to the PLC.