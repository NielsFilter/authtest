import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import {
  BehaviorSubject,
  EMPTY,
  Observable,
  catchError,
  take,
  tap,
} from 'rxjs';
import {
  AccountDto,
  AccountsClient,
  AuthenticateDto,
  AuthenticateRequest,
  AuthenticationResult,
} from 'src/shared/service-clients/service-clients';
import { StorageService } from './storage.service';

const AUTH_KEY = 'auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  account$: BehaviorSubject<AccountDto | null> = new BehaviorSubject<AccountDto | null>(null);
  auth$: BehaviorSubject<AuthenticateDto | null> = new BehaviorSubject<AccountDto | null>(null);

  refreshTokenTimeout: NodeJS.Timeout;

  constructor(
    private router: Router,
    private accountClient: AccountsClient,
    private storageService: StorageService
  ) {}

  //TODO: 
  // startRefreshTokenTimer() {
  //   this.stopRefreshTokenTimer();
  //   const authData = this.getStoredAuthResult();
  //   if (authData?.authenticate == null) {
  //     return;
  //   }

  //   // parse json object from base64 encoded jwt token
  //   const jwtBase64 = authData.authenticate.jwtToken!.split('.')[1];
  //   const jwtToken = JSON.parse(atob(jwtBase64));

  //   // set a timeout to refresh the token a minute before it expires
  //   const expires = new Date(jwtToken.exp * 1000);
  //   const timeout = expires.getTime() - Date.now() - 10 * 1000; //TODO:

  //   if (timeout <= 0) {
  //     this.refreshToken()
  //       .pipe(take(1))
  //       .subscribe({
  //         next: (res) => this.updateAuthDetails(res),
  //       });
  //   }

  //   this.refreshTokenTimeout = setTimeout(
  //     () =>
  //       this.refreshToken()
  //         .pipe(take(1))
  //         .subscribe({
  //           next: (res) => this.updateAuthDetails(res),
  //         }),
  //     timeout
  //   );
  // }

  autoLogin() {
    const authData = this.getStoredAuthResult()
    if (authData == null) {
      return;
    }
   //tODO: this.startRefreshTokenTimer();
    this.setAuthResult(authData);
  }

  login(authRequest: AuthenticateRequest): Observable<AuthenticateDto> {
    return this.accountClient.accountsAuthenticate(authRequest).pipe(
      tap({
        next: (res) => this.updateAuthDetails(res),
      })
    );
  }

  private updateAuthDetails(res: AuthenticationResult) {
    this.storeAuthResult(res);
    this.setAuthResult(res);
    //TODO: this.startRefreshTokenTimer();
  }

  private setAuthResult(res: AuthenticationResult | null) {
    this.auth$.next(res?.authenticate ?? null);
    this.account$.next(res?.account ?? null);
  }

  logout() {
    this.clearData();
    console.error('logging out....');
    this.router.navigate(['/account/login']);
  }

  clearData() {
    this.storageService.clean();
    this.setAuthResult(null);
    this.stopRefreshTokenTimer();
  }

  updateAccountInfo(account: AccountDto) {
    this.account$.next(account);
  }

  //todo:
  private stopRefreshTokenTimer() {
    if (this.refreshTokenTimeout) {
      clearTimeout(this.refreshTokenTimeout);
    }
  }

  refreshToken(): Observable<AuthenticationResult> {
    console.log('refreshing token');
    const authData = this.getStoredAuthResult();
    if (authData?.authenticate == null) {
      return EMPTY;
    }

    return this.accountClient
      .accountsRefreshToken(authData.authenticate.refreshToken)
      .pipe(
        take(1),
        tap((res) => {
          this.updateAuthDetails(res);
          //todo: this.setAuthResult(res);
        })
      );
  }

  getStoredAuthResult(): AuthenticationResult | null {
    return this.storageService.getItem<AuthenticationResult>(AUTH_KEY);
  }
  
  private storeAuthResult(result: AuthenticationResult): void {
    this.storageService.saveItem(AUTH_KEY, result);
  }
}
