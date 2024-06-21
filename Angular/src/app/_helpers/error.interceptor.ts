import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, concatMap, shareReplay } from 'rxjs/operators';
import { AuthService } from '@app/_services/auth.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((err) => {
        const cleansedError = this.handleError(request, err);
        if ([401, 403].includes(err.status)) {
          this.authService.logout();
          return throwError(() => cleansedError);
        }
        return throwError(() => cleansedError);
      })
    );
  }

  private handleError(request: HttpRequest<any>, err: any) {
    const error = (err && err.error && err.error.message) || err.statusText;
    console.error(err);
    return error;
  }
}
