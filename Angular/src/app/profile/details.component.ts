import { Component } from '@angular/core';

import { AccountService, AlertService } from '@app/_services';

@Component({ templateUrl: 'details.component.html' })
export class DetailsComponent {
    account = this.accountService.accountValue;

    constructor(
        private accountService: AccountService,
        private alertService: AlertService) { }

    showSuccess() {

    }
}