import {Component, OnInit} from '@angular/core';
import {first} from 'rxjs/operators';
import { AccountsClient } from 'src/shared/service-clients/service-clients';

@Component({ templateUrl: 'list.component.html' })
export class ListComponent implements OnInit {
    accounts?: any[];

    constructor(private accountClient: AccountsClient) { }

    ngOnInit() {
        this.accountClient.accountsGetAll()
            .pipe(first())
            .subscribe(accounts => this.accounts = accounts);
    }

    deleteAccount(id: number) {
        const account = this.accounts!.find(x => x.id === id);
        account.isDeleting = true;
        this.accountClient.accountsDelete(id)
            .pipe(first())
            .subscribe(() => {
                this.accounts = this.accounts!.filter(x => x.id !== id)
            });
    }
}
