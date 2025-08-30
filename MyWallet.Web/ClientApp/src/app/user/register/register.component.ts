import {Component, Inject} from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidationErrors,
  ValidatorFn,
  Validators
} from "@angular/forms";
import {Alert} from "../../shared/models/alert";
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  userForm: FormGroup;
  alerts: Alert[];
  constructor(
    private http: HttpClient,@Inject('BASE_URL') private baseUrl: string, private router: Router
  ) {
    this.userForm =  new FormGroup({
      name: new FormControl(null,[Validators.required]),
      email: new FormControl(null,[Validators.required, Validators.email]),
      password: new FormControl(null,[Validators.required, Validators.minLength(6)]),
      confirmPassword: new FormControl(null,[Validators.required])
    }, { validators: this.confirmPasswordValidator() });
    this.alerts = [];
  }

  confirmPasswordValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const password = control.get('password');
      const confirmPassword = control.get('confirmPassword');
      if (!password || !confirmPassword || password.value === confirmPassword.value) {
        return null;
      }
      return { mismatch: true };
    };
  }

  ngOnInit() { }

  submitForm() {
    if (this.userForm.valid) {
      var request = {};
      request = Object.assign(request,this.userForm.value);
      this.http.post(`${this.baseUrl}user`, request).subscribe({
        next: (data) => {
          this.userForm.reset();
          this.alerts.push( {
            type : "success",
            message : 'New user created'
          } as Alert)
        },
        error: (err) => {
          this.alerts.push({
            type: "danger",
            message: err?.error?.errors?.Email?.join('<br>') ?? err.title // Melhorar a forma como lida com err.errors.Email
          } as Alert);
        }
      });

      return true;
    } else {
      return false;
    }
  }

  close(alert: Alert) {
    this.alerts.splice(this.alerts.indexOf(alert), 1);
  }
}
