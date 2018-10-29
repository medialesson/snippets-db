import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-search-box',
  templateUrl: './search-box.component.html',
  styleUrls: ['./search-box.component.scss']
})
export class SearchBoxComponent implements OnInit {
  
  searchTerm;
  @Output() onSearch = new EventEmitter();

  constructor() { }

  ngOnInit() {
  }

  onSearchSubmit(value: string) {
    this.searchTerm = value;
    this.onSearch.emit(this.searchTerm);
  }

}
