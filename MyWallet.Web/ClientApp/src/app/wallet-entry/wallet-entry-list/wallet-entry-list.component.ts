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
  constructor(private router: Router, private walletEntryService: WalletEntryService) {

  }
  ngOnInit(): void {
    this.walletEntryService.getWalletEntries().subscribe({
      next: (data: WalletEntries) => {
        this.walletEntries = data.items;
      }
    });

  }

  add() {
    this.router.navigate(['add-order']);
  }
}
