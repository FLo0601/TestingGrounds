import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FaqComponent } from "./faq/faq.component";
import { HelpViewComponent } from "./help-view/help-view.component";
import { LinkTreeComponent } from "./link-tree/link-tree.component";
import { MatTreeModule } from "@angular/material/tree";
import { MatCommonModule } from "@angular/material/core";
import { MatIconModule } from "@angular/material/icon";

@NgModule({
  imports: [
    CommonModule,
    MatTreeModule,
    MatCommonModule,
    MatIconModule
  ],
  exports: [
    FaqComponent,
    HelpViewComponent,
    LinkTreeComponent
  ],
  declarations: [
    FaqComponent,
    HelpViewComponent,
    LinkTreeComponent
  ],
})
export class HelpModule {}
