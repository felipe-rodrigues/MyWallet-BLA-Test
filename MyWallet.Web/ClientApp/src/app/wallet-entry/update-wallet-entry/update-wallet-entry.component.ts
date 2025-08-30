import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, FormArray, ValidatorFn, AbstractControl } from '@angular/forms';
import { WalletEntryService } from '../../shared/services/wallet-entry.service';
import { WalletEntry } from '../../shared/models/wallet-entry';
import { Alert } from 'src/app/shared/models/alert';

@Component({
  selector: 'app-update-wallet-entry',
  templateUrl: './update-wallet-entry.component.html',
  styleUrls: ['./update-wallet-entry.component.css']
})
export class UpdateWalletEntryComponent implements OnInit {
  walletEntryForm!: FormGroup;
  submitted = false;
  alerts: Alert[] = [];
  entryId!: string;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private walletEntryService: WalletEntryService
  ) {}

  ngOnInit(): void {
    this.walletEntryForm = new FormGroup({
      description: new FormControl('', [Validators.required, Validators.minLength(3)]),
      value: new FormControl('', [Validators.required, this.notZeroValidator()]),
      date: new FormControl('', Validators.required),
      categories: new FormArray([
        new FormControl('', Validators.required)
      ])
    });

    this.entryId = this.route.snapshot.paramMap.get('id')!;
    this.walletEntryService.getWalletEntryById(this.entryId).subscribe({
      next: (entry: WalletEntry) => {
        this.walletEntryForm.patchValue({
          description: entry.description,
          value: entry.value,
          date: entry.date ? entry.date.split('T')[0] : ''
        });
        this.categories.clear();
        if (entry.categories && entry.categories.length) {
          entry.categories.forEach(cat => this.categories.push(new FormControl(cat, Validators.required)));
        } else {
          this.categories.push(new FormControl('', Validators.required));
        }
      },
      error: () => {
        this.alerts.push({ type: 'danger', message: 'Could not load entry.' });
      }
    });
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

  notZeroValidator(): ValidatorFn {
    return (control: AbstractControl) => {
      const val = Number(control.value);
      if (control.value === '' || isNaN(val)) return null;
      return val === 0 ? { notZero: true } : null;
    };
  }

  onSubmit() {
    this.submitted = true;
    if (this.walletEntryForm.invalid) {
      return;
    }
    const entry: WalletEntry = {
      id: this.entryId,
      ...this.walletEntryForm.value
    };
    this.walletEntryService.updateWalletEntry(this.entryId, entry).subscribe({
      next: () => {
        this.alerts.push({ type: 'success', message: 'Wallet entry updated successfully!' });
        setTimeout(() => this.router.navigate(['wallet-entry']), 1200);
      },
      error: () => {
        this.alerts.push({ type: 'danger', message: 'Error updating wallet entry.' });
      }
    });
  }

  close(alert: Alert) {
    this.alerts.splice(this.alerts.indexOf(alert), 1);
  }
}
