import {Component} from "@angular/core";
import {CommonModule} from "@angular/common";
import {CardComponent} from "../../theme/shared/components/card/card.component";

@Component({
  selector: 'app-pending-task',
  imports: [CommonModule,CardComponent],
  templateUrl: './pendingTask.component.html',
  styleUrls: ['./pendingTask.component.scss']
})
export class PendingTaskComponent{

}
