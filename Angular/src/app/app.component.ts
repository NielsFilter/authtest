import { AfterViewInit, Component, HostListener } from '@angular/core';

import { AccountService } from './_services';
import { Account, Role } from './_models';
import { SignalrService } from './_services/signalr.service';

@Component({ selector: 'app-root', templateUrl: 'app.component.html' })
export class AppComponent implements AfterViewInit {
    Role = Role;
    account?: Account | null;
    sideCollapsed: Boolean = false;
    innerWidth: number;

    constructor(private accountService: AccountService, private signalrService: SignalrService) {
        this.accountService.account.subscribe(x => this.account = x);
        this.innerWidth = 0;
        this.signalrService.startConnection();
    }
    ngAfterViewInit(): void {
        this.innerWidth = window.innerWidth;
    }

    logout() {
        this.accountService.logout();
    }

    toggleSidebar() {
        this.sideCollapsed = !this.sideCollapsed;
    }

    @HostListener('window:resize', ['$event'])
    onResize() {
        if(window.innerWidth <= 991) {
            // screen is small
            if(this.innerWidth > 991) {
                // screen was large, now reduced. Collapse side bar 
                this.sideCollapsed = true;
            }
        } else {
            // screen is large
            if(this.innerWidth <= 991) {
                // screen was small, now increased. Expand side bar 
                this.sideCollapsed = false;
            }
        }
        this.innerWidth = window.innerWidth;
    }
}