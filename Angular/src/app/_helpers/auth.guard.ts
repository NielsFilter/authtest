import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '@app/_services/auth.service';
import { catchError, map, of, take } from 'rxjs';

export const AuthGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  return authService.account$.pipe(
    take(1), // take the first one and then unsubscribe automatically
    map((account) => {
      if (account?.roles != null) {
        // check if route is restricted by role
        if (route.data.roles) {
          for (const role of route.data.roles) {
            if (account.roles.indexOf(role) < 0) {
              // Unauthorized - don't have all the required roles
              router.navigate(['/']);
              return false;
            }
          }
        }

        // authorized so return true
        return true;
      }

      // not logged in so redirect to login page with the return url
      router.navigate(['/account/login'], {
        queryParams: { returnUrl: state.url },
      });
      return false;
    }),
    catchError((err) => {
      console.error('kicking out', err);
      router.navigate(['/account/login'], {
        queryParams: { returnUrl: state.url },
      });
      return of(false);
    })
  );
};
