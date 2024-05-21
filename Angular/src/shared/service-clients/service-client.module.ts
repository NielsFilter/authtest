import { NgModule } from '@angular/core';
import { API_BASE_URL, AccountsClient, NotificationClient } from './service-clients';
import { environment } from '@environments/environment';

@NgModule({
    providers: [
        AccountsClient,
        NotificationClient,
        { provide: API_BASE_URL, useValue: environment.apiUrl } 
    ],
})
export class ServiceClientModule {}
