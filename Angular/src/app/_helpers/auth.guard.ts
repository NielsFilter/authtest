import { Injectable } from '@angular/core';
import {
  Router,
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { AppSessionService } from 'src/shared/common/session/app-session.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(
    private router: Router,
    private appSessionService: AppSessionService
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return this.appSessionService.accountSession$.pipe(
      map(
        (account) => {
          if (account) {
            // check if route is restricted by role
            if (
              route.data.roles &&
              !route.data.roles.includes(account.account?.role)
            ) {
              // role not authorized so redirect to home page
              this.router.navigate(['/']);
              return false;
            }

            // authorized so return true
            return true;
          }

          // not logged in so redirect to login page with the return url
          this.router.navigate(['/account/login'], {
            queryParams: { returnUrl: state.url },
          });
          return false;
        },
        catchError(() => {
          this.router.navigate(['/account/login'], {
            queryParams: { returnUrl: state.url },
          });
          return of(false);
        })
      )
    );
  }
}
