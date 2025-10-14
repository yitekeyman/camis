import {Component} from "@angular/core";
import {CommonModule} from "@angular/common";
import {LbPendingTaskComponent} from "../../land-bank/lb-pending-task/lb-pending-task.component";
import {ReactiveFormsModule} from "@angular/forms";
import {FmPendingTaskComponent} from "../../farm-management/fm-pending-task/fm-pending-task.component";

@Component({
  selector: 'app-pending-task',
  imports: [CommonModule, LbPendingTaskComponent, ReactiveFormsModule, FmPendingTaskComponent],
  templateUrl: './pendingTask.component.html',
  styleUrls: ['./pendingTask.component.scss']
})
export class PendingTaskComponent {
  loginRole = '0';

  constructor() {
    this.loginRole = localStorage.getItem("role");
  }
}
