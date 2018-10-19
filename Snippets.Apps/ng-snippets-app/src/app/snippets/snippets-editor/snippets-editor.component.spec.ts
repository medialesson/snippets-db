import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SnippetsEditorComponent } from './snippets-editor.component';

describe('SnippetsEditorComponent', () => {
  let component: SnippetsEditorComponent;
  let fixture: ComponentFixture<SnippetsEditorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SnippetsEditorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SnippetsEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
