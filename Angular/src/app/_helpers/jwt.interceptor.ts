import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, catchError, map, switchMap, take } from 'rxjs';

import { environment } from '@environments/environment';
import { AuthService } from '@app/_services/auth.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    constructor(private authService: AuthService) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        return this.authService.auth$.pipe(
            take(1),
            switchMap(res => {
                const isLoggedIn = res?.jwtToken != null;
                const isApiUrl = req.url.startsWith(environment.apiUrl);
                if (isLoggedIn && isApiUrl) {
                    // add auth header with jwt since account is logged in and request is to the api url
                    req = req.clone({
                      setHeaders: { Authorization: `Bearer ${res?.jwtToken}` }
                    });
                }
                
                return next.handle(req);
            }));
    }
}