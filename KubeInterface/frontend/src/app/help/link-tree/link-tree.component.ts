import {FlatTreeControl} from '@angular/cdk/tree';
import {Component} from '@angular/core';
import {MatTreeFlatDataSource, MatTreeFlattener} from '@angular/material/tree';

/**
 * Food data with nested structure.
 * Each node has a name and an optional list of children.
 */
interface LinkNode {
  name: string;
  children?: LinkNode[];
}

const TEMP_LINKS: LinkNode[] = [
  {
    name: 'FAQ',
    children: [{name: 'How does it work'}, {name: 'Why does it work'}, {name: 'When will it end'} ],
  },
  {
    name: 'PDFs',
    children: [ { name: 'test.pdf' }, { name: 'Orange.pdf'} ],
  },
  {
    name: 'Links',
    children: [ { name: 'Link 1' }, { name: 'Link 2' } ]
  }
];

/** Flat node with expandable and level information */
interface ExampleFlatNode {
  expandable: boolean;
  name: string;
  level: number;
}

/**
 * @title Tree with flat nodes
 */

@Component({
  selector: 'app-link-tree',
  templateUrl: './link-tree.component.pug',
  styleUrls: ['./link-tree.component.scss'],
})
export class LinkTreeComponent {
  private _transformer = (node: LinkNode, level: number) => {
    return {
      expandable: !!node.children && node.children.length > 0,
      name: node.name,
      level: level,
    };
  };

  treeControl = new FlatTreeControl<ExampleFlatNode>(
    node => node.level,
    node => node.expandable,
  );

  treeFlattener = new MatTreeFlattener(
    this._transformer,
    node => node.level,
    node => node.expandable,
    node => node.children,
  );

  dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

  constructor() {
    this.dataSource.data = TEMP_LINKS;
  }

  hasChild = (_: number, node: ExampleFlatNode) => node.expandable;
}
