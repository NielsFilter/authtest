import { NgModule } from '@angular/core';
import { API_BASE_URL, AuthClient, AdminProfileClient, NotificationClient, ProfileClient } from './service-clients';
import { environment } from '@environments/environment';

@NgModule({
    providers: [
        AuthClient,
        ProfileClient,
        AdminProfileClient,
        NotificationClient,
        { provide: API_BASE_URL, useValue: environment.apiUrl } 
    ],
})
export class ServiceClientModule {}
