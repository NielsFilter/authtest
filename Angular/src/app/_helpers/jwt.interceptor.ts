import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, catchError, map, switchMap, take, throwError } from 'rxjs';

import { environment } from '@environments/environment';
import { AuthService } from '@app/_services/auth.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    constructor(private authService: AuthService) { }
    
      intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        const authResult = this.authService.getStoredAuthResult()?.authenticate;
        const accessToken = authResult?.jwtToken;
        console.log('Checking accesstoken to ADD TO REQUEST', accessToken);
        if (accessToken) {
            request = this.addTokenToRequest(request, accessToken);        
        }

        return next.handle(request).pipe(
          catchError((error) => {
            console.error('Error in request:', error);
            console.warn('error status: ', error.status);
            console.warn('accessToken: ', accessToken);
            // Check if the error is due to an expired access token
            if (error.status === 401 && accessToken) {
                console.warn('handling token expired');
              return this.handleTokenExpired(request, next);
            }
            console.warn('not 401, logging out');
            this.authService.logout();
            return throwError(() => new Error(error));
          })
        );
      }
    
      private addTokenToRequest(request: HttpRequest<any>, token: string): HttpRequest<any> {
        const isApiUrl = request.url.startsWith(environment.apiUrl);
        const isRefreshingToken = request.url.includes('/accounts/refresh-token');
        console.log('adding token to request', isApiUrl, isRefreshingToken, request.url);
        if(isApiUrl && !isRefreshingToken) {
            console.log('ADDING TOKEN', token);
            return request.clone({
            setHeaders: {
                Authorization: `Bearer ${token}`,
            },
            });
        }
        return request;
      }
    
      private handleTokenExpired(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // Call the refresh token endpoint to get a new access token
        return this.authService.refreshToken().pipe(
          switchMap((res) => {
            console.warn('got refresh token result: ', res);
            const newAccessToken = res.authenticate?.jwtToken;
            if(!newAccessToken) {
                return throwError(() => new Error('Unable to refresh token'));
            }

            request = this.addTokenToRequest(request, res.authenticate!.jwtToken!);
            // Retry the original request with the new access token
            return next.handle(request);
          }),
          catchError((error) => {
            // Handle refresh token error (e.g., redirect to login page)
            console.error('Error handling expired access token:', error);
            this.authService.logout();
            return throwError(() => new Error(error));
          })
        );
      }
    }


    // intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    //     return this.authService.auth$.pipe(
    //         take(1),
    //         switchMap(res => {
    //             const isLoggedIn = res?.jwtToken != null;
    //             const isApiUrl = req.url.startsWith(environment.apiUrl);
    //             if (isLoggedIn && isApiUrl) {
    //                 // add auth header with jwt since account is logged in and request is to the api url
    //                 req = req.clone({
    //                   setHeaders: { Authorization: `Bearer ${res?.jwtToken}` }
    //                 });
    //             }
                
    //             return next.handle(req);
    //         }));
    // }
// }