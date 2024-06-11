import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map, take } from 'rxjs/operators';
import { AccountsClient } from 'src/shared/service-clients/service-clients';
import { AuthService } from '@app/_services/auth.service';
import { AppSessionService } from 'src/shared/common/session/app-session.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(
        private sessionService: AppSessionService,
        private authService: AuthService
    ) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            if ([401, 403].includes(err.status)) {

                this.sessionService.accountSession$.pipe(
                    take(1),
                    map(account => {
                    if (account) {
                        // auto logout if 401 or 403 response returned from api
                        this.authService.logout();
                    }
                }));
            }

            const error = (err && err.error && err.error.message) || err.statusText;
            console.error(err);
            return throwError(() => error);
        }))
    }
}