import {Component, OnDestroy, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import {WalletEntryService} from "../../shared/services/wallet-entry.service";
import {WalletEntries} from "../../shared/models/wallet-entry";

@Component({
  selector: 'app-wallet-entry-list',
  templateUrl: './wallet-entry-list.component.html',
  styleUrls: ['./wallet-entry-list.component.css']
})
export class WalletEntryListComponent implements OnInit{
  walletEntries: any[] = [];
  loading = false;
  constructor(private router: Router, private walletEntryService: WalletEntryService) {

  }
  ngOnInit(): void {
    this.loadEntries();
  }

  loadEntries() {
    this.loading = true;
    this.walletEntryService.getWalletEntries().subscribe({
      next: (data: WalletEntries) => {
        this.walletEntries = data.items;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  add() {
    this.router.navigate(['add-wallet-entry']);
  }

  remove(entryId: string) {
    if (!confirm('Are you sure you want to remove this entry?')) return;
    this.walletEntryService.deleteWalletEntry(entryId).subscribe({
      next: () => {
        this.loadEntries();
      }
    });
  }

  edit(entryId: string) {
    this.router.navigate(['update-wallet-entry', entryId]);
  }
}
