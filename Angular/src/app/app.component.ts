import { AfterViewInit, Component, HostListener } from '@angular/core';

import { Role } from './_models';
import { SignalrService } from './_services/signalr.service';
import { AccountsClient, RevokeTokenRequest } from 'src/shared/service-clients/service-clients';
import { AuthService } from './_services/auth.service';

@Component({ selector: 'app-root', templateUrl: 'app.component.html' })
export class AppComponent implements AfterViewInit {
    Role = Role;
    sideCollapsed: Boolean = false;
    innerWidth: number;

    constructor(
        private accountsClient: AccountsClient,
        private signalrService: SignalrService,
        private authService: AuthService) {
            this.innerWidth = 0;

            console.log('starting connection');
            this.signalrService.startConnection().subscribe(() => {
                this.signalrService.receiveMessage().subscribe((message) => {
                console.log('got a message: ');
                console.log(message);
            });
          });
    }
    ngAfterViewInit(): void {
        this.innerWidth = window.innerWidth;
    }

    logout() {
        this.authService.logout();
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