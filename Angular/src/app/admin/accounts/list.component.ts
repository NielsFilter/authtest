import {Component, OnInit} from '@angular/core';
import {finalize, first} from 'rxjs/operators';
import { AdminProfileClient, FilterPagedDto } from 'src/shared/service-clients/service-clients';
import { Router, NavigationStart } from '@angular/router';

@Component({
    // TODO: selector: 'list-settings',
    templateUrl: './list.component.html',
    styleUrl: './list.component.less'
})
export class ListComponent implements OnInit {
    accounts: any[];
    loading: boolean = false;

    constructor(
        private router: Router,
        private adminProfileClient: AdminProfileClient) { }

    ngOnInit() {
        this.loadAccounts();
    }

    loadAccounts() {
        this.loading = true;

        //TODO: 
        const input = new FilterPagedDto();

        this.adminProfileClient.adminProfileSearchAllPaged(input)
            .pipe(
                first(),
                finalize(() => this.loading = true))
            .subscribe(accounts => this.accounts = accounts);
    }

    deleteAccount(id: number) {
        const account = this.accounts!.find(x => x.id === id);
        account.isDeleting = true;
        this.adminProfileClient.adminProfileDelete(id)
            .pipe(first())
            .subscribe(() => {
                this.accounts = this.accounts!.filter(x => x.id !== id)
            });
    }

    editAccount(id: number) {
     this.router.navigate(['admin/accounts/edit', id]);   
    }
}
