import { Component } from '@angular/core';
import {AuthService} from "../shared/services/auth.services";
import {ActivatedRoute, Router} from "@angular/router";
import {Alert} from "../shared/models/alert";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  public email: string = '';
  public password: string ='';
  loading = false;
  alerts: Alert[];

  constructor(
    private auth: AuthService, private router: Router, private route: ActivatedRoute
  ) {
    this.alerts = [];
  }

  ngOnInit(): void {
  }

  login() {
    this.loading = true;
    this.auth.login(this.email, this.password).subscribe({
      next: res => {
        this.loading = false;
        const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '';
        this.router.navigateByUrl(returnUrl);
      },
      error: err => {
        this.loading = false;
        this.alerts.push( {
          type : "danger",
          message : 'User not found'
        } as Alert)
      }
    });
  }

  close(alert: Alert) {
    this.alerts.splice(this.alerts.indexOf(alert), 1);
  }
}
