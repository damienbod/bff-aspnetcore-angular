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
  dataGraphApiCalls$: Observable<any>;
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
    this.httpClient
      .get(`${this.getCurrentHost()}/api/User`)
      .subscribe((result: any) => {
        console.log(result);

        if(result.isAuthenticated) this.isAuthenticated = true;
        this.userProfileClaims = result;
      });
  }

  getDirectApiData() {
    this.dataFromAzureProtectedApi$ = this.httpClient
      .get(`${this.getCurrentHost()}/api/DirectApi`)
      .pipe(catchError((error) => of(error)));
  }

  getGraphApiDataUsingApi() {
    this.dataGraphApiCalls$ = this.httpClient
      .get(`${this.getCurrentHost()}/api/GraphApiData`)
      .pipe(catchError((error) => of(error)));
  }

  
  private getCurrentHost() {
    const host = window.location.host;
    const url = `${window.location.protocol}//${host}`;
    return url;
  }
}
