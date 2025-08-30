import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray, FormControl, ValidatorFn, AbstractControl } from '@angular/forms';
import { WalletEntry } from '../../shared/models/wallet-entry';
import { WalletEntryService } from '../../shared/services/wallet-entry.service';
import { Alert } from 'src/app/shared/models/alert';

@Component({
  selector: 'app-add-wallet-entry',
  templateUrl: './add-wallet-entry.component.html',
  styleUrls: ['./add-wallet-entry.component.css']
})
export class AddWalletEntryComponent implements OnInit {
  walletEntryForm!: FormGroup;
  submitted = false;
  alerts: Alert[] = [];

  constructor(private walletEntryService: WalletEntryService) {}

  ngOnInit(): void {
    this.walletEntryForm = new FormGroup({
      description: new FormControl('', [Validators.required, Validators.minLength(3)]),
      value: new FormControl('', [Validators.required, this.notZeroValidator()]),
      date: new FormControl('', Validators.required),
      categories: new FormArray([
        new FormControl('', Validators.required)
      ])
    });
  }

  notZeroValidator(): ValidatorFn {
    return (control: AbstractControl) => {
      const val = Number(control.value);
      if (control.value === '' || isNaN(val)) return null;
      return val === 0 ? { notZero: true } : null;
    };
  }

  get f() { return this.walletEntryForm.controls; }
  get categories() { return this.walletEntryForm.get('categories') as FormArray; }

  addCategory() {
    this.categories.push(new FormControl('', Validators.required));
  }

  removeCategory(index: number) {
    if (this.categories.length > 1) {
      this.categories.removeAt(index);
    }
  }

  onSubmit() {
    this.submitted = true;
    if (this.walletEntryForm.invalid) {
      return;
    }
    const entry: WalletEntry = {
      id: '',
      ...this.walletEntryForm.value
    };
    this.walletEntryService.createWalletEntry(entry).subscribe({
      next: (result) => {
        this.alerts.push({ type: 'success', message: 'Wallet entry created successfully!' });
        this.walletEntryForm.reset();
  this.submitted = false;
      },
      error: (err) => {
        this.alerts.push({ type: 'danger', message: 'Error creating wallet entry.' });
      }
    });
  }

  close(alert: Alert) {
    this.alerts.splice(this.alerts.indexOf(alert), 1);
  }
}
