import { ActionButtonProps } from '@mms/mms-ui-library/components/ActionButton';

import { ExpandableGridCellProps } from '../../types';

export interface ExpandCellRendererOptions<T extends ExpandableGridCellProps> {
	shouldRender?: (row: T) => boolean;
	getHasChildren?: (row: T) => boolean;
	getExpanded?: (row: T) => boolean;
	getIsDisabled?: (row: T, hasChildren: boolean) => boolean;
	onExpandChange?: (row: T, expanded: boolean) => T;
	chevronColor?: string;
	chevronSize?: ActionButtonProps['size'];
}
