#!/usr/bin/env bash
set -e

echo "Rodando migrações..."
dotnet DebugMeBackend.dll --migrate 2>/dev/null || true

echo "Iniciando aplicação..."
dotnet DebugMeBackend.dll
