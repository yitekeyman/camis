// json-format.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'jsonFormat'
})
export class JsonFormatPipe implements PipeTransform {
  transform(value: any): any {
    if (typeof value === 'string') {
      try {
        // Try to parse as JSON
        const parsed = JSON.parse(value);
        return this.cleanObject(parsed);
      } catch (e) {
        // If it's not valid JSON, try to clean up the string
        return this.cleanString(value);
      }
    }
    return this.cleanObject(value);
  }

  private cleanObject(obj: any): any {
    if (typeof obj === 'object' && obj !== null) {
      // Recursively clean object properties
      if (Array.isArray(obj)) {
        return obj.map(item => this.cleanObject(item));
      } else {
        const cleaned: any = {};
        Object.keys(obj).forEach(key => {
          cleaned[key] = this.cleanObject(obj[key]);
        });
        return cleaned;
      }
    } else if (typeof obj === 'string') {
      return this.cleanString(obj);
    }
    return obj;
  }

  private cleanString(str: string): string {
    // Remove extra escaping and clean up the string
    return str
      .replace(/\\"/g, '"')
      .replace(/\\\\/g, '\\')
      .replace(/^"|"$/g, '')
      .trim();
  }
}
