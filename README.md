# MojeWidelo_WebApi

WebAPI stoi jako usługa io2_api.service na naszej VM.

### Ręczne przebudowanie po wprowadzonych zmianach
    cd ~/io2/team-10-backend
    git pull
    sudo dotnet publish -c Release -o /var/www/io2/backend/
    sudo systemctl restart io2_api.service

### Plik konfiguracyjny dla usługi io2_api.service (nic nie trzeba zmieniać):
    
    /etc/systemd/system/io2_api.service
