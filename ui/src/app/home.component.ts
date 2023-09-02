import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from "@angular/common";


@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: 'home.component.html',
  imports: [ CommonModule ],
})
export class HomeComponent implements OnInit {
  dataFromAzureProtectedApi$: Observable<any>;
  userProfileClaims: any;
  isAuthenticated = false;
  constructor(
    private httpClient: HttpClient
  ) {}

  ngOnInit() {
      console.info('home component');
      this.getUserProfile();
  }

  getUserProfile() {
    let data: any = this.httpClient
      .get(`${this.getCurrentHost()}/api/User`)
      .subscribe((result) => {
        data = result;
        console.log(result);

        if(data.isAuthenticated) this.isAuthenticated = true;
        this.userProfileClaims = data;
      });
  }

  callApi() {
    this.dataFromAzureProtectedApi$ = this.httpClient
      .get(`${this.getCurrentHost()}/api/DirectApi`)
      .pipe(catchError((error) => of(error)));
  }

  public getCurrentHost() {
    const host = window.location.host;
    const url = `${window.location.protocol}//${host}`;
    return url;
  }
}
