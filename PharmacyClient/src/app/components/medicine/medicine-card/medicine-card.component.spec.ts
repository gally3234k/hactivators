import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { MedicineCardComponent } from './medicine-card.component';

describe('MedicineCardComponent', () => {
  let component: MedicineCardComponent;
  let fixture: ComponentFixture<MedicineCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicineCardComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([])
      ]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(MedicineCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
