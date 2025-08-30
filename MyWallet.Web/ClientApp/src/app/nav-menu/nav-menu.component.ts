import {Component, Inject} from '@angular/core';
import {Observable} from "rxjs";
import {User} from "../shared/models/user";
import {AuthService} from "../shared/services/auth.services";
import {Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;
  currentUser: Observable<User | null>;
  constructor (private auth: AuthService, private router: Router, private http:HttpClient , @Inject('BASE_URL') private baseUrl: string){
    this.currentUser = this.auth.currentUser;
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['']);
  }
}
