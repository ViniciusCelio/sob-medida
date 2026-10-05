import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResumeGenerator } from './resume-generator';

describe('ResumeGenerator', () => {
  let component: ResumeGenerator;
  let fixture: ComponentFixture<ResumeGenerator>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ResumeGenerator],
    }).compileComponents();

    fixture = TestBed.createComponent(ResumeGenerator);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
