import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { HomeComponent } from './home.component';

@Component({
    imports: [HomeComponent, RouterModule],
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'ui';
}
