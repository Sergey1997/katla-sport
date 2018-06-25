import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HiveService } from '../services/hive.service';
import { Hive } from '../models/hive';

@Component({
  selector: 'app-hive-form',
  templateUrl: './hive-form.component.html',
  styleUrls: ['./hive-form.component.css']
})
export class HiveFormComponent implements OnInit {

  hive = new Hive(0, "", "", "", false, "");
  existed = false;
  hiveId = 0;
  lastUpdate = "";

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private hiveService: HiveService
  ) { }

  ngOnInit() {
    this.route.params.subscribe(p => {
      if (p['id'] === undefined) return;
      this.hiveService.getHive(p['id']).subscribe(h => this.hive = h);
      this.existed = true;
      this.hiveId = p['id'];
    });
  }

  navigateToHives() {
    this.router.navigate(['/hives']);
  }

  onCancel() {
    this.navigateToHives();
  }
  
  onSubmit() {
    if(this.existed == true){
      this.hiveService.updateHive(this.hive).subscribe(c => this.navigateToHives());
    }
    else{
      this.hiveService.addHive(this.hive).subscribe(c => this.navigateToHives());
    }
  }

  onDelete() {
    this.hiveService.setHiveStatus(this.hive.id, false).subscribe(e => this.hive.isDeleted == false);
  }

  onUndelete() {
    this.hiveService.setHiveStatus(this.hive.id, true).subscribe(e => this.hive.isDeleted == true);
  }

  onPurge() {
    this.hiveService.deleteHive(this.hive.id).subscribe(e => this.navigateToHives());
  }
}
