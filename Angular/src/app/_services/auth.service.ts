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
  AuthClient,
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
    private authClient: AuthClient,
    private storageService: StorageService
  ) {}

  autoLogin() {
    const authData = this.getStoredAuthResult();
    if (authData == null) {
      return;
    }
    this.setAuthResult(authData);
  }

  login(authRequest: AuthenticateRequest): Observable<AuthenticateDto> {
    return this.authClient.authAuthenticate(authRequest).pipe(
      tap({
        next: (res) => this.updateAuthDetails(res),
      })
    );
  }

  private updateAuthDetails(res: AuthenticationResult) {
    this.storeAuthResult(res);
    this.setAuthResult(res);
  }

  private setAuthResult(res: AuthenticationResult | null) {
    this.auth$.next(res?.authenticate ?? null);
    this.account$.next(res?.account ?? null);
  }

  logout() {
    this.clearData();
    this.router.navigate(['/account/login']);
  }

  clearData() {
    this.storageService.clean();
    this.setAuthResult(null);
  }

  updateAccountInfo(account: AccountDto) {
    this.account$.next(account);
  }

  refreshToken(): Observable<AuthenticationResult> {
    const authData = this.getStoredAuthResult();
    if (authData?.authenticate == null) {
      return EMPTY;
    }

    return this.authClient
      .authRefreshToken(authData.authenticate.refreshToken)
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
