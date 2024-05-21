import {Component} from '@angular/core';

import {AccountService} from '@app/_services';
import { NotificationClient, PagedRequest } from 'src/shared/service-clients/service-clients';

@Component({ templateUrl: 'home.component.html' })
export class HomeComponent {
    account = this.accountService.accountValue;

    constructor(private accountService: AccountService, private notificationClient: NotificationClient) { }

    sendNotification() {
        console.log('sending....');
        const request = new PagedRequest();
        request.index = 0;
        request.pageSize = 10;

        this.notificationClient.notificationGetAll(request).subscribe(x => console.log(x));
    }
}
