import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '@environments/environment';
import { AppSessionService } from 'src/shared/common/session/app-session.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    constructor(private sessionService: AppSessionService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        this.sessionService.accountSession$.subscribe(res => {

            const isLoggedIn = res?.account;
            const isApiUrl = request.url.startsWith(environment.apiUrl);
            if (isLoggedIn && isApiUrl) {
                request = request.clone({
                    setHeaders: { Authorization: `Bearer ${account.jwtToken}` }
                });
            }


            if (account) {
                // auto logout if 401 or 403 response returned from api
                this.authService.logout();
            }
        });


        // add auth header with jwt if account is logged in and request is to the api url


    

        return next.handle(request);
    }
}