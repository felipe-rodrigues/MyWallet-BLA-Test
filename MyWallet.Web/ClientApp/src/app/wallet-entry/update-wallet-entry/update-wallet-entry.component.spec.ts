import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateWalletEntryComponent } from './update-wallet-entry.component';

describe('UpdateWalletEntryComponent', () => {
  let component: UpdateWalletEntryComponent;
  let fixture: ComponentFixture<UpdateWalletEntryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UpdateWalletEntryComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateWalletEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
