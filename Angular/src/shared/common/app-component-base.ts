import { Component, Injector, OnDestroy, OnInit } from "@angular/core";
import { AppSessionService } from "./session/app-session.service";
import { AccountDto, AccountSessionInfo, AuthenticateDto } from "../service-clients/service-clients";
import { AuthService } from "@app/_services/auth.service";
import { BehaviorSubject, Observable, Subscription } from "rxjs";
import { Role } from "@app/_models";

@Component({
    template: '',
})
export abstract class AppComponentBase implements OnDestroy {

    defaultPageSize: number = 10;
    account: AccountDto | null;
    isAdmin: boolean = false;

    private authSub: Subscription;

    constructor(injector: Injector) { 
        const authService = injector.get(AuthService);
        // Subscribe the currentQuote property of quote service to get real time value
        this.authSub = authService.account$.subscribe(
            // update the component's property
            res => { 
                this.account = res ?? null;
                this.isAdmin = this.account?.roles != null && this.account.roles.indexOf(Role.Admin) > -1;
            }
        );
    }
    ngOnDestroy(): void {
        this.authSub.unsubscribe();
    }
}
