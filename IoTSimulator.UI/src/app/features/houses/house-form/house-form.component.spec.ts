import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HouseFormComponent } from './house-form.component';

describe('HouseFormComponent', () => {
  let component: HouseFormComponent;
  let fixture: ComponentFixture<HouseFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HouseFormComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(HouseFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
