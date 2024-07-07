//TODO: 
// import { Injectable } from "@angular/core";
// import { BehaviorSubject, Observable } from "rxjs";
// import { AccountSessionInfo, AccountsClient, AuthenticateDto } from "../../service-clients/service-clients";

// @Injectable()
// export class AppSessionService {
//     private accountSessionSubject: BehaviorSubject<AccountSessionInfo | null> = new BehaviorSubject<AuthenticateDto | null>(null);
//     accountSession$ : Observable<AccountSessionInfo | null> = this.accountSessionSubject.asObservable();

//     refreshTokenTimeout: NodeJS.Timeout;

//     constructor(private _accountClient: AccountsClient) { }
    
//     fetchSessionInfo(): void {
//         this._accountClient.accountsGetAccountSessionInfo()
//             .subscribe(res => this.accountSessionSubject.next(res));
//     }

//     clearSessionInfo(): void {
//         this.accountSessionSubject.next(null);
//     }
// }