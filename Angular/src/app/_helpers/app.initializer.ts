import { catchError, of } from 'rxjs';

export function appInitializer() {
    return () => of();
        //TODO: 
        // accountClient.accountsRefreshToken()
        // .pipe(
        //     // catch error to start app on success or failure
        //     catchError(() => of())
        // );
}