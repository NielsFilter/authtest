import {Component} from '@angular/core';
import {Router} from '@angular/router';
import { AccountsClient } from 'src/shared/service-clients/service-clients';

@Component({ templateUrl: 'layout.component.html' })
export class LayoutComponent {
    constructor(
        private router: Router,
        private accountClient: AccountsClient
    ) {
        // redirect to home if already logged in
        //todo:
        // if (this.accountService.accountValue) {
        //     this.router.navigate(['/']);
        // }
    }
}
