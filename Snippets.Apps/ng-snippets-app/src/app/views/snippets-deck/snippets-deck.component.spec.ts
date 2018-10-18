import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SnippetsDeckComponent } from './snippets-deck.component';

describe('SnippetsDeckComponent', () => {
  let component: SnippetsDeckComponent;
  let fixture: ComponentFixture<SnippetsDeckComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SnippetsDeckComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SnippetsDeckComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
