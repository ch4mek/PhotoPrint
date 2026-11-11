import { ExpandableGridCellProps } from '../../types';

export const defaultShouldRender = () => true;

export const defaultGetHasChildren = <T extends ExpandableGridCellProps>(
	row: T
) => Boolean(row.hasChildren);

export const defaultGetExpanded = <T extends ExpandableGridCellProps>(row: T) =>
	Boolean(row.expanded);

export const defaultOnExpandChange = <T extends ExpandableGridCellProps>(
	row: T,
	expanded: boolean
) => ({ ...row, expanded });

export const defaultGetIsDisabled = <T extends ExpandableGridCellProps>(
	row: T,
	hasChildren: boolean
) => Boolean(row.disabled) || !hasChildren;
