import {Component, OnInit} from '@angular/core';
import {finalize, first} from 'rxjs/operators';
import { AccountsClient } from 'src/shared/service-clients/service-clients';

@Component({ templateUrl: 'list.component.html' })
export class ListComponent implements OnInit {
    accounts?: any[];
    loading: boolean = false;

    constructor(private accountClient: AccountsClient) { }

    ngOnInit() {
    }

    loadAccounts() {
        this.loading = true;
        this.accountClient.accountsGetAll()
            .pipe(
                first(),
                finalize(() => this.loading = true))
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
