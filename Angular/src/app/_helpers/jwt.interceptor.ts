import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';
import { Observable, catchError, map, switchMap, take, throwError } from 'rxjs';

import { environment } from '@environments/environment';
import { AuthService } from '@app/_services/auth.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const authResult = this.authService.getStoredAuthResult()?.authenticate;
    const accessToken = authResult?.jwtToken;
    if (accessToken) {
      request = this.addTokenToRequest(request, accessToken);
    }

    return next.handle(request).pipe(
      catchError((error) => {
        // Check if the error is due to an expired access token
        if (error.status === 401 && accessToken) {
          return this.handleTokenExpired(request, next);
        }
        this.authService.logout();
        return throwError(() => new Error(error));
      })
    );
  }

  private addTokenToRequest(
    request: HttpRequest<any>,
    token: string
  ): HttpRequest<any> {
    const isApiUrl = request.url.startsWith(environment.apiUrl);
    const isRefreshingToken = request.url.includes('/accounts/refresh-token');
    if (isApiUrl && !isRefreshingToken) {
      return request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      });
    }
    return request;
  }

  private handleTokenExpired(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    // Call the refresh token endpoint to get a new access token
    return this.authService.refreshToken().pipe(
      switchMap((res) => {
        const newAccessToken = res.authenticate?.jwtToken;
        if (!newAccessToken) {
          return throwError(() => new Error('Unable to refresh token'));
        }

        request = this.addTokenToRequest(request, res.authenticate!.jwtToken!);
        // Retry the original request with the new access token
        return next.handle(request);
      }),
      catchError((error) => {
        // Handle refresh token error (e.g., redirect to login page)
        this.authService.logout();
        return throwError(() => new Error(error));
      })
    );
  }
}
