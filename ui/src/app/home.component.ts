import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: 'home.component.html',
})
export class HomeComponent implements OnInit {
  //dataFromAzureProtectedApi$: Observable<any>;
  isAuthenticated = false;
  constructor(
    private httpClient: HttpClient
  ) {}

  ngOnInit() {
      console.warn('home component');
  }

  callApi() {
    //this.dataFromAzureProtectedApi$ = this.httpClient
    //  .get('https://localhost:5001/api/DirectApi')
    //  .pipe(catchError((error) => of(error)));
  }
}
