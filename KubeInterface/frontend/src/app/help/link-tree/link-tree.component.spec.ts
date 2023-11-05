import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LinkTreeComponent } from './link-tree.component';

describe('LinkTreeComponent', () => {
  let component: LinkTreeComponent;
  let fixture: ComponentFixture<LinkTreeComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [LinkTreeComponent]
    });
    fixture = TestBed.createComponent(LinkTreeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
