import { Component, Injector } from '@angular/core';
import { AppComponentBase } from 'src/shared/common/app-component-base';
import { NotificationClient } from 'src/shared/service-clients/service-clients';

@Component({ templateUrl: 'home.component.html' })
export class HomeComponent extends AppComponentBase{
    account = this.accountInfo!;

    constructor(injector: Injector,
        private notificationClient: NotificationClient) { 
            super(injector);
        }

    sendNotification() {
        const pageSize = this.defaultPageSize;
        const index = 0;
        this.notificationClient.notificationGetAll(pageSize, index).subscribe(x => { console.log(x);});
    }
}
