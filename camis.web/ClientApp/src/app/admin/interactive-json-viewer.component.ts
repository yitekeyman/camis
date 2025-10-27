// interactive-json-viewer.component.ts
import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from "@angular/common";

@Component({
  selector: 'app-interactive-json-viewer',
  imports: [CommonModule],
  template: `
    <div class="json-viewer">
      <!-- Handle objects -->
      <div *ngIf="isObject(processedData)" class="json-object">
        <div class="json-toggle" (click)="toggleExpanded()">
          <span class="toggle-icon">{{ expanded ? '−' : '+' }}</span>
          {{ title }} {{ isObject(processedData) ? '{' + getObjectKeys(processedData).length + '}' : '' }}
        </div>
        <div *ngIf="expanded" class="json-children">
          <div *ngFor="let key of getObjectKeys(processedData)" class="json-property">
            <span class="json-key">{{ key }}: </span>
            <span class="json-value-inline">
              <app-interactive-json-viewer
                [data]="processedData[key]"
                [title]="key">
              </app-interactive-json-viewer>
            </span>
          </div>
        </div>
      </div>

      <!-- Handle arrays -->
      <div *ngIf="isArray(processedData)" class="json-array">
        <div class="json-toggle" (click)="toggleExpanded()">
          <span class="toggle-icon">{{ expanded ? '−' : '+' }}</span>
          {{ title }} [{{ processedData.length }}]
        </div>
        <div *ngIf="expanded" class="json-children">
          <div *ngFor="let item of processedData; let i = index" class="json-array-item">
            <span class="json-index">{{ i }}: </span>
            <span class="json-value-inline">
              <app-interactive-json-viewer
                [data]="item"
                [title]="'[' + i + ']'">
              </app-interactive-json-viewer>
            </span>
          </div>
        </div>
      </div>

      <!-- Handle primitive values - INLINE -->
      <div *ngIf="isPrimitive(processedData)" class="json-primitive">
        <span [class]="getValueClass()">{{ formatValue(processedData) }}</span>
      </div>
    </div>
  `,
  styles: [`
    .json-viewer {
      font-family: 'Courier New', monospace;
      font-size: 14px;
      line-height: 1.4;
    }
    .json-toggle {
      cursor: pointer;
      user-select: none;
      font-weight: bold;
      padding: 2px 0;
    }
    .json-toggle:hover {
      background-color: #f0f0f0;
      border-radius: 3px;
    }
    .toggle-icon {
      display: inline-block;
      width: 20px;
      text-align: center;
      font-weight: bold;
    }
    .json-children {
      margin-left: 20px;
      border-left: 1px dashed #ccc;
      padding-left: 10px;
    }
    .json-property {
      display: flex;
      align-items: flex-start;
      margin: 2px 0;
    }
    .json-array-item {
      display: flex;
      align-items: flex-start;
      margin: 2px 0;
    }
    .json-key {
      color: #881391;
      font-weight: bold;
      min-width: fit-content;
      margin-right: 4px;
      white-space: nowrap;
    }
    .json-index {
      color: #666;
      font-weight: bold;
      min-width: fit-content;
      margin-right: 4px;
      white-space: nowrap;
    }
    .json-value-inline {
      display: inline;
      word-break: break-word;
    }
    .json-primitive {
      display: inline;
    }
    .json-string {
      color: #c41a16;
    }
    .json-number {
      color: #1c00cf;
    }
    .json-boolean {
      color: #0d22aa;
    }
    .json-null {
      color: #808080;
      font-style: italic;
    }
    .json-undefined {
      color: #808080;
      font-style: italic;
    }
  `]
})
export class InteractiveJsonViewerComponent implements OnChanges {
  @Input() data: any;
  @Input() title: string = 'Object';
  expanded = true;
  processedData: any;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['data']) {
      this.processData();
    }
  }

  private processData(): void {
    if (typeof this.data === 'string') {
      try {
        // Try to parse as JSON
        this.processedData = JSON.parse(this.data);
      } catch (e) {
        // If it's not valid JSON, use the string as is
        this.processedData = this.data;
      }
    } else {
      this.processedData = this.data;
    }
  }

  isObject(value: any): boolean {
    return typeof value === 'object' && value !== null && !Array.isArray(value);
  }

  isArray(value: any): boolean {
    return Array.isArray(value);
  }

  isPrimitive(value: any): boolean {
    return !this.isObject(value) && !this.isArray(value);
  }

  getObjectKeys(obj: any): string[] {
    return Object.keys(obj);
  }

  toggleExpanded(): void {
    this.expanded = !this.expanded;
  }

  getValueClass(): string {
    if (this.processedData === null) return 'json-null';
    if (this.processedData === undefined) return 'json-undefined';

    const type = typeof this.processedData;
    return `json-${type}`;
  }

  formatValue(value: any): string {
    if (value === null) return 'null';
    if (value === undefined) return 'undefined';
    if (typeof value === 'string') return `"${value}"`;
    return value.toString();
  }
}
