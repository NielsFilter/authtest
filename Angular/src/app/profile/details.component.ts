import { Component } from '@angular/core';

import { AccountService, AlertService } from '@app/_services';

@Component({ templateUrl: 'details.component.html' })
export class DetailsComponent {
    account = this.accountService.accountValue;

    counter: number = 1;
    constructor(
        private accountService: AccountService,
        private alertService: AlertService) { }

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