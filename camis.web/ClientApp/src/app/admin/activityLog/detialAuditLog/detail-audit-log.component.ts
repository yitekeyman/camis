import {Component, EventEmitter, Input, OnInit, Output} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {InteractiveJsonViewerComponent} from "../../interactive-json-viewer.component";

@Component({
  selector: "app-auditLog-detail",
  templateUrl: "./detail-audit-log.component.html",
  styleUrls: ["./detail-audit-log.component.scss"],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, InteractiveJsonViewerComponent]
})

export class DetailAuditLogComponent implements OnInit{
  @Input() auditLogItem: any;
  @Input() headerButton=true;
  @Output() closeAuditLogDetails = new EventEmitter();
  processedOldValues: any;
  processedNewValues: any;
  constructor() {
  }
  ngOnInit() {
    if (this.auditLogItem?.auditLog) {
      this.processedOldValues = this.parseJsonData(this.auditLogItem.auditLog.oldValues);
      this.processedNewValues = this.parseJsonData(this.auditLogItem.auditLog.newValues);
    }
  }
  private parseJsonData(data: any): any {
    if (typeof data === 'string') {
      try {
        return JSON.parse(data);
      } catch (e) {
        return data;
      }
    }
    return data;
  }
  closeModal(): void {
    this.closeAuditLogDetails.emit();
  }
}
