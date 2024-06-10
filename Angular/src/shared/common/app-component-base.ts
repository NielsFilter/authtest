import { Component, Injector, OnDestroy, OnInit } from "@angular/core";
import { AppSessionService } from "./session/app-session.service";
import { AccountDto, AccountSessionInfo } from "../service-clients/service-clients";

@Component({
    template: '',
})
export abstract class AppComponentBase {

    defaultPageSize: number = 10;
    accountInfo: AccountDto | undefined;
    sessionService: AppSessionService;

    constructor(injector: Injector) { 
        this.sessionService = injector.get(AppSessionService);

        // Subscribe the currentQuote property of quote service to get real time value
        this.sessionService.accountSession$.subscribe(
            // update the component's property
            res => this.accountInfo = res?.account
        );
    }
}
