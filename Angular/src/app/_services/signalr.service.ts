
import { Injectable } from '@angular/core';
import { environment } from '@environments/environment';
import * as signalR from "@microsoft/signalr"
import { Observable } from 'rxjs';
import { AccountsClient } from 'src/shared/service-clients/service-clients';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  private hubConnection!: signalR.HubConnection;

  constructor(private accountClient: AccountsClient) {
    //TODO: 
    // var token = accountService?.accountValue?.jwtToken;
    // if(!token) {
    //   return;
    // }

    // this.hubConnection = new signalR.HubConnectionBuilder()
    //   .withUrl(`${environment.apiUrl}/Notify`, { accessTokenFactory: () => token! }) // SignalR hub URL
    //   .build();
  }

  // startConnection(): Observable<void> {
  //   return new Observable<void>((observer) => {
  //     this.hubConnection
  //       .start()
  //       .then(() => {
  //         console.log('Connection established with SignalR hub');
  //         observer.next();
  //         observer.complete();
  //       })
  //       .catch((error) => {
  //         console.error('Error connecting to SignalR hub:', error);
  //         observer.error(error);
  //       });
  //   });
  // }

  // receiveMessage() : Observable<NotificationResponse> {
  //   return new Observable<NotificationResponse>((observer) => {
  //     this.hubConnection.on('NewNotification', (notification: NotificationResponse) => {
  //       console.log('YES!!!!');
  //       console.log(notification);
  //       observer.next(notification);
  //     });
  //   });
  // }

    // public startConnection = () => {
    //   console.log('starting');
    //   this.hubConnection = new signalR.HubConnectionBuilder()
    //                           .withUrl('https://localhost:58914/Notify',{ skipNegotiation: true,
    //                           transport: signalR.HttpTransportType.WebSockets})
    //                           .build();
    //   this.hubConnection
    //     .start()
    //     .then(() => console.log('Connection started'))
    //     .catch(err => console.log('Error while starting connection: ' + err))
    // }
    
    // public addProductListner = () => {
    //   this.hubConnection.on('NewNotification', (notification: NotificationResponse) => {
    //     this.showNotification(notification);
    //   });
    // }

    // showNotification(notification: NotificationResponse) {
    //   console.log('YAY!!');
    //   console.log(notification);
    //   //TODO: this.toastr.warning( notification.message,notification.productID+" "+notification.productName);
    // }
}

// import { Injectable } from '@angular/core';
// import * as signalR from '@microsoft/signalr';
// import { Observable } from 'rxjs';

// @Injectable({
//   providedIn: 'root',
// })
// export class AppSignalRService {
//   private hubConnection: signalR.HubConnection;

//   constructor() {
//     this.hubConnection = new signalR.HubConnectionBuilder()
//       .withUrl('/realtimehub') // SignalR hub URL
//       .build();
//   }

//   startConnection(): Observable<void> {
//     return new Observable<void>((observer) => {
//       this.hubConnection
//         .start()
//         .then(() => {
//           console.log('Connection established with SignalR hub');
//           observer.next();
//           observer.complete();
//         })
//         .catch((error) => {
//           console.error('Error connecting to SignalR hub:', error);
//           observer.error(error);
//         });
//     });
//   }

//   receiveMessage(): Observable<string> {
//     return new Observable<string>((observer) => {
//       this.hubConnection.on('ReceiveMessage', (message: string) => {
//         observer.next(message);
//       });
//     });
//   }

//   sendMessage(message: string): void {
//     this.hubConnection.invoke('SendMessage', message);
//   }
// }