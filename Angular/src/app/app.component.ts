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
  templateUrl: 'app.component.html',
  styleUrl: 'app.component.scss'
 })
export class AppComponent
  extends AppComponentBase
  implements AfterViewInit, OnInit
{
  Role = Role;
  sideCollapsed: Boolean = true;
  innerWidth: number;
  mobileSize = 767.98;

  constructor(
    injector: Injector,
    private signalrService: SignalrService,
    private authService: AuthService
  ) {
    super(injector);
    this.innerWidth = 0;

    console.log(window.innerWidth);
    this.sideCollapsed = window.innerWidth > this.mobileSize ? false : true;

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
    // console.log('auto login app');  
    // this.authService.autoLogin();
  }

  logout() {
    this.authService.logout();
  }

  toggleSidebar() {
    this.sideCollapsed = !this.sideCollapsed;
  }

  @HostListener('window:resize', ['$event'])
  onResize() {
    if (window.innerWidth <= this.mobileSize) {
      // screen is small
      if (this.innerWidth > this.mobileSize) {
        // screen was large, now reduced. Collapse side bar
        this.sideCollapsed = true;
      }
    } else {
      // screen is large
      if (this.innerWidth <= this.mobileSize) {
        // screen was small, now increased. Expand side bar
        this.sideCollapsed = false;
      }
    }
    this.innerWidth = window.innerWidth;
  }

  menuClicked() {
    if(this.innerWidth <= this.mobileSize && !this.sideCollapsed) {
        // in mobile view the menu takes over the screen. Selecting an item should collapse the menu again
        this.sideCollapsed = true;
    }
  }
}
