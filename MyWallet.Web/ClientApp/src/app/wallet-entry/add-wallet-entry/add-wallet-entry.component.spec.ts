import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddWalletEntryComponent } from './add-wallet-entry.component';

describe('AddWalletEntryComponent', () => {
  let component: AddWalletEntryComponent;
  let fixture: ComponentFixture<AddWalletEntryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddWalletEntryComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddWalletEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
