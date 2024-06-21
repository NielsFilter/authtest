import { catchError, of } from 'rxjs';
import { AccountsClient } from 'src/shared/service-clients/service-clients';

export function appInitializer(accountClient: AccountsClient) {
    return () => of();
        //TODO: 
        // accountClient.accountsRefreshToken()
        // .pipe(
        //     // catch error to start app on success or failure
        //     catchError(() => of())
        // );
}