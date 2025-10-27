import {Component, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {FarmDetailComponent} from "../../_shared/farm/farm-detail/farm-detail.component";
import {ActivatedRoute, Router} from "@angular/router";
import {FarmApiService} from "../../_services/farm-api.service";
import {ProjectApiService} from "../../_services/project-api.service";
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";
import dialog from "../../_shared/dialog";
import {log} from "ol/console";
import {configs} from "../../app-config";


@Component({
  selector: "app-mne-farm-viewer-component",
  imports: [CommonModule, ReactiveFormsModule, FormsModule, FarmDetailComponent],
  templateUrl: './mne-farm-viewer.component.html',
  styleUrls: ['./mne-farm-viewer.component.scss']
})

export class MneFarmViewerComponent implements OnInit {
  farmId: string;
  farm: any;
  plan: any;
  loading = true;
  loginRole='0';
  urlPrefix = configs.url;

  constructor(
    private router: Router,
              private api: FarmApiService,
              private projectApi: ProjectApiService,
              private ar: ActivatedRoute,
              private keyCase: ObjectKeyCasingService
  ) {
    this.loginRole=localStorage.getItem("role");
  }

  ngOnInit() {

    this.ar.params.subscribe(params => {
      dialog.loading();
      this.farmId = params['farmId'];

      this.api.getFarm(this.farmId).subscribe(farm => {
        this.keyCase.camelCase(farm);
        this.farm = farm;

        this.projectApi.getPlanFromRootActivity(this.farm.activityId).subscribe(plan => {
          this.keyCase.camelCase(plan);
          this.plan = plan;
          this.loading = false;
          dialog.close()
        }, dialog.error);
      }, dialog.error);
    }, dialog.error);
  }

  goToReports(planId: string): void {
    this.router.navigate([`/mne/plan/${planId}/reports`]).catch(dialog.error);
  }

  protected readonly log = log;
}
