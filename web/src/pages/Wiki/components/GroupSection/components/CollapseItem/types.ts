import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import { MouseEventHandler } from 'react';

import { MenuItem } from '@/components/DraggableControlIcon/types';
import { SegmentTypes } from '@/types/wiki';

export interface CollapseItemProps {
	title: string;
	expanded: boolean;
	disabled: boolean;
	getMenuItems: (item: SegmentTypes) => PickerButtonOption<MenuItem>[];
	parentGroupId: string | number | null;
	isMoveMode: boolean;
	id?: string | number;
	onClick?: MouseEventHandler;
}
