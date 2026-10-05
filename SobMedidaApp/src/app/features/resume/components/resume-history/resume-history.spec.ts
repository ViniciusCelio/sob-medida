import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResumeHistory } from './resume-history';

describe('ResumeHistory', () => {
  let component: ResumeHistory;
  let fixture: ComponentFixture<ResumeHistory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ResumeHistory],
    }).compileComponents();

    fixture = TestBed.createComponent(ResumeHistory);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
