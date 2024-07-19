import {
  AfterViewInit,
  Component,
  HostListener,
  Injector,
  OnInit,
} from '@angular/core';

import { Role } from './_models';
import { SignalrService } from './_services/signalr.service';
import { AuthService } from './_services/auth.service';
import { AppComponentBase } from 'src/shared/common/app-component-base';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
styleUrl: 'app.component.scss'
 })
export class AppComponent
  extends AppComponentBase
  implements AfterViewInit, OnInit
{
  Role = Role;
  sideCollapsed: Boolean = false;
  innerWidth: number;

  constructor(
    injector: Injector,
    private signalrService: SignalrService,
    private authService: AuthService
  ) {
    super(injector);
    this.innerWidth = 0;

    //todo:
    //     console.log('starting connection');
    //     this.signalrService.startConnection().subscribe(() => {
    //         this.signalrService.receiveMessage().subscribe((message) => {
    //         console.log('got a message: ');
    //         console.log(message);
    //     });
    //   });
  }
  ngAfterViewInit(): void {
    this.innerWidth = window.innerWidth;
  }

  ngOnInit() {
    this.authService.autoLogin();
  }

  logout() {
    this.authService.logout();
  }

  toggleSidebar() {
    this.sideCollapsed = !this.sideCollapsed;
  }

  @HostListener('window:resize', ['$event'])
  onResize() {
    if (window.innerWidth <= 991) {
      // screen is small
      if (this.innerWidth > 991) {
        // screen was large, now reduced. Collapse side bar
        this.sideCollapsed = true;
      }
    } else {
      // screen is large
      if (this.innerWidth <= 991) {
        // screen was small, now increased. Expand side bar
        this.sideCollapsed = false;
      }
    }
    this.innerWidth = window.innerWidth;
  }
}
