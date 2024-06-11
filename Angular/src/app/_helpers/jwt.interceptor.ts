import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, map, switchMap, take } from 'rxjs';

import { environment } from '@environments/environment';
import { AppSessionService } from 'src/shared/common/session/app-session.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    constructor(private sessionService: AppSessionService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return this.sessionService.accountSession$.pipe(
            take(1),
            map(res => {
                const isLoggedIn = res?.account;
                const isApiUrl = request.url.startsWith(environment.apiUrl);
                if (isLoggedIn && isApiUrl) {
                    // add auth header with jwt since account is logged in and request is to the api url
                    request = request.clone({
                        withCredentials: true
                        //TODO: remove this!!!! setHeaders: { Authorization: `Bearer ${account.jwtToken}` }
                    });
                }
                return next.handle(request);
            }),
            switchMap((res) => res));
    }
}