import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HelpViewComponent } from './help/help-view/help-view.component';

const routes: Routes = [
  { path: '', redirectTo: '', pathMatch: 'full' },
  { path: 'help-view', component: HelpViewComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
