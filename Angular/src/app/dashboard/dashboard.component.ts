import { Component, Injector } from '@angular/core';
import { AlertService } from '@app/_services';
import { AppComponentBase } from 'src/shared/common/app-component-base';
import { NotificationClient } from 'src/shared/service-clients/service-clients';

@Component({
    templateUrl: 'dashboard.component.html',
    styleUrl: 'dashboard.component.scss'
})
export class HomeComponent extends AppComponentBase{
    counter: number = 1;
    constructor(injector: Injector,
        private alertService: AlertService,
        private notificationClient: NotificationClient) {
            super(injector);
        }

    sendNotification() {
        const pageSize = this.defaultPageSize;
        const index = 0;
        this.notificationClient.notificationGetAll(pageSize, index).subscribe(x => { console.log(x);});
    }

    showSuccess() {
        this.counter++;
        var msg = this.counter %  3 == 0 ? 'This is a short message ' + this.counter : 'This was a long message but there is also a bit more to this story. And here it iss also a bit more to this story. And here it is: ' + this.counter;
        this.alertService.success(msg);
    }
    showWarn() {
        this.counter++;
        var msg = this.counter %  3 == 0 ? 'This is a short message ' + this.counter : 'This was a long message but there is also a bit more to this story. And here it iss also a bit more to this story. And here it is: ' + this.counter;
        this.alertService.warn(msg);
    }
    showInfo() {
        this.counter++;
        var msg = this.counter %  3 == 0 ? 'This is a short message ' + this.counter : 'This was a long message but there is also a bit more to this story. And here it iss also a bit more to this story. And here it is: ' + this.counter;
        this.alertService.info(msg);
    }
    showError() {
        this.counter++;var msg = this.counter %  3 == 0 ? 'This is a short message ' + this.counter : 'This was a long message but there is also a bit more to this story. And here it iss also a bit more to this story. And here it is: ' + this.counter;
        this.alertService.error(msg);
    }

}
