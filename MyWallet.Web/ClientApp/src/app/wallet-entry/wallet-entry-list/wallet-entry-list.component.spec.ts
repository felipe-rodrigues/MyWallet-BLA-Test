import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WalletEntryListComponent } from './wallet-entry-list.component';

describe('WalletEntryListComponent', () => {
  let component: WalletEntryListComponent;
  let fixture: ComponentFixture<WalletEntryListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WalletEntryListComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WalletEntryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
