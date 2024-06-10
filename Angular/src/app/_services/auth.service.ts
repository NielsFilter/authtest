import { Injectable } from '@angular/core';
import { AccountsClient, RevokeTokenRequest } from 'src/shared/service-clients/service-clients';

@Injectable({ providedIn: 'root' })
export class AuthService {

    refreshTokenTimeout: NodeJS.Timeout;

    constructor(private accountClient: AccountsClient) { }

    startRefreshTokenTimer(token: string) {
        // parse json object from base64 encoded jwt token
        const jwtBase64 = token!.split('.')[1];
        const jwtToken = JSON.parse(atob(jwtBase64));

        // set a timeout to refresh the token a minute before it expires
        const expires = new Date(jwtToken.exp * 1000);
        const timeout = expires.getTime() - Date.now() - (60 * 1000);
        this.refreshTokenTimeout = setTimeout(() => this.refreshToken(), timeout);
    }

    logout() {
        const revokeTokenRequest = new RevokeTokenRequest();
        this.accountClient.accountsRevokeToken(revokeTokenRequest);
        this.stopRefreshTokenTimer();
    }
    
    private stopRefreshTokenTimer() {
        clearTimeout(this.refreshTokenTimeout);
    }

    private refreshToken() {
        this.accountClient.accountsRefreshToken()
            .subscribe(account => {
                this.startRefreshTokenTimer(account.jwtToken!);
            });
    }
}